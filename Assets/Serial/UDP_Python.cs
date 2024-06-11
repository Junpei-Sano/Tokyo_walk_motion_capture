using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Drawing;
using System;

public class UDP_Python : MonoBehaviour
{
    // UDP関連
    private readonly int _LOCALPORT = 22222;
    private readonly string _REMOTEHOST = "localhost";
    private readonly int _REMOTEPORT = 11111;
    private UdpClient _udp;
    private Thread _thread;

    void Start()
    {
        // UDPの設定
        _udp = new UdpClient(_LOCALPORT);
        //_udp.Client.ReceiveTimeout = 1000;
        _thread = new Thread(new ThreadStart(ThreadMethod));
        _thread.Start();
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
                //string text = Encoding.UTF8.GetString(data);
                //Debug.Log(text);
                int dataInt = BitConverter.ToInt32(data, 0);
                Debug.LogFormat("Receive: {0}", dataInt);
            }
            catch (System.Exception e)
            {
                // do nothing
            }
        }
    }

    private void send(byte[] bytes)
    {
        _udp.Send(bytes, bytes.Length, _REMOTEHOST, _REMOTEPORT);
    }
    private void send(int intVal)
    {
        // intは4バイト
        byte[] bytes = BitConverter.GetBytes(intVal);
        _udp.Send(bytes, bytes.Length, _REMOTEHOST, _REMOTEPORT);
    }
    private void send(float floatVal)
    {
        // floatは4バイト
        byte[] bytes = BitConverter.GetBytes(floatVal);
        _udp.Send(bytes, bytes.Length, _REMOTEHOST, _REMOTEPORT);
    }
    private void send(float[] floatValAry)
    {
        byte[] bytes = new byte[floatValAry.Length * 4];
        int idx = 0;
        foreach (float val in floatValAry)
        {
            byte[] bFval = BitConverter.GetBytes(val);
            for (int i = 0; i < 4; i++)
            {
                bytes[idx++] = bFval[i];
            }
        }
        _udp.Send(bytes, bytes.Length, _REMOTEHOST, _REMOTEPORT);
    }


    private int _timer = 0;
    void Update()
    {
        if (Time.time - _timer > 1)
        {
            //this.send((int)Time.time * 1000);
            _timer = (int)Time.time;
            Debug.Log("send");
            float[] sendval = { 1.1f, 2.2f };
            this.send(sendval);
        }
    }
}
