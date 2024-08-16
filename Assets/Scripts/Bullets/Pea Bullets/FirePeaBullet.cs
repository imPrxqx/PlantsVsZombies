using UnityEngine;

/// <summary>
/// Trida reprezentuje specialni typ projektilu Pea Bullet
/// </summary>
public class FirePeaBullet : PeaBullet
{
    /// <summary>
    /// Prefab ohniveho efektu, ktery bude aplikovan na herni objekt
    /// </summary>
    private GameObject fireStatusPrefab { get; set; }

    void Awake()
    {
        fireStatusPrefab = Resources.Load<GameObject>("Prefabs/Effects/FireEffect");
        movementSpeed = 5f;
        damage = 20;
    }

    void Update()
    {
        Move();
    }

    /// <summary>
    /// Metoda aplikuje ohnivy efekt na cilovy herni objekt
    /// </summary>
    private void ApplyBurn()
    {
        StatusEffectManager burnTarget = target.GetComponent<StatusEffectManager>();
         
        if (burnTarget != null)
        {
            burnTarget.AddEffect(fireStatusPrefab);
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
            ApplyBurn();
            Destroy(gameObject);
        }
    }
}
