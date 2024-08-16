using UnityEngine;

/// <summary>
/// Trida reprezentuje specialni typ projektilu Pea Bullet
/// </summary>
public class SnowPeaBullet : PeaBullet
{
    /// <summary>
    /// Prefab zpomalovaciho efektu, ktery bude aplikovan na herni objekt
    /// </summary>
    private GameObject slowStatusPrefab { get; set; }

    void Awake()
    {
        slowStatusPrefab = Resources.Load<GameObject>("Prefabs/Effects/SlowEffect");
        movementSpeed = 5f;
        damage = 15;
    }

    void Update()
    {
        Move();                   
    }


    /// <summary>
    /// Metoda aplikuje zpomalovaci efekt na cilovy herni objekt
    /// </summary>
    private void ApplySlow()
    {
        StatusEffectManager slowTarget = target.GetComponent<StatusEffectManager>();

        if (slowTarget != null)
        {
            slowTarget.AddEffect(slowStatusPrefab);
        }
    }

    /// <summary>
    /// Metoda zpracuje prichozi kolizi s zombikem
    /// </summary>
    /// <param name="collision">Kolize objektu</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Zombies"))
        {
            target = collision.gameObject;

            Attack();
            ApplySlow();
            Destroy(gameObject);
        }
    }
}
