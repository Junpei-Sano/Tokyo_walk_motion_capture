using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyPhotonPrefabPool : MonoBehaviour, IPunPrefabPool
{
    public List<GameObject> PrefabList;

    private PlayerManager.PlayersController _controller;

    public void Awake()
    {
        _controller = this.GetComponent<PlayerManager.PlayersController>();
    }

    public void Start()
    {
        PhotonNetwork.PrefabPool = this;
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        foreach (var s in PrefabList)
        {
            if (s.name == prefabId)
            {
                GameObject go = Instantiate(s, position, rotation);
                go.SetActive(false);
                _controller.JoinedPlayer(go);    // ì¸é∫éûÇÃèàóù
                return go;
            }
        }
        Debug.LogWarning("Cannot Find the prefab.");
        return null;
    }

    public void Destroy(GameObject gameObject)
    {
        _controller.LeftPlayer(gameObject);
        GameObject.Destroy(gameObject);
    }
}
