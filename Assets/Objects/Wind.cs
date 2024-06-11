using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : PlayerManager.PlayerInformationBase
{
    [SerializeField] private Vector3 _windPower = new Vector3(100, 0, 0);
    [SerializeField] private Vector3 _snowForceMax = new Vector3(7, 1, 1);
    [SerializeField] private Vector3 _snowForceMin = new Vector3(5, 0, 0);
    [SerializeField] private GameObject _snowObj;

    private ParticleSystem.ForceOverLifetimeModule _snow;
    private Rigidbody _parentRig;
    private bool _isCollisionStay = false;

    protected override void Awake()
    {
        base.Awake();
        _parentRig = this.transform.parent.parent.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (base.wind)
        {
            _snow = _snowObj.GetComponent<ParticleSystem>().forceOverLifetime;
            _snow.x = new ParticleSystem.MinMaxCurve(_snowForceMin.x, _snowForceMax.x);
            _snow.y = new ParticleSystem.MinMaxCurve(_snowForceMin.y, _snowForceMax.y);
            _snow.z = new ParticleSystem.MinMaxCurve(_snowForceMin.z, _snowForceMax.z);
        }
        else
        {
            this.enabled = false;
            _snowObj.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isCollisionStay)
        {
            _snow.enabled = true;
            _parentRig.AddForce(_windPower * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            _snow.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _isCollisionStay = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _isCollisionStay = false;
    }
}
