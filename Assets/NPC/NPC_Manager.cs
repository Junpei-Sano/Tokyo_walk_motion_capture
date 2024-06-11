using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Butterfly_NPC
{
    public class NPC_Manager : MonoBehaviour
    {
        private readonly string _prefabName = "Butterfly2";
        private readonly int _NumNPCButterfly = 100;
        private readonly float _butterflyScale = 50;

        // Start is called before the first frame update
        void Start()
        {
            GameObject photonController = GameObject.Find("Photon Controller").gameObject;
            MyPhotonPrefabPool prefabPool = photonController.GetComponent<MyPhotonPrefabPool>();
            for (int i = 0; i < _NumNPCButterfly; i++)
            {
                GameObject go = prefabPool.Instantiate(_prefabName, Vector3.zero, Quaternion.identity);
                go.transform.parent = this.transform;
                // ‚È‚º‚©ƒXƒP[ƒ‹‚ª0.01‚É‚È‚é‚Ì‚ðC³
                go.transform.localScale = Vector3.one * _butterflyScale;
                go.name = "ButterflyNPC" + i.ToString();
                go.SetActive(true);
            }
        }
    }
}
