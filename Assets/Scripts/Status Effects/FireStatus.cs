using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trida reprezentuje ohnivy efekt
/// </summary>
public class FireStatus : StatusEffect, IRestrictedEffects, ICompatibilityEffects
{

    /// <summary>
    /// Cilovy herni objekt, na kterym je efekt aplikovan
    /// </summary>
    public override GameObject target { get; set; }

    /// <summary>
    /// Interval mezi jednotlivymi poskozeni
    /// </summary>
    private float burnInterval { get; set; }

    /// <summary>
    /// Celkove poskozeni v kazdem intervalu
    /// </summary>
    private int damage { get; set; }

    /// <summary>
    /// Doba trvani efektu
    /// </summary>
    private float duration { get; set; }

    /// <summary>
    /// Seznam efektu, se kterymi nemuze soucasne existovat - Povoleno: efekty z tridy Status Effect
    /// </summary>
    public List<Type> restrictedEffectsList => new List<Type>() { typeof(SlowStatus) };

    /// <summary>
    /// Seznam efektu, se kterymi nemuze soucasne existovat - Povoleno: chovani hernich objektu
    /// </summary>
    public List<Type> compatibilityEffectsList => new List<Type>() { typeof(IDamagable) };

    /// <summary>
    /// Delegat, ktery upozorni ostatni, ze efekt skoncil
    /// </summary>
    public override event Action<StatusEffect> OnEffectExpired;


    void Awake()
    {
        burnInterval = 1f;
        duration = 5f;
        damage = 1;
    }

    /// <summary>
    /// Metoda aplikuje ohnivy efekt na cilovy herni objekt
    /// </summary>
    public override void ApplyEffect()
    {
        GetComponent<ParticleSystem>().Play();
        StartCoroutine(LifeTimeEffect());
        StartCoroutine(Burn());
    }

    /// <summary>
    /// Metoda zastavi ohnivy efekt na cilovy herni objekt
    /// </summary>
    public override void StopEffect()
    {
        OnEffectExpired?.Invoke(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Metoda okamzite zastavi ohnivy efekt na cilovy herni objekt
    /// </summary>
    public override void StopImmediatelyEffect()
    {
        StopAllCoroutines();
        StopEffect();
    }

    /// <summary>
    /// Metoda obnovi ohnivy efekt na cilovy herni objekt
    /// </summary>
    public override void ResetEffect()
    {
        StopAllCoroutines();
        StartCoroutine(LifeTimeEffect());
        StartCoroutine(Burn());
    }

    /// <summary>
    /// Korutina vyckava urcitou dobu a pote udeli poskozeni na cilovy herni objekt
    /// </summary>
    private IEnumerator Burn()
    {
        while (true)
        {
            target.GetComponent<IDamagable>().TakeDamage(damage);
            yield return new WaitForSeconds(burnInterval);
        }
    }

    /// <summary>
    /// Korutina vyckava na skonceni ohniveho efektu a pote zrusi efekt na cilovy herni objekt 
    /// </summary>
    private IEnumerator LifeTimeEffect()
    {
        yield return new WaitForSeconds(duration);
        StopEffect();
    }
}
