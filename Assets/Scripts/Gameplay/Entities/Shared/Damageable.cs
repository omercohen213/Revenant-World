using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Damageable : MonoBehaviour
{
    public event Action<HitData> OnHit;

    [SerializeField] private HitZoneData _hitZoneData;
    
    private IDamageReceiver _receiver;


    void Awake()
    {
        _receiver = GetComponentInParent<IDamageReceiver>();
    }
    public void ReceiveHit(HitData hitData)
    {
        hitData.Damage *= _hitZoneData.DamageMultiplier;
        hitData.HitZoneData = _hitZoneData;

        _receiver?.RecieveDamage(hitData);
        OnHit?.Invoke(hitData);
        //Debug.Log("received dmg: "+ hitData.HitZoneData.ZoneName);
    }
}

