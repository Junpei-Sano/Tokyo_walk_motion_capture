using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerManager;    // �ǉ�

using MyPhotonMessage;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;
using UnityEngine.Windows;
using Photon.Realtime;
using Butterfly_NPC;

/*
 * ����
 * �E��]�͐e�I�u�W�F�N�g�𓮂����Ȃ��AModel�̂݉�]������
 * �@�E����ĕ����擾��Model��transform���g�p���邱��
 * �E�ړ��͐e�I�u�W�F�N�g�𓮂���
 * �EInput_system�̎��e�q�֌W���ς��̂Œ���
 * */

public class SampleCharacter3 : PlayerInformationBase
{
    private ReadInput _input;
    private Rigidbody _rigidbody;
    private Transform _model;
    private Butterfly.Motion _butterfly;
    private Arrow _arrow;

    [SerializeField] private GameObject _deathObj;
    [SerializeField] private MessagePanelController _msgPanel;
    [SerializeField] private Player_collisionNPC _npc_collision;

    private readonly float _deathMagnitude = 20;
    private readonly float _acceleration = 400;
    private readonly float _maxVelocity = float.MaxValue;    // �������Ȃ��ꍇ�́ufloat.MaxValue�v
    private readonly float _mass = 1.0f;
    private readonly float _drag = .5f;
    private readonly float _anglarDrag = 1.0f;
    private Vector3 _gravity = new Vector3(0, -9.8f, 0);
    //private Vector3 _gravity = Vector3.zero;
    private readonly float _propellingPower = 4f;
    private readonly float _jumpPower = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = this.GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = new Vector3(0, 0f, 0);
        _model = this.transform.Find("Model").gameObject.transform;
        _arrow = _model.transform.Find("Forward/Arrow").gameObject.GetComponent<Arrow>();

