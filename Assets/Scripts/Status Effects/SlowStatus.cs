using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trida reprezentuje zpomalovaci efekt
/// </summary>
public class SlowStatus : StatusEffect, IRestrictedEffects, ICompatibilityEffects
{

    /// <summary>
    /// Cilovy herni objekt, na kterym je efekt aplikovan
    /// </summary>
    public override GameObject target { get; set; }

    /// <summary>
    /// Mira aplikace zpomaleni na cilovy herni objekt 
    /// </summary>
    private float slowAmount { get; set; }

    /// <summary>
    /// Doba trvani efektu
    /// </summary>
    private float duration { get; set; }

    /// <summary>
    /// Seznam efektu, se kterymi nemuze soucasne existovat - Povoleno: efekty z tridy Status Effect
    /// </summary>
    public List<Type> restrictedEffectsList => new List<Type>() { typeof(FireStatus) };

    /// <summary>
    /// Seznam efektu, se kterymi nemuze soucasne existovat - Povoleno: chovani hernich objektu
    /// </summary>
    public List<Type> compatibilityEffectsList => new List<Type>() { typeof(IMoveable) };

    /// <summary>
    /// Delegat, ktery upozorni ostatni, ze efekt skoncil
    /// </summary>
    public override event Action<StatusEffect> OnEffectExpired;

    void Awake()
    {
        slowAmount = 0.75f;
        duration = 5f;
    }

    /// <summary>
    /// Metoda aplikuje zpomalovaci efekt na cilovy herni objekt
    /// </summary>
    public override void ApplyEffect()
    {
        GetComponent<ParticleSystem>().Play();
        target.GetComponent<IMoveable>().movementSpeed *= slowAmount;
        StartCoroutine(LifeTimeEffect());
    }

    /// <summary>
    /// Metoda zastavi zpomalovaci efekt na cilovy herni objekt
    /// </summary>
    public override void StopEffect()
    {
        target.GetComponent<IMoveable>().movementSpeed /= slowAmount;
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
    /// Metoda obnovi zpomalovaci efekt na cilovy herni objekt
    /// </summary>
    public override void ResetEffect()
    {
        StopAllCoroutines();
        StartCoroutine(LifeTimeEffect());
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
