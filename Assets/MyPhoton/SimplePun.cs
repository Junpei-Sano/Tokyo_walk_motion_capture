using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

/* 
 * ���p���Fhttps://qiita.com/UpAllNight/items/43e1b24301eb6029f18b
 */

namespace PlayerManager
{
    public class SimplePun : PlayerInformationBasePun2
    {
        private GameObject _playerObj;

        // Use this for initialization
        void Start()
        {
            PhotonNetwork.NickName = base.playerName;    // NickName ���v���C���[���Őݒ�
            //���o�[�W�����ł͈����K�{�ł������APUN2�ł͕s�v�ł��B
            PhotonNetwork.ConnectUsingSettings();
        }

        void OnGUI()
        {
            //���O�C���̏�Ԃ���ʏ�ɏo��
            GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        }

        //���[���ɓ����O�ɌĂяo�����
        public override void OnConnectedToMaster()
        {
            // "room"�Ƃ������O�̃��[���ɎQ������i���[����������΍쐬���Ă���Q������j
            PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions(), TypedLobby.Default);
        }

        //���[���ɓ�����ɌĂяo�����
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("���[���ɓ���܂����B");
            PlayerManager manager = GameObject.Find("PlayerData").GetComponent<PlayerManager>();
            //�L�����N�^�[�𐶐�
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
