using System;
using UnityEngine;

/// <summary>
/// Abstraktni trida reprezentuje vlastnost kovu veci
/// </summary>
public abstract class Metal : Item, IRemoveable<Metal>
{
    /// <summary>
    /// Informace, zda byla vec odebrana
    /// </summary>
    public bool isRemoved { get; set; }

    /// <summary>
    /// Delegat, ktery upozorni ostatni, ze vec byla odebrana
    /// </summary>
    public event Action OnItemRemoved;

    /// <summary>
    /// Funkce oznaci vec jako odebranou a vrati ji
    /// </summary>
    /// <returns>Odebrana vec</returns>
    public virtual GameObject TakeItem()
    {
        isRemoved = true;
        OnItemRemoved?.Invoke();
        return gameObject;
    }
}