        _rigidbody.mass = _mass;
        _rigidbody.drag = _drag;
        _rigidbody.angularDrag = _anglarDrag;
        Physics.gravity = _gravity;
        _rigidbody.useGravity = true;
    }

    private void Start()
    {
        GameObject butterfly = this.transform.Find("Model/Model").gameObject;

        switch (base.controllerType)
        {
            case ControllerType.input_system_wing:
                _input = new ReadInputSystemWing();    //�A�b�v�L���X�g
                _butterfly = new Butterfly.Motion_InputSystem(butterfly, _input);
                // Open XR�ɂ��ύX
                //this.transform.Find("OVRPlayerController").parent = _model;
                this.transform.Find("XR Origin").parent = _model;
                break;
            case ControllerType.oculus_wing:
                //_input = new ReadOculusWing(this.gameObject, _model);
                _butterfly = new Butterfly.Motion_Oculus(butterfly, this.gameObject);
                break;
            case ControllerType.open_xr_wing:
                _input = new ReadOpenXRWing(this.gameObject, _model);
                _butterfly = new Butterfly.Motion_OpenXR(butterfly, this.gameObject);
                break;
            case ControllerType.udp_mediapipe:
                _input = new ReadMediaPipe(this.gameObject);
                _butterfly = new Butterfly.Motion_OpenXR(butterfly, this.gameObject);
                this.transform.Find("XR Origin").parent = _model;
                break;
            default:
                Debug.LogWarning("Unsupported Controller type: " + base.controllerType);
                break;
        }
        this.GetComponent<CapsuleCollider>().enabled = true;
        _deathObj.SetActive(false);
        _msgPanel.InitPanel(this, _input);
    }

    private void Rudder(float angle)
    {
        Vector3 plane = _model.InverseTransformDirection(Vector3.up);
        _model.rotation *= Quaternion.AngleAxis(angle, plane);
    }

    private void Elevator(float angle)
    {
        _model.rotation *= Quaternion.AngleAxis(angle, Vector3.right);
    }

    private void Aileron(float angle)
    {
        _model.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Throttle(float value)
    {
        Vector3 direction = _model.up;
        Vector3 power = direction * value * _acceleration * -Physics.gravity.y * Time.deltaTime;

        Vector2 velocityXY = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
        //Debug.LogFormat("Speed = {0}", velocityXY.magnitude);
        if (velocityXY.magnitude > _maxVelocity) { power.x = power.z = 0.0f; }
        _rigidbody.AddForce(power, ForceMode.Acceleration);    // y�����ɗ�
    }

    private void Fire()
    {
        _npc_collision.Fire();
    }

    private void PropellingPower()
    {
        if (_rigidbody.velocity.y < 0)    // ���~���̂Ƃ�
        {
            Vector3 power = -_model.up * _rigidbody.velocity.y * _propellingPower * -Physics.gravity.y * Time.deltaTime;
            Vector2 velocityXY = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            if (velocityXY.magnitude > _maxVelocity) { power.x = power.z = 0.0f; }
            _rigidbody.AddForce(power, ForceMode.Acceleration);
        }
    }

    private void PlayerDeath(DeathPattern d)
    {
        if (_deathObj == null)
        {
            Debug.LogWarning("Cannot find the object of death display.");
            return;
        }
        Debug.Log("You Died");
        _deathObj.GetComponent<DeathMessage>().deathPattern = d;
        _deathObj.SetActive(true);
        this.GetComponent<SampleCharacter3>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float magnitude = collision.impulse.magnitude;
        //Debug.LogFormat("Collision: {0}, Magnitude = {1}", collision.gameObject.name, magnitude);

        if (magnitude > _deathMagnitude)
        {
            PlayerDeath(DeathPattern.collision);
        }
    }

    /*
    private void OnCollisionStay(Collision collision)
    {
        float pow = _input.JumpAction();
        if (pow >= 1f && !_deathObj.activeSelf)    // �}�W�b�N�i���o�[�͓K���Ȑ��i���W�����A���ɂ��n�ʂɒ����Ȃ���Ԃ�h���j
        {
            Debug.Log("Jump");
            Throttle(pow * _jumpPower);
        }
    }
    */

    private void FallVoidCheck()    // �ޗ��`�F�b�N
    {
        if (this.transform.position.y < -250)
        {
            PlayerDeath(DeathPattern.fallVoid);
            _rigidbody.velocity = Vector3.zero;    // ��~������
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_input.ReadFireButton())
        {
            //_arrow.TargetSelection();
            Fire();
        }

        // ���쐫�̊֌W���烂�[�h1�̊��蓖��
        Vector2 stickR = _input.ReadStick_R();
        Vector2 stickL = _input.ReadStick_L();

        Rudder(stickL.x);
        Elevator(stickL.y);
        Aileron(stickR.x);
        Throttle(stickR.y);

        PropellingPower();
        FallVoidCheck();

        _butterfly.Flap();

        // �������x�̐���
        if (_rigidbody.velocity.y < -_maxVelocity)
        {
            // �d�͂�ł�����
            _rigidbody.AddForce(-_gravity * Time.deltaTime, ForceMode.Impulse);
        }
    }
}
/*
public class ReadOculusWing : ReadInput    // Oculus�̃R���g���[�����g�p
{
    private float _throttlePower = 3;

    private Transform _model;
    private GameObject _centerEye;
    private GameObject _LeftHand;
    private GameObject _RightHand;

    private float _previousY = 0.0f;

    private float _previousEyeAveY = 0.0f;
    private float _jumpLowest = float.MaxValue;
    private float _jumpTime = float.MaxValue;
    private bool _isJumping = false;

    public ReadOculusWing(GameObject player, Transform model)
    {
        Debug.Log("Input: Oculus (Wing)");

        _centerEye = player.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/CenterEyeAnchor").gameObject;
        _LeftHand = player.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerAnchor").gameObject;
        _RightHand = player.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerAnchor").gameObject;

        _model = model;
    }

    private float VectorAngle(Vector3 from, Vector3 to, Vector3 planeNormal)
    {
        Vector3 planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
        Vector3 planeTo = Vector3.ProjectOnPlane(to, planeNormal);
        float angle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);
        return angle;
    }

    // �n�ʂɒ����Ă���ۂ̃W�����v����
    public override float JumpAction()
    {
        float power = 0.0f;
        float currentY = _centerEye.transform.position.y;
        float currentAveY = _centerEye.transform.position.y + _RightHand.transform.position.y + _LeftHand.transform.position.y;
        float dy = currentAveY - _previousEyeAveY;
        float time = Time.time - _jumpTime;
        if (time > 2.0 && _isJumping == true)    // 2�b�ȏ�̂Ƃ��̓W�����v�łȂ��Ɣ���
        {
            _jumpTime = Time.time;
            _isJumping = false;
        }
        else if (dy > 0 && _isJumping == false)    // 3�_�̕��ϒl���㏸��
        {
            _isJumping = true;
            _jumpTime = Time.time;
        }
        else if (dy <= 0)
        {
            if (_isJumping == true)
            {
                power = currentY - _jumpLowest;
                power /= time;
                _isJumping = false;
                _jumpLowest = float.MaxValue;
            }
            else
            {
                _jumpLowest = currentY;
            }
        }
        _previousEyeAveY = currentAveY;
        return power;
    }

    public override Vector2 ReadStick_L()
    {
        Vector2 angle;
        // ���_�[
        Vector3 rightFrom = _model.transform.right;
        Vector3 rightTo = (_RightHand.transform.position - _LeftHand.transform.position).normalized;
        Vector3 plane = _model.InverseTransformDirection(Vector3.up);
        angle.x = VectorAngle(rightFrom, rightTo, plane);

        // �G���x�[�^�[�E2�̃R���g���[���̒��_����{�̂ɐL�т�x�N�g��
        Vector3 upFrom = _model.transform.up;
        Vector3 center = (_RightHand.transform.position + _LeftHand.transform.position) / 2;
        Vector3 upTo = (_centerEye.transform.position - center).normalized;
        angle.y = VectorAngle(upFrom, upTo, _model.right);

        return angle;
    }

    public override Vector2 ReadStick_R()
    {
        Vector2 angle = Vector2.zero;
        // �G������
        Vector3 rightFrom = _model.transform.right;
        Vector3 rightTo = (_RightHand.transform.position - _LeftHand.transform.position).normalized;
        angle.x = VectorAngle(rightFrom, rightTo, _model.forward);

        // �X���b�g��
        Vector3 average = (_RightHand.transform.position + _LeftHand.transform.position) / 2;
        float currentY = _centerEye.transform.position.y - average.y;
        float dy = currentY - _previousY;
        if (dy > 0 && _previousY != 0.0f)
        {
            angle.y = dy * _throttlePower;
        }
        _previousY = currentY;

        return angle;
    }

    public override bool ReadQuitButton()
    {
        return OVRInput.GetDown(OVRInput.Button.Start);
    }

    public override bool ReadFireButton()
    {
        return OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
    }

    public override bool ReadMessageButton()
    {
        return OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger);
    }

    public override bool ReadEscButton()
    {
        return ReadMessageButton();
    }
}
*/

