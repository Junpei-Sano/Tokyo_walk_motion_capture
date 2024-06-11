using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JavascriptMediaPipe
{
    public class MediaPipeReciever : MonoBehaviour
    {
        private MediaPipe_test _script = null;

        public void SetMyObject(GameObject myobj)
        {
            _script = myobj.GetComponent<MediaPipe_test>();
        }

        // à¯êîÇÕJSONå`éÆ
        public void SetHeadValue(string xyz)
        {
            if (_script != null)
            {
                Vector3 v = JsonUtility.FromJson<Vector3>(xyz);
                _script.SetHeadValue(-v);
            }
        }

        public void SetLeftHandValue(string xyz)
        {
            if (_script != null)
            {
                Vector3 v = JsonUtility.FromJson<Vector3>(xyz);
                _script.SetLeftHandValue(-v);
            }
        }

        public void SetRightHandValue(string xyz)
        {
            if (_script != null)
            {
                Vector3 v = JsonUtility.FromJson<Vector3>(xyz);
                _script.SetRightHandValue(-v);
            }
        }
    }
}