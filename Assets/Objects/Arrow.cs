using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    /* targetTransform = myTransformなら矢印をオフにする仕様 */

    [SerializeField] Text _targetText;
    private GameObject _arrow;    // 矢印のオブジェクト
    private GameObject _myObj;
    private GameObject _targetObj;
    private PlayerManager.PlayerList _players;

    // Start is called before the first frame update
    private void Start()
    {
        _arrow = this.transform.GetChild(0).gameObject;    // 矢印の実態（一つ下の子オブジェクト）
        _myObj = this.transform.parent.parent.parent.gameObject;    // 親の親の親がPlayer
        _targetObj = _myObj;    // デフォルトは矢印非表示
        _players = GameObject.Find("PlayerData").GetComponent<PlayerManager.PlayerList>();
    }

    public void TargetSelection()
    {
        int targetCount = _players.PlayerCount();
        int playerNum = _players.GetPlayerNumber(_targetObj);
        if (playerNum + 1 >= targetCount)
        {
            _targetObj = _players.GetGameObject(0);
        }
        else
        {
            _targetObj = _players.GetGameObject(playerNum + 1);
        }
        _targetText.text = "Target: " + _targetObj.name;
        Debug.LogFormat("New Target: ({0}) {1}", playerNum, _targetObj.name);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_targetObj == null) { TargetSelection(); }
        else if (_targetObj == _myObj)    // ターゲットが自分のとき矢印を停止
        {
            _arrow.SetActive(false);
            _targetText.enabled = false;
        }
        else
        {
            _arrow.SetActive(true);
            _targetText.enabled = true;
            this.transform.LookAt(_targetObj.transform);    // ターゲットの方を向く
        }
    }
}
