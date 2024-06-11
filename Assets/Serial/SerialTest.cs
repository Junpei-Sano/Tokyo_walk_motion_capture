using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialTest : MonoBehaviour
{
    //��قǍ쐬�����N���X
    public SerialHandler serialHandler;

    void Start()
    {
        //�M������M�����Ƃ��ɁA���̃��b�Z�[�W�̏������s��
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

    //��M�����M��(message)�ɑ΂��鏈��
    void OnDataReceived(string message)
    {
        Debug.Log(message);
    }
}
