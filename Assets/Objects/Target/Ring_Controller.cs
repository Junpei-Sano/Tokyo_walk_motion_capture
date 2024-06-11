using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Target
{
    class Ring_Controller : MonoBehaviour
    {
        [SerializeField] private GameObject _ringObject;
        private int _targetNumber = 50;

        // Start is called before the first frame update
        void Start()
        {
            try    // try catch‚ðŽg‚Á‚Ä‚Ý‚½‚¢
            {
                GameObject target;
                for (int i = 0; i < _targetNumber; i++)
                {
                    target = Instantiate(_ringObject);
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
            // do nothing
        }
    }
}