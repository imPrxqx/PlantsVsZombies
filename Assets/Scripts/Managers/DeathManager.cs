using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Trida ma starost smrt hrace
/// </summary>
public class DeathManager : MonoBehaviour
{
    /// <summary>
    /// Indikator, zda skoncila hra
    /// </summary>
    public bool end { get; set; }

    /// <summary>
    /// Prefab UI prvku, ktery se ukaze po prohre hrace
    /// </summary>
    public GameObject lost { get; set; }

    /// <summary>
    /// Doba zobrazeni UI prvku "You Lost"
    /// </summary>
    public float destroyDelay { get; set; }

    /// <summary>
    /// Delegat, ktery upozorni ostatni, ze karta byla vybrana do ruky
    /// </summary>
    public event Action OnPlayerDeath;

    void Awake()
    {
        lost = Resources.Load<GameObject>("Prefabs/UI/Lost");
        end = false;
        destroyDelay = 2f;
    }


    /// <summary>
    /// Metoda kontroluje, zda na konci nedosel zombik
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Zombies") && end == false)
        {
            end = true;
            GameObject lostInstance = Instantiate(lost);
            StartCoroutine(DestroyAfterDelay(lostInstance));
        }
    }

    /// <summary>
    /// Korutina vyckava na konec animace konce hry
    /// </summary>
    /// <param name="obj">GameObject ke zniceni</param>
    private IEnumerator DestroyAfterDelay(GameObject lost)
    {
        yield return new WaitForSeconds(destroyDelay);
        OnPlayerDeath.Invoke();
        Destroy(lost);
    }
}
