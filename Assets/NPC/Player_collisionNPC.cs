using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Butterfly_NPC
{
    public class Player_collisionNPC : ButterflyFormation
    {
        [SerializeField] private GameObject _bullet;

        private void OnTriggerEnter(Collider other)
        {
            CollideObj(other.gameObject);
        }

        public void Fire()
        {
            GameObject bullet = Instantiate(_bullet, this.transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().Fire(this.transform.forward, this.gameObject);
        }

        public void CollideObj(GameObject otherObj)
        {
            NPC_Butterfly newButterfly = otherObj.GetComponent<NPC_Butterfly>();
            if (newButterfly == null || newButterfly.player == this) { return; }

            Debug.LogFormat("New Butterfly Added ({0})", newButterfly.name);
            if (this.next == null)
            {
                this.next = newButterfly;
                newButterfly.prev = this;
            }
            else
            {
                ButterflyFormation tail;
                for (tail = this.next; tail.next != null; tail = tail.next) ;
                tail.next = newButterfly;
                newButterfly.prev = tail;
            }
            newButterfly.player = this;
            newButterfly.SetFollowing();
        }
    }
}
