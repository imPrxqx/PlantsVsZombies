using System;
using UnityEngine;

/// <summary>
/// Abstraktni trida reprezentuje statusovy efekt
/// </summary>
public abstract class StatusEffect : MonoBehaviour
{
    /// <summary>
    /// Cilovy herni objekt, na kterym je efekt aplikovan
    /// </summary>
    public abstract GameObject target { get; set; }

    /// <summary>
    /// Delegat, ktery upozorni ostatni, ze efekt skoncil
    /// </summary>
    public abstract event Action<StatusEffect> OnEffectExpired;

    /// <summary>
    /// Metoda aplikuje statusovy efekt na cilovy herni objekt
    /// </summary>
    public abstract void ApplyEffect();

    /// <summary>
    /// Metoda obnovi statusovy efekt na cilovy herni objekt
    /// </summary>
    public abstract void ResetEffect();

    /// <summary>
    /// Metoda zastavi statusovy efekt na cilovy herni objekt
    /// </summary>
    public abstract void StopEffect();

    /// <summary>
    /// Metoda okamzite zastavi statusovy efekt na cilovy herni objekt
    /// </summary>
    public abstract void StopImmediatelyEffect();

}
