using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamestart
{
    public class GameStart : MonoBehaviour
    {
        private Vector2 worldMax = new Vector3(200, 400);
        private Vector3 worldMin = new Vector3(-200, -400);
        private float height = -100;
        private float angle = 0;    //‰ºŒü‚«Šp“x
        private float rotateSpeed = 2f;
        private float maxRadius = 10;

        private Vector3 _center;

        private GameObject _menu;

        private Vector3 RandomPos()
        {
            Vector3 pos;
            pos.x = Random.Range(worldMin.x, worldMax.x);
            pos.z = Random.Range(worldMin.y, worldMax.y);
            pos.y = height;
            return pos;
        }

        // Start is called before the first frame update
        void Start()
        {
            Vector3 myPos;
            do
            {
                //À•Wİ’è
                myPos = RandomPos();

                //’†SÀ•Wİ’è
                _center = RandomPos();

            } while ((myPos - _center).magnitude > maxRadius);
            this.transform.position = myPos;

            _menu = GameObject.Find("Menu");
            _menu.transform.position += new Vector3(0, 0, 1);

            //‰Šú‰ñ“]İ’è
            float randAngle = Random.Range(0, 360);
            this.transform.rotation = Quaternion.AngleAxis(randAngle, Vector3.up);
            this.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.right);

            //‰ñ“]‚ÌŒü‚«İ’è
            int rotateDir = Random.Range(0, 2);
            rotateSpeed *= (rotateDir == 0) ? -1 : 1;
        }

        // Update is called once per frame
        void Update()
        {
            float speed = rotateSpeed * Time.deltaTime;
            this.transform.RotateAround(_center, Vector3.up, speed);
        }
    }
}