using UnityEngine;

/// <summary>
/// Trida reprezentuje projektil Pea Bullet
/// </summary>
public class PeaBullet : Projectile
{
    void Awake()
    {
        movementSpeed = 5f;
        damage = 20;
    }

    void Update()
    {
        Move();
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
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metoda zpusobi celkove poskozeni
    /// </summary>
    public override void Attack()
    {
        IDamagable damageTarget = target.GetComponent<IDamagable>();

        if (target != null)
        {
            damageTarget.TakeDamage(damage);
        }
    }

    /// <summary>
    /// Metoda zajistuje pohyb projektilu Pea Bullet
    /// </summary>
    public override void Move()
    {
        transform.Translate(Vector2.right * movementSpeed * Time.deltaTime);
    }
}
