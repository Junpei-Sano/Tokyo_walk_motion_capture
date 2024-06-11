using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManager
{
    public interface PlayerUpdate
    {
        void JoinedGame(GameObject player);
        void LeftGame(GameObject player);
    }

    public class PlayersController : MonoBehaviour
    {
        private List<PlayerUpdate> _functions = new List<PlayerUpdate>();

        // Start is called before the first frame update
        private void Start() {}
        // Update is called once per frame
        private void Update() {}

        public void Set(PlayerUpdate func)
        {
            this._functions.Add(func);
        }

        public void JoinedPlayer(GameObject player)
        {
            for (int i = 0; i < _functions.Count; i++)
            {
                _functions[i].JoinedGame(player);
            }
        }

        public void LeftPlayer(GameObject player)
        {
            for (int i = 0; i < _functions.Count; i++)
            {
                _functions[i].LeftGame(player);
            }
        }
    }
}