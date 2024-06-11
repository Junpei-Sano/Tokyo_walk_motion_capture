using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManager
{
    public class PlayerList : MonoBehaviour, PlayerUpdate
    {
        private List<GameObject> players = new List<GameObject>();

        private PlayersController _controller;

        private void Awake()
        {
            _controller = GameObject.Find("Photon Controller").GetComponent<PlayersController>();
            _controller.Set(this);
        }

        public void JoinedGame(GameObject player)
        {
            players.Add(player);
        }

        public void LeftGame(GameObject player)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == player)
                {
                    players.RemoveAt(i);
                    return;
                }
            }
        }

        public int PlayerCount()
        {
            return players.Count;
        }

        public int GetPlayerNumber(GameObject player)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == player) { return i; }
            }
            Debug.LogWarning("Cannot get the player number.");
            return -1;
        }

        public GameObject GetGameObject(int n)
        {
            if (n >= players.Count)
            {
                Debug.LogWarning("Incorrect player number.");
                return null;
            }
            return players[n];
        }
    }
}
