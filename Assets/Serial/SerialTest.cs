using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialTest : MonoBehaviour
{
    //先ほど作成したクラス
    public SerialHandler serialHandler;

    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;

        StartCoroutine("timer1sec");
    }

    void Update()
    {
        // do nothing
    }

    IEnumerator timer1sec()
    {
        while (true)
        {
            serialHandler.Write("Hello");
            yield return new WaitForSeconds(1);
        }
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        Debug.Log(message);
    }
}
