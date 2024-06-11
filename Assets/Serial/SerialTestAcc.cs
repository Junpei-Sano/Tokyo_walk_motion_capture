using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialTestAcc : MonoBehaviour
{
    //先ほど作成したクラス
    public SerialHandler serialHandler;

    private Rigidbody _rig;
    private Vector3 _accVal = new Vector3(0, 0, 1);

    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;

        _rig = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float acc_threshold = 0.04f;

        Vector3 power = _accVal;
        power.x = power.z = 0;
        power.y = _accVal.z - 1.0f;
        if (-acc_threshold < power.y && power.y < acc_threshold) { power.y = 0.0f; }
        if (-acc_threshold < power.x && power.x < acc_threshold) { power.x = 0.0f; }
        if (-acc_threshold < power.z && power.z < acc_threshold) { power.z = 0.0f; }
        power *= 100;
        //Debug.Log(power);
        _rig.AddForce(power * Time.deltaTime, ForceMode.VelocityChange);
        if (_rig.position.y < 0.0f && _rig.velocity.y < 0.0f) { _rig.velocity = Vector3.zero; }
    }

    private float DigitalValueToAcc(int value)
    {
        float voltage = value * 3.3f / 4095.0f;
        float g = (voltage - 1.65f) / 0.66f;
        return g;
    }

    private Vector3 Convert_Acc(int x, int y, int z)
    {
        Vector3 acc = new Vector3(DigitalValueToAcc(x), DigitalValueToAcc(y), DigitalValueToAcc(z));
        return acc;
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        string[] msg_split = message.Split(",");
        Vector3 acc = Convert_Acc(int.Parse(msg_split[0]), int.Parse(msg_split[1]), int.Parse(msg_split[2]));
        //Debug.Log(acc);
        this._accVal = acc;
    }
}
