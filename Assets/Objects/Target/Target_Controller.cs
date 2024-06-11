using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// åªç›ñ¢égóp

/*
namespace Target
{
    public class Target_Controller : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        private int _targetNumber = 100;
        static public int point = 0;

        // Start is called before the first frame update
        void Start()
        {
            try    // try catchÇégÇ¡ÇƒÇ›ÇΩÇ¢
            {
                GameObject target;
                for (int i = 0; i < _targetNumber; i++)
                {
                    target = Instantiate(_target);
                    target.GetComponent<Target_Script>().enabled = true;
                    target.transform.parent = this.transform;
                }
            }
            catch (Exception)
            {
                Debug.Log("Couldn't find the target object");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGUI()
        {
            GUI.Label(new Rect(Screen.width - 100, 0, 100, 30), "SCORE : " + point);
        }
    }

}
*/