using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyPhotonMessage
{
    public class MessageReceiver : MonoBehaviour, MyPhotonMessage.ReceiveMessage
    {
        [SerializeField] private Text _text;    // テキストをアタッチ
        [SerializeField] private float _displayTime = 10.0f;
        [SerializeField] private const int _maxLine = 5;

        private string[] _msg = new string[_maxLine];
        private float[] _timer = new float[_maxLine];
        private int _lineNum = 0;

        private readonly string _newLine = Environment.NewLine;

        private void Start()
        {
            GameObject photonController = GameObject.Find("Photon Controller").gameObject;
            photonController.GetComponent<MessageSender>().SetReceiver(this);
            for (int i = 0; i < _maxLine; i++)
            {
                _msg[i] = "";
                _timer[i] = Time.time;
            }
        }

        private void SetText()
        {
            string message = "";
            int i = _lineNum;
            for (int j = 0; j < _maxLine; j++)
            {
                message += _newLine + _msg[i];
                if (++i >= _maxLine) { i = 0; }
            }
            _text.text = message;
        }

        public void SetMessage(string msg)
        {
            _msg[_lineNum] = msg;
            _timer[_lineNum] = Time.time;
            _lineNum++;
            if (_lineNum >= _maxLine) { _lineNum = 0; }
            SetText();
        }

        private void Update()
        {
            for (int i = 0; i < _maxLine; i++)
            {
                if (Time.time - _timer[i] > _displayTime)
                {
                    _msg[i] = "";
                    SetText();
                }
            }
        }
    }
}
