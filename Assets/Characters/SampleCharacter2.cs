using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCharacter2 : CharacterSetting
{
    private ReadInput _input;

    private float _defaultspeed = 7f;
    private float _deathMagnitude = 20;
    private Vector2 _scopeRange = new Vector2(0.3f, 0.3f);

    private bool _isDead = false;

    [SerializeField] GameObject _bullet;
    GameObject _stick;
    GameObject _scope;

    public void Awake()
    {
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        InitCharacter(rigidbody, 10.0f, 0.2f, 1.0f, 4f);

        _acceleration = 300f;
        _maxSpeed_foward = 15f;
        _maxSpeed_up = 5f;
        _maxSpeed_down = 20f;
        _changeDirectionRate = 0.4f;
        _rotateSpeed = new Vector3(30f, 10f, 30f);

        rigidbody.centerOfMass = new Vector3(0, 0f, 0);

        _stick = this.transform.Find("Stick").gameObject;
        _scope = this.transform.Find("Scope").gameObject;
    }

    private void Start()
    {
        switch(controllerType)
        {
            case ControllerType.input_system:
                _input = new ReadInputSystem();    //アップキャスト
                break;
            case ControllerType.oculus:
                //_input = new ReadOculus();
                break;
            case ControllerType.oculus_joystick:
                //_input = new ReadOculusJoystick();
                break;
            default:
                Debug.LogWarning("Unsupported Controller type");
                break;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("");
        GUILayout.Label(HorizontalSpeed().ToString());
    }

    protected override void AerodynamicLift()
    {
        float speedz = base.HorizontalSpeed().z;

        Vector3 force = _transform.up * -Physics.gravity.y * _rigidbody.mass * Time.deltaTime;
        if (speedz < _defaultspeed) { force *= (speedz / _defaultspeed); }
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 stickR, stickL;
        stickR = _input.ReadStick_R();
        stickL = _input.ReadStick_L();
        if (!_isDead)
        {
            CharacterController(stickL.y, stickR.x, stickL.x, stickR.y);
            StickController(stickR);
        }
        else
        {
            _scope.SetActive(false);
        }
        PhysicalActions();

        Fire();
        DeathCheck();
    }

    private void Fire()
    {
        Vector2 stick = _input.ReadAim();
        Vector3 stickPos = new Vector3(stick.x * _scopeRange.x, stick.y * _scopeRange.y, _scope.transform.localPosition.z);
        _scope.transform.localPosition = stickPos;
        if(_input.ReadFireButton())
        {
            Vector3 pos = _rigidbody.position + _rigidbody.transform.forward * 1;
            GameObject bullet = Instantiate(_bullet, pos, Quaternion.identity);
            Vector3 dir = _scope.transform.position - _rigidbody.position;
            bullet.GetComponent<Bullet>().Fire(dir);
        }
    }

    private void StickController(Vector2 val)
    {
        Vector3 dir = (new Vector3(val.x, 1, val.y)).normalized;
        Quaternion q = Quaternion.LookRotation(dir);
        _stick.transform.localRotation = q;
    }

    private void PlayerDeath(DeathPattern d)
    {
        GameObject deathobj = this.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/CenterEyeAnchor/Menu/Death").gameObject;
        deathobj.GetComponent<DeathMessage>().deathPattern = d;
        deathobj.SetActive(true);
        _isDead = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float magnitude = collision.impulse.magnitude;
        Debug.LogFormat("Collision Magnitude = {0}", magnitude);

        if (magnitude > _deathMagnitude)
        {
            PlayerDeath(DeathPattern.collision);
        }
    }

    private void DeathCheck()
    {
        if (this.transform.position.y < -250)
        {
            PlayerDeath(DeathPattern.fallVoid);
        }
    }
}
