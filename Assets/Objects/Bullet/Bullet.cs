using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _speed = 2500;
    private float _killtime = 2.0f;  //一定時間経過後オブジェクトを消去
    private int _boundTimes = 2;

    private Rigidbody _rigidbody;
    private float _time;
    private int _boundCount = 0;

    private GameObject _player;

    public void Awake()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
        this.transform.parent = GameObject.Find("Target Controller").transform;
    }

    // Start is called before the first frame update
    public void Fire(Vector3 direction)
    {
        Vector3 force = direction.normalized * _speed;
        _rigidbody.AddForce(force);
        _time = Time.time;
    }

    public void Fire(Vector3 direction, GameObject player)
    {
        Fire(direction);
        _player = player;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _time > _killtime)    //n秒以上飛んでたら消去
        {
            Destroy();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        _player.gameObject.GetComponent<Butterfly_NPC.Player_collisionNPC>().CollideObj(collision.gameObject);
        _boundCount++;
        if (_boundCount >= _boundTimes)    //n回以上反発させない
        {
            Destroy();
        }
    }
}
