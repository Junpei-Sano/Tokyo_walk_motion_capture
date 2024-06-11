using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDP_test_server : MonoBehaviour
{
    private int _LOCALPORT = 22222;
    private static UdpClient _udp;
    private Thread _thread;

    void Start()
    {
        _udp = new UdpClient(_LOCALPORT);
        _udp.Client.ReceiveTimeout = 1000;
        _thread = new Thread(new ThreadStart(ThreadMethod));
        _thread.Start();
    }

    void Update()
    {
    }

    void OnApplicationQuit()
    {
        _thread.Abort();
    }

    private static void ThreadMethod()
    {
        while (true)
        {
            try
            {
                IPEndPoint remoteEP = null;
                byte[] data = _udp.Receive(ref remoteEP);
                string text = Encoding.ASCII.GetString(data);
                Debug.Log(text);
            }
            catch (System.Exception e)
            {
                // do nothing
            }
        }
    }
}