// �R���X�g���N�^�̃����o�ϐ��w��̏ꏊ�i��A���E�̎�̃I�u�W�F�N�g�w��j�ȊO�͏�ƑS������
public class ReadOpenXRWing : ReadInput    // Oculus�̃R���g���[�����g�p
{
    private float _throttlePower = 3;

    private Transform _model;
    private GameObject _centerEye;
    private GameObject _LeftHand;
    private GameObject _RightHand;

    private float _previousY = 0.0f;

    private float _previousEyeAveY = 0.0f;
    private float _jumpLowest = float.MaxValue;
    private float _jumpTime = float.MaxValue;
    private bool _isJumping = false;

    public ReadOpenXRWing(GameObject player, Transform model)
    {
        Debug.Log("Input: XR Interaction Toolkit (Wing)");

        _centerEye = player.transform.Find("XR Origin/Camera Offset/Main Camera").gameObject;
        _LeftHand = player.transform.Find("XR Origin/Camera Offset/LeftHand Controller").gameObject;
        _RightHand = player.transform.Find("XR Origin/Camera Offset/RightHand Controller").gameObject;

        _model = model;
    }

    private float VectorAngle(Vector3 from, Vector3 to, Vector3 planeNormal)
    {
        Vector3 planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
        Vector3 planeTo = Vector3.ProjectOnPlane(to, planeNormal);
        float angle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);
        return angle;
    }

    // �n�ʂɒ����Ă���ۂ̃W�����v����
    public override float JumpAction()
    {
        float power = 0.0f;
        float currentY = _centerEye.transform.position.y;
        float currentAveY = _centerEye.transform.position.y + _RightHand.transform.position.y + _LeftHand.transform.position.y;
        float dy = currentAveY - _previousEyeAveY;
        float time = Time.time - _jumpTime;
        if (time > 2.0 && _isJumping == true)    // 2�b�ȏ�̂Ƃ��̓W�����v�łȂ��Ɣ���
        {
            _jumpTime = Time.time;
            _isJumping = false;
        }
        else if (dy > 0 && _isJumping == false)    // 3�_�̕��ϒl���㏸��
        {
            _isJumping = true;
            _jumpTime = Time.time;
        }
        else if (dy <= 0)
        {
            if (_isJumping == true)
            {
                power = currentY - _jumpLowest;
                power /= time;
                _isJumping = false;
                _jumpLowest = float.MaxValue;
            }
            else
            {
                _jumpLowest = currentY;
            }
        }
        _previousEyeAveY = currentAveY;
        return power;
    }

    public override Vector2 ReadStick_L()
    {
        Vector2 angle;
        // ���_�[
        Vector3 rightFrom = _model.transform.right;
        Vector3 rightTo = (_RightHand.transform.position - _LeftHand.transform.position).normalized;
        Vector3 plane = _model.InverseTransformDirection(Vector3.up);
        angle.x = VectorAngle(rightFrom, rightTo, plane);

        // �G���x�[�^�[�E2�̃R���g���[���̒��_����{�̂ɐL�т�x�N�g��
        Vector3 upFrom = _model.transform.up;
        Vector3 center = (_RightHand.transform.position + _LeftHand.transform.position) / 2;
        Vector3 upTo = (_centerEye.transform.position - center).normalized;
        angle.y = VectorAngle(upFrom, upTo, _model.right);

        return angle;
    }

    public override Vector2 ReadStick_R()
    {
        Vector2 angle = Vector2.zero;
        // �G������
        Vector3 rightFrom = _model.transform.right;
        Vector3 rightTo = (_RightHand.transform.position - _LeftHand.transform.position).normalized;
        angle.x = VectorAngle(rightFrom, rightTo, _model.forward);

        // �X���b�g��
        Vector3 average = (_RightHand.transform.position + _LeftHand.transform.position) / 2;
        float currentY = _centerEye.transform.position.y - average.y;
        float dy = currentY - _previousY;
        if (dy > 0 && _previousY != 0.0f)
        {
            angle.y = dy * _throttlePower;
        }
        _previousY = currentY;
        return angle;
    }

    public override bool ReadEscButton()
    {
        return ReadMessageButton();
    }
}


