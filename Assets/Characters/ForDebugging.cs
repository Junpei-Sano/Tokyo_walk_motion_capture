using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForDebugging : MonoBehaviour
{
    private ReadInput _input;

    private float _speed = 40f;
    private float _rudderSpeed = 50f;
    private float _elevatorSpeed = 30f;

    public bool _chasePlayer = false;
    public GameObject _chaseObject;

    // Start is called before the first frame update
    void Start()
    {
        _input = new ReadInputSystem();

        CameraMode();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector2 rightStick = _input.ReadStick_R();
        Vector2 leftStick = _input.ReadStick_L();

        Rudder(rightStick.x);
        Elevator(rightStick.y);
        Move(leftStick);

        Chase();
        */
    }

    private void CameraMode()
    {
        GameObject target = GameObject.Find("PlayerData").GetComponent<PlayerManager.PlayerList>().GetGameObject(1);
        GameObject targetModel = target.transform.Find("Model").gameObject;
        Debug.Log(target);
        GameObject camera = this.transform.Find("SubCamera").gameObject;
        camera.transform.position = targetModel.transform.position - new Vector3(0, 0, 5);
        camera.transform.parent = targetModel.transform;
        camera.SetActive(true);
        GameObject trackingSpace = this.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").gameObject;
        trackingSpace.transform.Find("CenterEyeAnchor").gameObject.SetActive(false);
    }

    private void Rudder(float x)
    {
        float angle = x * _rudderSpeed * Time.deltaTime;
        Vector3 axis = this.transform.InverseTransformDirection(Vector3.up);
        this.transform.rotation *= Quaternion.AngleAxis(angle, axis);
    }

    private void Elevator(float y)
    {
        float angle = y * _elevatorSpeed * Time.deltaTime;
        this.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.right);
    }

    private void Move(Vector2 z)
    {
        Vector3 val = Vector3.zero;
        val += this.transform.right * z.x * _speed * Time.deltaTime;
        val += this.transform.forward * z.y * _speed * Time.deltaTime;
        this.transform.position += val;
    }

    private void Chase()
    {
        if (_chasePlayer)
        {
            Vector3 pos = _chaseObject.transform.position;
            pos.z -= 3;
            this.transform.position = pos;
            this.transform.parent = _chaseObject.transform;
        } else
        {
            this.transform.parent = null;
        }
    }
}
