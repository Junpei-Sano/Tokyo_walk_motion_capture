using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JavascriptMediaPipe;

/* Pythonnet���g����Mediapipe�����s����ꍇ�̓R�����g�A�E�g���O���i������WebGL�ł̓r���h�G���[�ɂȂ� */

using Python.Runtime;

using UnityEngine.UIElements;


public class MediaPipe_test : MonoBehaviour
{
    private dynamic my_mp;

    [SerializeField] private GameObject head_obj;
    [SerializeField] private GameObject left_obj;
    [SerializeField] private GameObject right_obj;
    [SerializeField] private GameObject _camera;
    // Z���̌��_�����킹��
    private float z_diff = -0.5f;

    private float _rotate_param = 7.0f;
    private float _angle_sum = 0.0f;

    private Vector3 _head, _left, _right;
    private Vector3Smoothing _head_s, _left_s, _right_s;
    private const int _smoothing_frame = 3;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
        GameObject.Find("PlayerData").GetComponent<MediaPipeReciever>().SetMyObject(this.gameObject);
        _head = _left = _right = Vector3.zero;
        _head_s = new Vector3Smoothing(_smoothing_frame);
        _left_s = new Vector3Smoothing(_smoothing_frame);
        _right_s = new Vector3Smoothing(_smoothing_frame);
        Debug.Log("WebGL");

#else
        // �g�p����Python�i���̗�ł�Python3.9�j
        Runtime.PythonDLL = "python39";
        // Python�̏ꏊ�̃p�X
        var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"%userprofile%\AppData\Local\Programs\Python\Python39");
        PythonEngine.PythonHome = PYTHON_HOME;    // �p�X��ݒ�
        PythonEngine.Initialize();    // ������

        using (Py.GIL())    // Python�Ăяo���͂��̒��ɏ���
        {
            dynamic sys = Py.Import("sys");
            // Assets�t�H���_���p�X�ɒǉ�
            sys.path.append(Environment.CurrentDirectory + @"\Assets");
            sys.path.append(Environment.CurrentDirectory + @"\Assets\Characters\MediaPipe");
            //Debug.Log(sys.path);

            dynamic test = Py.Import("mediapipe_test");
            my_mp = test.mediapipe_sample();    // MediaPipe�̃C���X�^���X�𐶐�
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vHead, vLeft, vRight;
#if UNITY_WEBGL
        vHead = _head_s.GetSetValue(_head);
        vLeft = _left_s.GetSetValue(_left);
        vRight = _right_s.GetSetValue(_right);

#else
        using (Py.GIL())
        {
            dynamic val = my_mp.loop();    // �|�[�Y�̍��W�z����󂯎��

            if (val == null) { return; }
            dynamic head = val.landmark[0];    // �@�̍��W
            dynamic right = val.landmark[16];    // �E���̍��W
            dynamic left = val.landmark[15];    // �����̍��W

            // ���W�f�[�^��Python�̌^����Vector3��
            vHead = LandmarkToVector(head);
            vLeft = LandmarkToVector(left);
            vRight = LandmarkToVector(right);
        }
#endif

        // ���z���W��0�ɂ���i��]�����Ȃ��j
        vLeft.z = vRight.z = 0.0f;
        vHead.z += z_diff;

        // ���ꂼ��̒��S�𒱂̒��S�ֈړ�
        Vector3 center_RL = (vRight + vLeft) / 2;
        vHead -= center_RL; vLeft -= center_RL; vRight -= center_RL;
        if (vHead.z < 0) { vHead.z = 0.0f; }    // ���ɂ͐i�܂Ȃ��悤�ɂ���

        // �X���ɉ�������]��������
        float y_diff = vLeft.y - vRight.y;
        _angle_sum += y_diff * _rotate_param;
        Quaternion q = Quaternion.AngleAxis(_angle_sum, Vector3.up);
        vLeft = q * vLeft; vRight = q * vRight; vHead = q * vHead;
        _camera.transform.rotation = q;    // �J��������]

        // ���̂��ړ�
        head_obj.transform.localPosition = vHead;
        left_obj.transform.localPosition = vLeft;
        right_obj.transform.localPosition = vRight;
    }

    public void SetHeadValue(Vector3 v) { _head = v; }
    public void SetLeftHandValue(Vector3 v) { _left = v; }
    public void SetRightHandValue(Vector3 v) { _right = v; }

    // �����h�}�[�N���W�iPython�̃N���X�H�j��Vector3�ɕϊ�����
    // ���_��0����ɕύX
    // ���łɃv���X�E�}�C�i�X���C������
    private Vector3 LandmarkToVector(dynamic landmark)
    {
        Vector3 val = new Vector3(-(float)landmark.x, -(float)landmark.y, -(float)landmark.z);
        return val;
    }

    private void OnApplicationQuit()
    {
        my_mp.close();
        //PythonEngine.Shutdown();
        Debug.Log("Quit");
    }
}

public class Vector3Smoothing
{
    private Vector3[] _v3Array = null;
    private int _idx = 0;

    public Vector3Smoothing(int numFrame)
    {
        _v3Array = new Vector3[numFrame];
        for (int i = 0; i < numFrame; i++)
        {
            _v3Array[i] = Vector3.zero;
        }
    }

    // �t�B���^�����O�֐��i���͓K���A���ݎ����ɋ߂��قǏd�ݑ�j
    // n�͌��ݎ����Ƃ̍��̃t���[����
    private float FilterFunc(int n)
    {
        return (float)Math.Pow(_v3Array.Length - n, 2);
    }

    public Vector3 GetSetValue(Vector3 v)
    {
#if UNITY_WEBGL
        _v3Array[_idx] = v;
        Vector3 sum = Vector3.zero;
        float filterFrameSum = 0.0f;
        for (int i = _idx, j = 0; j < _v3Array.Length; i--, j++)
        {
            if (i < 0) { i = _v3Array.Length - 1; }
            float filter = FilterFunc(j);
            sum += _v3Array[i] * filter;
            filterFrameSum += filter;
        }
        sum /= filterFrameSum;
        if (_idx++ >= _v3Array.Length) { _idx = 0; }
        return sum;
#else
        return Vector3.zero;
#endif
    }
}