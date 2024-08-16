using UnityEngine;

/// <summary>
/// Abstraktni trida reprezentuje herni objekt
/// </summary>
public abstract class Being : MonoBehaviour, IDamagable
{
    /// <summary>
    /// Aktualni zdravi
    /// </summary>
    public int health { get; set; }

    /// <summary>
    /// Metoda zpracuje poskozeni prijate z vnejsich zdroju
    /// </summary>
    /// <param name="damage">Celkove poskozeni</param>
    public abstract void TakeDamage(int damage);
}
