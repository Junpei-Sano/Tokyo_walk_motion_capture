using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCharacter1 : CharacterSetting
{
    private ReadInput _input;
    
    public void Awake()
    {
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        InitCharacter(rigidbody, 1.0f, 0.1f, 1.0f, 3f);

        _acceleration = 100f;
        _maxSpeed_foward = 30f;
        _maxSpeed_up = 10f;
        _maxSpeed_down = 20f;
        _changeDirectionRate = 0.5f;
        _rotateSpeed = new Vector3(30f, 10f, 30f);

        rigidbody.centerOfMass = new Vector3(0, 0.1f, 0);
    }

    private void OnGUI()
    {
        GUILayout.Label("");
        GUILayout.Label(HorizontalSpeed().ToString());
    }

    private void Start()
    {
        switch (controllerType)
        {
            case ControllerType.input_system:
                _input = new ReadInputSystem();    //アップキャスト
                break;
            case ControllerType.oculus:
                //_input = new ReadOculus();
                break;
            default:
                Debug.LogWarning("Unsupported Controller type");
                break;
        }

    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 stickR, stickL;
        stickR = _input.ReadStick_R();
        stickL = _input.ReadStick_L();
        CharacterController(stickL.y, stickR.x, stickL.x, stickR.y);
        PhysicalActions();
    }
}
