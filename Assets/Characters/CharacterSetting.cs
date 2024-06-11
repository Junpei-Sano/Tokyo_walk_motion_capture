using UnityEngine;

public class CharacterSetting : MonoBehaviour
{
    public ControllerType controllerType;

    protected float _acceleration;
    protected float _maxSpeed_foward;
    protected float _maxSpeed_up;
    protected float _maxSpeed_down;
    [Range(0.0f, 1.0f)] protected float _changeDirectionRate;
    protected Vector3 _rotateSpeed;

    protected Rigidbody _rigidbody;
    protected Transform _transform;

    public void InitCharacter(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
        _transform = _rigidbody.transform;
    }

    public void InitCharacter(Rigidbody rigidbody, float mass, float drag, float AnglarDrag)
    {
        InitCharacter(rigidbody);

        _rigidbody.mass = mass;
        _rigidbody.drag = drag;
        _rigidbody.angularDrag = AnglarDrag;
    }

    public void InitCharacter(Rigidbody rigidbody, float mass, float drag, float AnglarDrag, float gravity)
    {
        InitCharacter(rigidbody, mass, drag, AnglarDrag);

        Vector3 gravity_3 = new Vector3(0, -gravity, 0);
        Physics.gravity = gravity_3;
    }

    protected Vector3 HorizontalSpeed()  //ローカル座標を水平に直した座標のスピードを返す
    {
        Vector3 local = _transform.InverseTransformDirection(_rigidbody.velocity);
        Vector3 horizon = Vector3.ProjectOnPlane(local, _transform.InverseTransformDirection(Vector3.up));
        Vector3 val;
        val.x = horizon.x;
        val.y = _rigidbody.velocity.y;
        val.z = horizon.z;
        return val;
    }

    private Vector3 GetHorizontalRight()
    {
        return Vector3.ProjectOnPlane(_transform.right, Vector3.up).normalized;
    }

    private Vector3 GetHorizontalForward()
    {
        return Vector3.ProjectOnPlane(_transform.forward, Vector3.up).normalized;
    }

    private Vector3 GetHorizontalUp()
    {
        return Vector3.up;
    }

    private void Throttle(float y)
    {
        Vector3 value = y * _transform.forward * _acceleration * Time.deltaTime;
        _rigidbody.AddForce(value, ForceMode.Acceleration);
    }

    private void Rudder(float x)
    {
        float val = x * _rotateSpeed.y * Time.deltaTime;
        Quaternion q = Quaternion.AngleAxis(val, Vector3.up);
        _rigidbody.rotation *= q;
    }

    private void Elevator(float y)
    {
        float val = y * _rotateSpeed.x * Time.deltaTime;
        Quaternion q = Quaternion.AngleAxis(val, Vector3.right);
        _rigidbody.rotation *= q;
    }

    private void Aileron(float x)
    {
        float val = -x * _rotateSpeed.z * Time.deltaTime;
        Quaternion q = Quaternion.AngleAxis(val, Vector3.forward);
        _rigidbody.rotation *= q;
    }

    private void Resistance()
    {
        Vector3 horizontalspeed = HorizontalSpeed();
        Vector3 force = Vector3.zero;
        /* 廃止、RigidbodyのDragで代用
        force += GetHorizontalForward() * _resistanceForce.z * (horizontalspeed.z > 0 ? -1 : 1);
        force += GetHorizontalRight() * _resistanceForce.x * (horizontalspeed.x > 0 ? -1 : 1);
        force += GetHorizontalUp() * _resistanceForce.y * (horizontalspeed.y > 0 ? -1 : 1);
        */

        if (Mathf.Abs(horizontalspeed.z) > _maxSpeed_foward)  //物体正面方向のスピードチェック
        {
            force += GetHorizontalForward() * _acceleration * (horizontalspeed.z > 0 ? -1 : 1);
        }
        if ((horizontalspeed.y < -_maxSpeed_down) || (_maxSpeed_up < horizontalspeed.y))
        {
            force += GetHorizontalUp() * _acceleration * (horizontalspeed.y > 0 ? -1 : 1);
        }
        _rigidbody.AddForce(force * Time.deltaTime, ForceMode.Acceleration);
    }

    private void ChangeDirection()
    {
        Vector3 velocity, val;
        velocity = _rigidbody.velocity * _changeDirectionRate;
        float horizontalV = HorizontalSpeed().z;
        if (Mathf.Abs(horizontalV) > 0.5)  //速度が小さいときは進行方向変換を前方に
        {
            val = (horizontalV >= 0 ? 1 : -1) * transform.forward * velocity.magnitude - velocity;
        }
        else
        {
            val = transform.forward * velocity.magnitude - velocity;
        }
        _rigidbody.AddForce(val, ForceMode.Acceleration);

    }

    protected virtual void AerodynamicLift()  //揚力
    {
        Vector3 force = _transform.up * -Physics.gravity.y;
        _rigidbody.AddForce(_rigidbody.mass * force * Time.deltaTime, ForceMode.Impulse);
    }

    protected void PhysicalActions()
    {
        Resistance();
        ChangeDirection();
        AerodynamicLift();
    }

    protected void CharacterController(float throttle, float aileron, float rudder, float elevator)
    {
        Throttle(throttle);
        Aileron(aileron);
        Rudder(rudder);
        Elevator(elevator);
    }
}