public class ReadInputSystemWing : ReadInput    // input system�i�L�[�{�[�h�j�g�p
{
    private float _RudderSpeed = 1.0f;
    private float _elevatorSpeed = 1.0f;
    private float _aileronSpeed = 30.0f;
    private float _throttlePower = 2.0f;

    private Test_Actions _input;

    public ReadInputSystemWing()
    {
        _input = new Test_Actions();
        _input.Enable();
        Debug.Log("Input: Input System");
    }

    public override Vector2 ReadStick_R()
    {
        Vector2 val = Vector2.zero;
        val.x = -_input.User.Rotate.ReadValue<Vector2>().x * _aileronSpeed * Time.deltaTime;
        if (this.ReadUpButton())
        {
            val.y = _throttlePower;
        }
        return val;
    }

    public override Vector2 ReadStick_L()
    {
        Vector2 val = _input.User.Move.ReadValue<Vector2>();
        val.x *= _RudderSpeed;
        val.y *= _elevatorSpeed;
        return val;
    }

    public override bool ReadQuitButton()
    {
        return _input.User.Quit.triggered;
    }

    public override bool ReadFireButton()
    {
        return _input.User.Fire.triggered;
    }

    public override bool ReadUpButton()
    {
        return _input.User.Up.triggered;
    }
    public override bool ReadMessageButton()
    {
        return _input.User.Message.triggered;
    }

    public override bool ReadEscButton()
    {
        return _input.User.Esc.triggered;
    }

    public override float JumpAction()
    {
        if (_input.User.Jump.triggered)
        {
            Debug.Log("Jump");
            return 1;
        }
        else
        {
            return 0;
        }
    }
}


public class ReadMediaPipe : ReadInput    // MediaPipe(UDP)�g�p�iInputSystem���ꕔ�Ŏg�p�j
{
    private float _RudderSpeed = 1.0f;
    private float _elevatorSpeed = .5f;
    //private float _aileronSpeed = 30.0f;
    private float _throttlePower = 3.0f;

    // InputSystem�֌W
    private Test_Actions _input;

    // UDP�֘A
    private readonly int _LOCALPORT = 22222;
    private UdpClient _udp;
    private Thread _thread;

    // �Q�[���֘A
    private string _current_status = "stop";    // ���݂̉^���̏��
    private bool _isFlap = false;
    private bool _isJump = false;
    private bool _isForward = false;
    private float _tilt_sum = .0f;
    private bool _isOnFire = false;
    private float _nose_y = .0f;
    private float _left_y = .0f;
    private float _right_y = .0f;

