using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerType
{
    input_system,
    oculus,
    oculus_joystick,
    oculus_wing,    // SampleCharacter3に記載
    input_system_wing,
    open_xr_wing,
    udp_mediapipe
}

public abstract class ReadInput
{
    public virtual Vector2 ReadStick_R() { return Vector2.zero; }
    public virtual Vector2 ReadStick_L() { return Vector2.zero; }
    public virtual Vector2 ReadAim() { return Vector2.zero; }
    public virtual bool ReadQuitButton() { return false; }
    public virtual bool ReadFireButton() { return false; }
    public virtual bool ReadUpButton() { return false; }
    public virtual bool ReadMessageButton() { return false; }
    public virtual bool ReadEscButton() { return false; }
    public virtual float JumpAction() { return 0.0f; }
}

public class ReadInputSystem : ReadInput    // input system（キーボード）使用
{
    private Test_Actions _input;

    public ReadInputSystem()
    {
        _input = new Test_Actions();
        _input.Enable();
        Debug.Log("Input: Input System");
    }

    public override Vector2 ReadStick_R()
    {
        return _input.User.Rotate.ReadValue<Vector2>();
    }

    public override Vector2 ReadStick_L()
    {
        return _input.User.Move.ReadValue<Vector2>();
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
}
/*
public class ReadOculus : ReadInput    // Oculusのコントローラを使用
{
    public ReadOculus()
    {
        Debug.Log("Input: Oculus");
    }

    public override Vector2 ReadStick_R()
    {
        return OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
    }

    public override Vector2 ReadStick_L()
    {
        return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
    }

    public override bool ReadQuitButton()
    {
        return OVRInput.GetDown(OVRInput.Button.Start);
    }

    public override bool ReadFireButton()
    {
        return OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
    }
}

public class ReadOculusJoystick : ReadInput
{
    private GameObject _rightHandAnchor;
    private Transform _playerObj;

    public ReadOculusJoystick()
    {
        _rightHandAnchor = GameObject.Find("RightHandAnchor");
        _playerObj = _rightHandAnchor.transform.parent.parent.parent.parent;
        Debug.Log("Input: Oculus(Joystick)");
    }

    private bool _isJoystickRotate = false;
    private Vector3 _joystickStandard = Vector3.zero;
    public override Vector2 ReadStick_R()
    {
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            if (!_isJoystickRotate)
            {
                _isJoystickRotate = true;
                _joystickStandard = _rightHandAnchor.transform.up;
            }
            Vector3 val3 = _rightHandAnchor.transform.up - _joystickStandard;
            Vector3 val = _playerObj.InverseTransformDirection(val3);
            Vector2 val2 = new Vector2(val.x, val.z);
            return val2;
        }
        else
        {
            _isJoystickRotate = false;
            return Vector2.zero;
        }
    }

    public override Vector2 ReadStick_L()
    {
        return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
    }

    public override Vector2 ReadAim()
    {
        return OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
    }

    public override bool ReadQuitButton()
    {
        return OVRInput.GetDown(OVRInput.Button.Start);
    }

    public override bool ReadFireButton()
    {
        return OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
    }
}
*/