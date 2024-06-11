using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

/* 
 * 引用源：https://qiita.com/UpAllNight/items/43e1b24301eb6029f18b
 */

namespace PlayerManager
{
    public class SimplePun : PlayerInformationBasePun2
    {
        private GameObject _playerObj;

        // Use this for initialization
        void Start()
        {
            PhotonNetwork.NickName = base.playerName;    // NickName をプレイヤー名で設定
            //旧バージョンでは引数必須でしたが、PUN2では不要です。
            PhotonNetwork.ConnectUsingSettings();
        }

        void OnGUI()
        {
            //ログインの状態を画面上に出力
            GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        }

        //ルームに入室前に呼び出される
        public override void OnConnectedToMaster()
        {
            // "room"という名前のルームに参加する（ルームが無ければ作成してから参加する）
            PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions(), TypedLobby.Default);
        }

        //ルームに入室後に呼び出される
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("ルームに入りました。");
            PlayerManager manager = GameObject.Find("PlayerData").GetComponent<PlayerManager>();
            //キャラクターを生成
            _playerObj = PhotonNetwork.Instantiate(base.prefabName, base.spawnPosition, Quaternion.identity, 0);
            manager.StartGame(_playerObj);
        }

        public void DisconnectPlayer()
        {
            PhotonNetwork.Destroy(_playerObj);
            PhotonNetwork.Disconnect();
        }
    }
}
