using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Target
{
    interface TargetPoint
    {
        int GetPoint();    // そのターゲットに当たった時の点数を返す
    }

    class Target_Script : MonoBehaviour, TargetPoint
    {
        private float _ringSize = 0.3f;
        private float _radius = 20;    // 輪の配置の最小半径

        private Vector3 worldMax = new Vector3(450, -150, 800);
        private Vector3 worldMin = new Vector3(-450, -200, -800);

        private void Randomposition()
        {
            Vector3 pos;
            Vector3 max = worldMax - worldMin;
            pos.x = Random.Range(0, max.x);
            pos.y = Random.Range(0, max.y);
            pos.z = Random.Range(0, max.z);
            pos += worldMin;
            this.transform.position = pos;

            this.name = "Target";    // "Target"の名前で
        }

        private void OnEnable()
        {
            this.transform.localScale = new Vector3(_ringSize, _ringSize, _ringSize);
            Randomposition();
            this.GetComponent<BoxCollider>().enabled = true;

            int count = 0;
            while (Physics.CheckSphere(this.transform.position, _radius) == true)
            {
                Randomposition();
                count++;
                if(count > 100)    //あまりにも繰り返し過ぎたら配置場所無し、終了
                {
                    Debug.LogWarning("Cannot place the target");
                    DestroyTarget();
                }
            }
        }

        /*
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.GetComponent<Target_Script>() == null)
            {
                //Target_Controller.point += 1;
                DestroyTarget();
            }
        }
        */

        private void DestroyTarget()
        {
            Destroy(gameObject);
        }

        public int GetPoint()
        {
            DestroyTarget();
            return 1;    // とりあえず1点
        }
    }
}