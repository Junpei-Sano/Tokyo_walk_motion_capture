using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Pythonnet���g����Mediapipe�����s����ꍇ�̓R�����g�A�E�g���O���i������WebGL�ł̓r���h�G���[�ɂȂ� */
//using Python.Runtime;

namespace mediapipe_unitychan
{
    public static class VectorToQuaternion
    {
        // 3�_�̍��W����A���[���h���W�ł̍��W���̉�]���v�Z����
        // xz���ʏ��3�_���w�肷��
        // p1->p2��p1->p3����Ȃ�x�N�g���̉E�˂�������y��
        // ����n�ł��邽�ߊO�ς̌������t
        // �Ex�F p1 -> p2 �ւ̃x�N�g��
        // �Ey�F p1, p2, p3 ����Ȃ镽�ʂ̖@���x�N�g��
        // �Ez�F p1, p2, p3 ����Ȃ镽�ʒ���p1->p2�̃x�N�g���ɒ����������
        // �Ԓl�F���[���h���W�n���炱�̍��W�n�ւ�Quaternion
        public static Quaternion CoordinateToRotationXZ(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 axis_x = (p2 - p1).normalized;    // p->p2
            Vector3 axis_y = Vector3.Cross(p3 - p1, p2 - p1).normalized;    // p1,p2,p3���ʂ̖@���x�N�g��
            Vector3 axis_z = Vector3.Cross(axis_x, axis_y);    // p1,p2,p3���ʏ��i�ɒ�������x�N�g��

            Quaternion q = Quaternion.LookRotation(axis_z, axis_y);    // ���W�n�̉�]�𓾂�ix�͈����Ƃ��Ă͕s�v�j
            return q;
        }

        // ��̊֐���xy���ʔ�
        public static Quaternion CoordinateToRotationXY(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 axis_x = (p2 - p1).normalized;    // p->p2
            Vector3 axis_z = Vector3.Cross(p2 - p1, p3 - p1).normalized;    // p1,p2,p3���ʂ̖@���x�N�g��
            Vector3 axis_y = Vector3.Cross(axis_z, axis_x);    // p1,p2,p3���ʏ��i�ɒ�������x�N�g��

            Quaternion q = Quaternion.LookRotation(axis_z, axis_y);    // ���W�n�̉�]�𓾂�ix�͈����Ƃ��Ă͕s�v�j
            return q;
        }

        // ��xy���ʔł́Ap3��p1->p2�����Ɍ��ĉE���ɂ���Ƃ��i�Ȃ����ł��Ȃ��j
        public static Quaternion CoordinateToRotationXY2(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            p1.y *= -1; p2.y *= -1; p3.y *= -1;
            Vector3 axis_x = (p1 - p2).normalized;    // p->p2
            Vector3 axis_z = Vector3.Cross(p2 - p1, p3 - p1).normalized;    // p1,p2,p3���ʂ̖@���x�N�g��
            Vector3 axis_y = Vector3.Cross(axis_z, axis_x);    // p1,p2,p3���ʏ��i�ɒ�������x�N�g��

            Quaternion q = Quaternion.LookRotation(axis_z, axis_y);    // ���W�n�̉�]�𓾂�ix�͈����Ƃ��Ă͕s�v�j
            return q;
        }



        // �e���W���̉�]from���猩���q���W���̉�]to�܂ł̉�]��Ԃ�
        public static Quaternion Get_local_rotation(Quaternion from, Quaternion to)
        {
            Quaternion q = Quaternion.Inverse(from) * to;
            return q;
        }
    }

    public class Bone_Handler
    {
        private int[] p1, p2, p3;    // 3���_�̃C���f�b�N�X�ԍ�
        private Animator anim;
        private HumanBodyBones bone;
        private Bone_Handler parent;
        private Func<Vector3, Vector3, Vector3, Quaternion> _vecToQtn;
        private MySmoothing _sm1 = new MySmoothing(5);
        private MySmoothing _sm2 = new MySmoothing(5);
        private MySmoothing _sm3 = new MySmoothing(5);

        private Vector3 offset_p1 = Vector3.zero;
        private Vector3 offset_p2 = Vector3.zero;
        private Vector3 offset_p3 = Vector3.zero;

        private Vector3 pos1, pos2, pos3;

        public Quaternion rotation = Quaternion.identity;

        public Bone_Handler(Quaternion rotation)
        {
            this.rotation = rotation;
        }

        // xz���ʂȂ��������true
        // ��2-4�����͊e���_�̓Y�����ԍ��A�z��̃x�N�g���̕��ϒl�𗘗p
        // ������
        // �E0�Fxz���ʏ�Ax�����������č�����p3�H�i�v�m�F�j
        // �E2�Fxy���ʏ�Ax�����������č�����p3
        // �E3�Fxy���ʏ�Ax�����������ĉE����p3
        public Bone_Handler(int mode, int[] index_p1, int[] index_p2, int[] index_p3, Animator animator, HumanBodyBones bone, Bone_Handler parent)
        {
            p1 = index_p1; p2 = index_p2; p3 = index_p3;
            this.anim = animator;
            this.bone = bone;
            this.parent = parent == null ? new Bone_Handler(Quaternion.identity) : parent;
            switch (mode)
            {
                case 0:
                    this._vecToQtn = VectorToQuaternion.CoordinateToRotationXZ;
                    break;
                case 2:
                    this._vecToQtn = VectorToQuaternion.CoordinateToRotationXY;
                    break;
                case 3:
                    this._vecToQtn = VectorToQuaternion.CoordinateToRotationXY2;
                    break;
            }
        }

        private Vector3 LandmarkToVector(dynamic landmark)
        {
            Vector3 val = new Vector3(-(float)landmark.x, -(float)landmark.y, -(float)landmark.z);
            return val;
        }

        private Vector3 BonesAverage(dynamic bones, int[] pos)
        {
            Vector3 vec = Vector3.zero;
            for (int i = 0; i < pos.Length; i++)
            {
                vec += LandmarkToVector(bones[pos[i]]);
            }
            vec /= pos.Length;
            return vec;
        }

        // ��̉��ʂȂǂ��C���������Ƃ�
        public void Set_offset(Vector3 p1_offset, Vector3 p2_offset, Vector3 p3_offset)
        {
            this.offset_p1 = p1_offset;
            this.offset_p2 = p2_offset;
            this.offset_p3 = p3_offset;
        }

        public void Rotate(dynamic bones)
        {
            pos1 = _sm1.GetAverage(BonesAverage(bones, this.p1)) + offset_p1;
            pos2 = _sm2.GetAverage(BonesAverage(bones, this.p2)) + offset_p2;
            pos3 = _sm3.GetAverage(BonesAverage(bones, this.p3)) + offset_p3;
            this.rotation = _vecToQtn(pos1, pos2, pos3);
            Quaternion q = VectorToQuaternion.Get_local_rotation(parent.rotation, this.rotation);
            anim.GetBoneTransform(bone).localRotation = q;
        }

        public void ShowObservePoint()
        {
            Debug.LogFormat("{0}: {1}, {2}, {3}", bone.ToString(), pos1, pos2, pos3);
        }
    }

    public class MP_UnityChan : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private dynamic _mediaPipe;

        [SerializeField] private GameObject head_obj;
        [SerializeField] private GameObject left_obj;
        [SerializeField] private GameObject right_obj;

        private List<Bone_Handler> cbones = new List<Bone_Handler>();
        private Bone_Handler hips;
        private Bone_Handler spine;
        private Bone_Handler head;
        private Bone_Handler lshoulder;
        private Bone_Handler lArm;
        private Bone_Handler lfArm;
        private Bone_Handler rshoulder;
        private Bone_Handler rArm;
        private Bone_Handler rfArm;

        // Start is called before the first frame update
        void Start()
        {/*
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
                sys.path.append(Environment.CurrentDirectory + @"\Assets\MediaPipe");
                //Debug.Log(sys.path);

                dynamic test = Py.Import("mediapipe_test");
                _mediaPipe = test.mediapipe_sample();    // MediaPipe�̃C���X�^���X�𐶐�
            }

            _animator = this.GetComponent<Animator>();
            hips = new Bone_Handler(Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.right));
            spine = new Bone_Handler(0, new int[] { 11, 12 }, new int[] { 23, 24 }, new int[] { 23 }, _animator, HumanBodyBones.Spine, hips);
            head = new Bone_Handler(0, new int[] { 2, 5 }, new int[] { 9, 10 }, new int[] { 2 }, _animator, HumanBodyBones.Head, spine);
            head.Set_offset(Vector3.zero, new Vector3(0, 0, 0.06f), Vector3.zero);
            lshoulder = new Bone_Handler(Quaternion.AngleAxis(90, Vector3.right));
            lArm = new Bone_Handler(2, new int[] { 13 }, new int[] { 11 }, new int[] { 15 }, _animator, HumanBodyBones.LeftUpperArm, lshoulder);
            lArm.Set_offset(new Vector3(0, 0, 0.11f), Vector3.zero, new Vector3(0, 0, -0.07f));
            lfArm = new Bone_Handler(0, new int[] { 15 }, new int[] { 13 }, new int[] { 17, 19 }, _animator, HumanBodyBones.LeftLowerArm, lArm);
            lfArm.Set_offset(new Vector3(0, 0, -0.22f), Vector3.zero, new Vector3(0, 0, -0.26f));
            rshoulder = new Bone_Handler(Quaternion.AngleAxis(-90, Vector3.right));
            rArm = new Bone_Handler(3, new int[] { 14 }, new int[] { 12 }, new int[] { 16 }, _animator, HumanBodyBones.RightUpperArm, rshoulder);
            rArm.Set_offset(new Vector3(0, 0, -0.15f), Vector3.zero, new Vector3(0, 0, -0.5f));
            rfArm = new Bone_Handler(3, new int[] { 16 }, new int[] { 14 }, new int[] { 12 }, _animator, HumanBodyBones.RightLowerArm, rArm);
            rfArm.Set_offset(new Vector3(0, 0, -0.35f), Vector3.zero, new Vector3(0, 0, 0.11f));
            cbones.Add(spine);
            cbones.Add(head);
            cbones.Add(lArm);
            cbones.Add(lfArm);
            cbones.Add(rArm);
            cbones.Add(rfArm);*/
        }

        // Update is called once per frame
        void Update()
        {/*
            using (Py.GIL())
            {
                dynamic val = _mediaPipe.loop();    // �|�[�Y�̍��W�z����󂯎��

                if (val != null)
                {
                    foreach (Bone_Handler cbone in cbones)
                    {
                        cbone.Rotate(val.landmark);
                    }
                }
                spine.ShowObservePoint();

                dynamic head = val.landmark[0];    // �@�̍��W
                dynamic right = val.landmark[16];    // �E���̍��W
                dynamic left = val.landmark[15];    // �����̍��W
                Vector3 scale = new Vector3(2, 2, 1);    // Unity��ł̔{�������p
                head_obj.transform.localPosition = Vector3.Scale(LandmarkToVector(head), scale);
                left_obj.transform.localPosition = Vector3.Scale(LandmarkToVector(left), scale);
                right_obj.transform.localPosition = Vector3.Scale(LandmarkToVector(right), scale);
            }*/
        }

        private Vector3 LandmarkToVector(dynamic landmark)
        {
            Vector3 val = new Vector3(-(float)landmark.x, -(float)landmark.y, -(float)landmark.z);
            return val;
        }

        private void OnApplicationQuit()
        {
            _mediaPipe.close();
            //PythonEngine.Shutdown();
            Debug.Log("Quit");
        }
    }

    public class MySmoothing
    {
        private int index = 0;
        private Vector3[] array;
        private int size = 0;

        public MySmoothing(int n)
        {
            size = n;
            array = new Vector3[n];
        }

        public Vector3 GetAverage(Vector3 v)
        {
            array[index++] = v;
            if (index >= size) { index = 0; }
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < size; i++)
            {
                sum += array[i];
            }
            sum /= size;
            return sum;
        }
    }
}
