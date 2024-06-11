using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class Main_Character : MonoBehaviour
{
    // UDP�֘A
    private readonly int _LOCALPORT = 22222;
    private UdpClient _udp;
    private Thread _thread;

    // �Q�[���֘A
    private readonly float _speed = 10.0f;    // �ړ����x
    private readonly float _jump_power = 20.0f;
    private readonly float _min_motion_time = .0f;    // ���[�V�������ω������Ɣ��f����ŏ�����
    private Rigidbody _rig;
    private string _new_status = "";    // �V���ȉ^���̏��
    private string _current_status = "stop";    // ���݂̉^���̏��
    private float _changed_time = .0f;    // ���[�V�������؂�ւ��������
    private bool _isOnAir = false;

    [SerializeField] private Text _text;

    void Start()
    {
        // UDP�̐ݒ�
        _udp = new UdpClient(_LOCALPORT);
        //_udp.Client.ReceiveTimeout = 1000;
        _thread = new Thread(new ThreadStart(ThreadMethod));
        _thread.Start();

        // �Q�[���̐ݒ�
        _rig = this.GetComponent<Rigidbody>();
    }

    void OnApplicationQuit()
    {
        _thread.Abort();
    }

    private void ThreadMethod()
    {
        while (true)
        {
            try
            {
                IPEndPoint remoteEP = null;
                byte[] data = _udp.Receive(ref remoteEP);
                string text = Encoding.UTF8.GetString(data);
                Debug.Log(text);
                _new_status = text;
            }
            catch (System.Exception e)
            {
                // do nothing
            }
        }
    }

    private void Stop()
    {
        Vector3 speed = _rig.velocity;
        speed.x = speed.z = 0;
        _rig.velocity = speed;
    }

    private void Forward(float speed)
    {
        Vector3 dir = _rig.transform.forward;
        Vector3 velocity = dir * _speed * speed;
        _rig.velocity = velocity;
    }

    private void Jump()
    {
        if (_isOnAir) { return; }
        _isOnAir = true;
        Vector3 dir = _rig.transform.up;
        Vector3 power = dir * _jump_power;
        _rig.AddForce(power, ForceMode.Impulse);
        Debug.Log("jump");
    }

    void Update()
    {
        if (_current_status.Equals(_new_status))
        {
            _changed_time = Time.time;
        }
        else if (Time.time - _changed_time > _min_motion_time)
        {
            _current_status = _new_status;
            _text.text = _current_status;
            switch(_current_status)
            {
                case "stop":
                    this.Stop();
                    break;
                case "walk":
                    this.Forward(1.0f);
                    break;
                case "jump":
                    this.Jump();
                    break;
                case "run":
                    this.Forward(2.0f);
                    break;
                default:
                    Debug.Log("Wrong Message");
                    break;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        _isOnAir = false;
    }
}
