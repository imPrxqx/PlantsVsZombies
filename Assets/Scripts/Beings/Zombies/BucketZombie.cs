using UnityEngine;

/// <summary>
/// Trida reprezentuje specialni typ zakladniho zombika s kyblikem
/// </summary>
public class BucketZombie : BasicZombie
{
    /// <summary>
    /// Zakladni zombik vlastni kyblik
    /// </summary>
    private Bucket bucket {  get; set; }

    void Awake()
    {        
        movementSpeed = 1f;
        cooldown = 1f;
        damage = 10;
        health = 100;
        state = BasicZombieState.Moving;
        target = null;
        stateComplete = false;
        canAttack = true;
        bucket = Instantiate(Resources.Load<GameObject>("Prefabs/Items/Metal/Bucket"), transform).GetComponent<Bucket>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        bucket.OnItemRemoved += HandleBucketRemoved;
    }

    void Update()
    {
        if (stateComplete)
        {
            SelectState();
        }

        UpdateState();
    }

    /// <summary>
    /// Metoda zpracuje poskozeni prijate z vnejsich zdroju
    /// </summary>
    /// <param name="damage">Celkove poskozeni</param>
    public override void TakeDamage(int damage)
    {
        int remainderDamage = damage;

        if (bucket != null)
        {
            remainderDamage = Mathf.Max(0, damage - bucket.health);
            bucket.GetComponent<IDamagable>().TakeDamage(damage);
        }

        health -= remainderDamage;

        if (health <= 0)
        {
            stateComplete = true;
        }
    }

    /// <summary>
    /// Metoda je volana, kdyz je kyblik odebran
    /// </summary>
    private void HandleBucketRemoved()
    {
        bucket.isRemoved = true;
        bucket = null;
    }
}
