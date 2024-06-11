using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using PlayerManager;
using MyPhotonMessage;

namespace MyPhotonMessage
{
    // 実行前に必ずInitPanelを実行
    public class MessagePanelController : PlayerInformationBase, ReceiveMessage
    {
        [SerializeField] private TextMeshProUGUI _tmProGUI;

        private ReadInput _input;
        private PlayerInformationBase _playerScript;
        private MessageSender _message;
        private GameObject _panel;
        //private InputField _inputField;
        private TMP_InputField _inputField;
        private GameObject _informationPanel;

        private bool _completeInit = false;
        private bool _isActive = false;

        private readonly string _newLine = Environment.NewLine;

        public void Start()
        {
            _tmProGUI.text = "";
            _panel = this.transform.Find("MessagePanel").gameObject;
            _inputField = _panel.transform.Find("InputField (TMP)").GetComponent<TMP_InputField>();
            _message = GameObject.Find("Photon Controller").GetComponent<MessageSender>();
            _informationPanel = this.transform.parent.Find("Information").Find("InformationPanel").gameObject;
            _message.SetReceiver(this);
        }

        // メッセージ表示時に停止、表示後に開始したいスクリプトと、inputを渡す
        public void InitPanel(PlayerInformationBase playerScript, ReadInput input)
        {
            this._input = input;
            this._playerScript = playerScript;
            _completeInit = true;
        }

        public void SetMessage(string msg)
        {
            _tmProGUI.text += _newLine + msg;
        }

        private void Display()
        {
            _playerScript.enabled = false;
            _isActive = true;
            _informationPanel.SetActive(false);
            _panel.SetActive(true);
        }

        private void Close()
        {
            _isActive = false;
            _panel.SetActive(false);
            _playerScript.enabled = true;
            _informationPanel.SetActive(true);
        }

        private void Update()
        {
            if (_completeInit)
            {
                if (_input.ReadEscButton() && _isActive)
                {
                    Close();
                }
                else if (_input.ReadMessageButton() && !_isActive)
                {
                    Display();
                }
            }
        }

        public void SendMessage()
        {
            string msg = _inputField.text;
            _message.SendAllplayerMessage(msg);
            _inputField.text = "";
        }
    }
}