    // �H�΂������[�V�����֌W
    private GameObject _head_obj;
    private GameObject _left_obj;
    private GameObject _right_obj;


    public ReadMediaPipe(GameObject player)
    {
        Debug.Log("Input: MediaPipe(UDP)");
        _input = new Test_Actions();
        _input.Enable();
        // UDP�̐ݒ�
        _udp = new UdpClient(_LOCALPORT);
        _thread = new Thread(new ThreadStart(ThreadMethod));
        _thread.Start();

        // ���[�V�����֌W�̐ݒ�
        _head_obj = player.transform.Find("XR Origin/Camera Offset/Main Camera").gameObject;
        _left_obj = player.transform.Find("XR Origin/Camera Offset/LeftHand Controller").gameObject;
        _right_obj = player.transform.Find("XR Origin/Camera Offset/RightHand Controller").gameObject;
    }

    ~ReadMediaPipe()
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
                //Debug.Log(text);
                //_new_status = text;
                this.ReceiveMessage(text);
            }
            catch (System.Exception e)
            {
                // do nothing
            }
        }
    }

    // ���b�Z�[�W�`���F�u���b�Z�[�W��� ����1,����2,...�v
    private void ReceiveMessage(string message)
    {
        string[] msgs = message.Split(' ');    // ���b�Z�[�W��ʂƈ����̋��E�͋�
        switch (msgs[0])
        {
            case "command":
                CommandAction(msgs[1]);
                break;
            case "state":
                StateAction(msgs[1]);
                break;
            default:
                Debug.LogWarning("Wrong Message");
                break;
        }
    }

    private void CommandAction(string command)
    {
        if (!_current_status.Equals(command))
        {
            _current_status = command;
            Debug.Log(_current_status);
            switch (_current_status)
            {
                case "stop":
                    break;
                case "down":
                    _isFlap = true;
                    break;
                case "up":
                    break;
                case "forward":
                    _isForward = true;
                    break;
                case "fire":
                    _isOnFire = true;
                    break;
                case "jump":
                    _isJump = true;
                    break;
                default:
                    Debug.LogWarning("Wrong Command");
                    break;
            }
        }
    }

    private void StateAction(string state)
    {
        string[] param = state.Split(',');
        _nose_y = float.Parse(param[0]);
        _left_y = float.Parse(param[1]);
        _right_y = float.Parse(param[2]);
    }

    public override Vector2 ReadStick_R()
    {
        Vector2 val = Vector2.zero;
        //val.x = -_input.User.Rotate.ReadValue<Vector2>().x * _aileronSpeed * Time.deltaTime;
        //val.x = this._right *_aileronSpeed * Time.deltaTime;
        val.x = 0;
        if (_isFlap)
        {
            val.y = _throttlePower;
            _isFlap = false;
        }
        return val;
    }

    public override Vector2 ReadStick_L()
    {
        Vector2 val;
        //val = _input.User.Move.ReadValue<Vector2>();

        float right_tilt = _left_y - _right_y;
        if (Mathf.Abs(right_tilt) < .05f) { right_tilt = .0f; }
        float front_tilt = 0.0f;
        if (_isForward)
        {
            if (!_current_status.Equals("forward"))
            {
                _isForward = false;
            }
            else
            {
                front_tilt = 1.0f;
            }
        }
        else if (_tilt_sum > 0)
        {
            front_tilt = -1.0f;
        }
        _tilt_sum += front_tilt;

        val.x = right_tilt;
        val.y = front_tilt;
        val.x *= _RudderSpeed;
        val.y *= _elevatorSpeed;

        // ���[�V�����֌W
        // ���ꂼ��̒��S�𒱂̒��S�ֈړ�
        Vector3 vHead = _head_obj.transform.localPosition;
        Vector3 vRight = _right_obj.transform.localPosition;
        Vector3 vLeft = _left_obj.transform.localPosition;
        vHead.y = _nose_y; vLeft.y = _left_y; vRight.y = _right_y;
        float center_RL = (vRight.y + vLeft.y) / 2;
        vHead.y -= center_RL; vLeft.y -= center_RL; vRight.y -= center_RL;
        vLeft.x = -1f; vRight.x = 1f;
        if (vHead.z < 0) { vHead.z = 0.0f; }    // ���ɂ͐i�܂Ȃ��悤�ɂ���
        // ���̂��ړ�
        _head_obj.transform.localPosition = vHead;
        _left_obj.transform.localPosition = vLeft;
        _right_obj.transform.localPosition = vRight;

        return val;
    }

    public override bool ReadEscButton()
    {
        return _input.User.Esc.triggered;
    }

    public override bool ReadFireButton()
    {
        if (_isOnFire)
        {
            _isOnFire = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override float JumpAction()
    {
        if (this._isJump)
        {
            Debug.Log("Jump");
            _isJump = false;
            return 1;
        }
        else
        {
            return 0;
        }
    }
}



namespace Butterfly
{
    public abstract class Motion
    {
        private float _maxWingRange = 120;
        private float _minWingRange = -30;

        private GameObject _butterflyR;
        private GameObject _butterflyL;
        private Quaternion _defaultWingL;    //����������ԕۑ��p
        private Quaternion _defaultWingR;

        public Motion(GameObject butterfly)
        {
            _butterflyR = butterfly.transform.Find("right_wing").gameObject;
            _butterflyL = butterfly.transform.Find("left_wing").gameObject;
            _defaultWingL = _butterflyL.transform.localRotation;    //���̏�����Ԃ��L��
            _defaultWingR = _butterflyR.transform.localRotation;
        }

        private float CulcButterflyAnlge(float x)    // x��0����1
        {
            float angle = (_maxWingRange - _minWingRange) * (x >= 0 ? x : 0) + _minWingRange;
            return angle;
        }

        protected void Flap_base(float left, float right)    // left, right ��0����1
        {
            float angleL = CulcButterflyAnlge(left);
            _butterflyL.transform.localRotation = _defaultWingL * Quaternion.AngleAxis(angleL, Vector3.up);
            float angleR = CulcButterflyAnlge(right);
            _butterflyR.transform.localRotation = _defaultWingR * Quaternion.AngleAxis(-angleR, Vector3.up);
        }

        public abstract void Flap();
    }

    public class Motion_Oculus : Motion
    {
        private GameObject _centerEye;
        private GameObject _LeftHand;
        private GameObject _RightHand;

        public Motion_Oculus(GameObject butterfly, GameObject player) : base(butterfly)
        {
            _centerEye = player.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/CenterEyeAnchor").gameObject;
            _LeftHand = player.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerAnchor").gameObject;
            _RightHand = player.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerAnchor").gameObject;
        }

        public override void Flap()
        {
            float leftWing = (_LeftHand.transform.position - _centerEye.transform.position).normalized.y;
            float rightWing = (_RightHand.transform.position - _centerEye.transform.position).normalized.y;
            Flap_base(-leftWing, -rightWing);
        }
    }

    // OpenXR�̂��߂ɒǉ��A�R���g���[���[�Ɠ��l�Ɋ�A����ȊO�͏�Ɠ���
    public class Motion_OpenXR : Motion
    {
        private GameObject _centerEye;
        private GameObject _LeftHand;
        private GameObject _RightHand;

        public Motion_OpenXR(GameObject butterfly, GameObject player) : base(butterfly)
        {
            _centerEye = player.transform.Find("XR Origin/Camera Offset/Main Camera").gameObject;
            _LeftHand = player.transform.Find("XR Origin/Camera Offset/LeftHand Controller").gameObject;
            _RightHand = player.transform.Find("XR Origin/Camera Offset/RightHand Controller").gameObject;
        }

        public override void Flap()
        {
            float leftWing = (_LeftHand.transform.position - _centerEye.transform.position).normalized.y;
            float rightWing = (_RightHand.transform.position - _centerEye.transform.position).normalized.y;
            Flap_base(-leftWing, -rightWing);
        }
    }

    public class Motion_InputSystem : Motion
    {
        private float flapSpeed = 4.0f;     //�ő�1
        private float flapReturnSpeed = 3.0f;
        private float flapMaxRange = 0.6f;
        private float flapMinRange = 0.1f;

        private ReadInput _input;
        private bool _isFlapping = false;
        private float _angle = 0;

        public Motion_InputSystem(GameObject butterfly, ReadInput input) : base(butterfly)
        {
            _input = input;
        }

        public override void Flap()
        {
            if (_input.ReadUpButton()) { _isFlapping = true; }
            if (_isFlapping)
            {
                if (_angle < flapMaxRange) { _angle += flapSpeed * Time.deltaTime; }
                else { _isFlapping = false; }
            }
            else if (_angle > flapMinRange) { _angle -= flapReturnSpeed * Time.deltaTime; }
            Flap_base(_angle, _angle);
        }
    }
}