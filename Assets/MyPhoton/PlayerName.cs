using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PlayerManager
{
    public class PlayerName : PlayerInformationBasePun2
    {
        // Start is called before the first frame update
        private void Start()
        {
            if (base.usePUN2)
            {
                string playerName = this.GetComponent<PhotonView>().Owner.NickName;
                this.gameObject.name = playerName;
            }
        }
    }
}
