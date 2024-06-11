using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

using PlayerManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyPhotonMessage
{
    public interface ReceiveMessage
    {
        void SetMessage(string msg);
    }

    public class MessageSender : PlayerInformationBasePun2
    {
        public string messageWindow;
        private List<ReceiveMessage> _functions = new List<ReceiveMessage>();

        public void SetReceiver(ReceiveMessage rm)
        {
            _functions.Add(rm);
        }

        [PunRPC]
        private void SendRpcMessage(string msg)
        {
            for (int i = 0; i < _functions.Count; i++)
            {
                _functions[i].SetMessage(msg);
            }
        }

        // ���b�Z�[�W���M�֘A�̃��\�b�h
        public void SendAllplayerMessage()
        {
            SendAllplayerMessage(messageWindow);
        }

        public void SendAllplayerMessage(string msg)
        {
            if (base.usePUN2)
            {
                string message = "<" + base.playerName + "> " + msg;
                photonView.RPC(nameof(SendRpcMessage), RpcTarget.All, message);
            }
        }

        public void SendAllplayerLogMessage(string msg)
        {
            if (base.usePUN2)
            {
                string message = base.playerName + msg;
                photonView.RPC(nameof(SendRpcMessage), RpcTarget.All, message);
            }
        }

        public void SendAllplayerLogMessage(string msg, string playerName)
        {
            if (base.usePUN2)
            {
                string message = playerName + msg;
                photonView.RPC(nameof(SendRpcMessage), RpcTarget.All, message);
            }
        }
    }
}

#if UNITY_EDITOR
namespace MyPhotonMessage
{
    /* Player3�I�u�W�F�N�g�̃C���X�y�N�^���烁�b�Z�[�W�𑗐M���� */

    [CustomEditor(typeof(MessageSender))]
    public class PhotonMessageEditor : Editor
    {
        /// <summary>
        /// Inspector��GUI���X�V
        /// </summary>
        public override void OnInspectorGUI()
        {
            //����Inspector������\��
            base.OnInspectorGUI();

            //target��ϊ����đΏۂ��擾
            MessageSender photonMessage = target as MessageSender;

            //PublicMethod�����s����p�̃{�^��
            if (GUILayout.Button("Send Message"))
            {
                photonMessage.SendAllplayerMessage();
            }
        }
    }
}
#endif
