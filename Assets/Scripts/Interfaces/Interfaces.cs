using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kontrakt, ze herni objekt nemuze byt pouzit jinak nez k vylepseni jineho herniho objektu
/// </summary>
/// <typeparam name="T">Zdrojovy herni objekt</typeparam>
public interface IUpgradeable<T> where T : class { }

/// <summary>
/// Kontrakt, ze vec muze byt odstranen urcitym zpusobem
/// </summary>
/// <typeparam name="T">Zpusob odstraneni</typeparam>
public interface IRemoveable<T>
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
    public GameObject TakeItem();
}

/// <summary>
/// Kontrakt, ze herni objekt muze byt prodan
/// </summary>
public interface ISellable
{
    public void Sell();
}

/// <summary>
/// Kontrakt, ze herni objekt muze byt obnoven do puvodniho stavu
/// </summary>
public interface IResetable
{
    /// <summary>
    /// Metoda nastavi herni objekt do puvodniho stavu
    /// </summary>
    public void ResetObject();
}

/// <summary>
/// Kontrakt, ze herni objekt muze byt poskozen z vnejsich zdroju
/// </summary>
public interface IDamagable
{
    /// <summary>
    /// Aktualni zdravi
    /// </summary>
    public int health { get; set; }

    /// <summary>
    /// Metoda zpracuje poskozeni prijate z vnejsich zdroju
    /// </summary>
    /// <param name="damage">Celkove poskozeni</param>
    public void TakeDamage(int damage);
}

/// <summary>
/// Kontrakt, ze herni objekt muze utocit
/// </summary>
public interface IDamager {

    /// <summary>
    /// Celkove poskozeni
    /// </summary>
    public int damage { get; set; }

    /// <summary>
    /// Metoda zpusobi celkove poskozeni
    /// </summary>
    public void Attack();
}

/// <summary>
/// Kontrakt, ze herni objekt muze pohybovat
/// </summary>
public interface IMoveable
{
    /// <summary>
    /// Aktualni rychlost
    /// </summary>
    public float movementSpeed { get; set; }

    /// <summary>
    /// Metoda zajistuje pohyb
    /// </summary>
    public void Move();
}

/// <summary>
/// Kontrakt, ze herni objekt ma seznam statusovych efektu, na ktere je immuni
/// </summary>
public interface IImmunityEffects
{
    /// <summary>
    /// Seznam statusovych efektu, na ktere je immuni
    /// </summary>
    public List<Type> immunityEffectsList { get; }
}

/// <summary>
/// Kontrakt, ze statusovy efekt ma seznam pozadavku, ktere musi mit herni objekt
/// </summary>
public interface ICompatibilityEffects
{
    /// <summary>
    /// Seznam efektu, se kterymi nemuze soucasne existovat - Povoleno: chovani hernich objektu
    /// </summary>
    public List<Type> compatibilityEffectsList { get; }
}

/// <summary>
/// Kontrakt, ze statusovy efekt ma seznam statusovych efekt, se kterymi nemuze existovat soucasne
/// </summary>
public interface IRestrictedEffects
{
    /// <summary>
    /// Seznam efektu, se kterymi nemuze soucasne existovat - Povoleno: efekty z tridy Status Effect
    /// </summary>
    public List<Type> restrictedEffectsList { get; }
}
