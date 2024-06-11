using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using MyPhotonMessage;

namespace PlayerManager
{
    public class MasterClient : PlayerInformationBasePun2
    {
        private MessageSender _message;

        // Start is called before the first frame update
        private void Start()
        {
            _message = this.GetComponent<MessageSender>();
        }

        // マスタークライアント専用コメント
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)    // マスタークライアントが実行
            {
                string playerName = newPlayer.NickName;
                _message.SendAllplayerLogMessage(" joined the game", playerName);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)    // マスタークライアントが実行
            {
                string playerName = otherPlayer.NickName;
                _message.SendAllplayerLogMessage(" left the game", playerName);
            }
        }
    }
}