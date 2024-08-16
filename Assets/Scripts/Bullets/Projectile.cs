using UnityEngine;

/// <summary>
/// Abstraktni trida reprezentuje projektil
/// </summary>
public abstract class Projectile : MonoBehaviour, IDamager, IMoveable
{
    /// <summary>
    /// Cilovy herni objekt, ktery projektil poskozuje
    /// </summary>
    public GameObject target { get; set; }

    /// <summary>
    /// Aktualni rychlost projektilu
    /// </summary>
    public float movementSpeed { get; set; }

    /// <summary>
    /// Celkove poskozneni projektilu
    /// </summary>
    public int damage { get; set; }

    /// <summary>
    /// Metoda zpusobi celkove poskozeni
    /// </summary>
    public abstract void Attack();

    /// <summary>
    /// Metoda zajistuje pohyb
    /// </summary>
    public abstract void Move();
}
