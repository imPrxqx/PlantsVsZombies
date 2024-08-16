using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trida ma na starost spravu statusovych efektu na herni objekt
/// </summary>
public class StatusEffectManager : MonoBehaviour
{

    /// <summary>
    /// Seznam aktivnich statusovych efektu
    /// </summary>
    public List<StatusEffect> activeEffects { get; set; }


    void Awake()
    {
        activeEffects = new List<StatusEffect>();
    }

    /// <summary>
    /// Metoda prida novy statusovy efekt na herni objekt
    /// </summary>
    public void AddEffect(GameObject prefabEffect)
    {
        if (IsEffectCompatible(prefabEffect) == false)
        {
            return;
        }

        if (IsGameObjectImmune(prefabEffect) == true)
        {
            return;
        }

        RemoveConflictingEffects(prefabEffect);

        GameObject effectObject = Instantiate(prefabEffect, transform);
        StatusEffect effect = effectObject.GetComponent<StatusEffect>();
        effect.target = gameObject;
        effect.OnEffectExpired += RemoveEffect;
        effect.ApplyEffect();
        activeEffects.Add(effect);
    }

    /// <summary>
    /// Metoda zastavi vsechny aktivni statusovy efekty na herni objekt
    /// </summary>
    public void ClearAllEffects()
    {
        List<StatusEffect> needToRemove = new List<StatusEffect>();

        foreach (StatusEffect activeEffect in activeEffects) 
        {
            needToRemove.Add(activeEffect);
        }

        foreach (StatusEffect removeEffect in needToRemove)
        {
            removeEffect.StopImmediatelyEffect();
        }
    }

    /// <summary>
    /// Metoda odstrani vyprseny statusovy efekt ze seznamu aktivnich statusovych efektu
    /// </summary>
    /// <param name="expiredStatus">Vyprseny statusovy efekt</param>
    public void RemoveEffect(StatusEffect expiredStatus)
    {
        activeEffects.Remove(expiredStatus);
    }

    /// <summary>
    /// Funkce vrati, zda je statusovy efekt kompatibilni s hernim objektem
    /// </summary>
    /// <param name="prefabEffect">Statusovy efekt</param>
    /// <returns>Je kompatibilni s hernim objektem</returns>
    private bool IsEffectCompatible(GameObject prefabEffect)
    {
        ICompatibilityEffects compatibilityEffects = prefabEffect.GetComponent<ICompatibilityEffects>();

        if (compatibilityEffects == null || compatibilityEffects.compatibilityEffectsList == null)
        {
            return true;
        }

        foreach (Type typeEffect in compatibilityEffects.compatibilityEffectsList)
        {
            if (gameObject.GetComponent(typeEffect) == null)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Metoda vrati, zda je statusovy efekt aplikovatelny na herni objekt
    /// </summary>
    /// <param name="prefabEffect"></param>
    /// <returns>Je aplikovatelny na herni objekt</returns>
    private bool IsGameObjectImmune(GameObject prefabEffect)
    {
        IImmunityEffects immunityEffects = gameObject.GetComponent<IImmunityEffects>();

        if (immunityEffects == null || immunityEffects.immunityEffectsList == null)
        {
            return false;
        }

        foreach (Type typeEffect in immunityEffects.immunityEffectsList)
        {
            if (prefabEffect.GetComponent(typeEffect) != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Metoda odstrani vsechny aktivni efekty ze seznamu aktivnich efektu, kteri nemuzou soucasne existovat s nove pridanym statusovym efektem
    /// </summary>
    /// <param name="prefabEffect"></param>
    private void RemoveConflictingEffects(GameObject prefabEffect)
    {
        List<StatusEffect> needToRemove = new List<StatusEffect>();

        foreach (StatusEffect activeEffect in activeEffects)
        {
            if (activeEffect.GetType() == prefabEffect.GetComponent<StatusEffect>().GetType())
            {
                activeEffect.ResetEffect();
                return;
            }

            IRestrictedEffects restrictedEffects = prefabEffect.GetComponent<IRestrictedEffects>();

            if (restrictedEffects != null && restrictedEffects.restrictedEffectsList != null &&
                restrictedEffects.restrictedEffectsList.Contains(activeEffect.GetType()))
            {
                needToRemove.Add(activeEffect);
            }
        }

        foreach (StatusEffect removeEffect in needToRemove)
        {
            removeEffect.StopImmediatelyEffect();
        }
    }
}
