using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Target
{
    public class PointCounter : MonoBehaviour
    {
        public int point { get; private set; }

        // Start is called before the first frame update
        private void Start()
        {
            // do noting
        }

        // Update is called once per frame
        private void Update()
        {
            // do nothing
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject obj = other.gameObject;
            if (obj.name == "Target")
            {
                TargetPoint target = obj.GetComponent<TargetPoint>();
                point += target.GetPoint();
                Debug.LogFormat("Point = {0}", point);
            }
        }

        public void ResetPoint()
        {
            point = 0;
        }
    }
}