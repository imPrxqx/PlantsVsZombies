using System.Collections;
using UnityEngine;


/// <summary>
/// Trida reprezentuje pea shooter
/// </summary>
public class PeaShooter : Plant, ISellable
{

    /// <summary>
    /// Stavy pea shooter
    /// </summary>
    protected enum PeaShooterState
    {
        /// <summary>
        /// Pea shooter hledá zombiky
        /// </summary>
        Idle,
        /// <summary>
        /// Pea shooter strili projektily
        /// </summary>
        Shooting
    }

    /// <summary>
    /// Prefab projektilu, kterou strili
    /// </summary>
    protected GameObject bulletPrefab { get; set; }

    /// <summary>
    /// Animator pro zmenu animace
    /// </summary>
    protected Animator animator { get; set; }

    /// <summary>
    /// Aktualni stav pea shooteru
    /// </summary>
    protected PeaShooterState state { get; set; }

    /// <summary>
    /// Indikator, zda byl aktualni stav dokoncen
    /// </summary>
    protected bool stateComplete { get; set; }

    /// <summary>
    /// Zda aktualne muze utocit
    /// </summary>
    protected bool canAttack { get; set; }

    /// <summary>
    /// Doba, jak dlouho musi pea shooter cekat po pouziti utoku
    /// </summary>
    protected float cooldown { get; set; }

    void Awake()
    {
        ResetObject();
    }

    
    void Start()
    {
        animator = GetComponent<Animator>();
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
    /// Metoda vybere novy stav po dokonceni predchoziho stavu
    /// </summary>
    protected void SelectState()
    {
        stateComplete = false;

        if(canAttack == true)
        {
            state = PeaShooterState.Shooting;
            animator.Play("Shoot");
        }
        else
        {
            state = PeaShooterState.Idle;
            animator.Play("Idle");
        }
    }

    /// <summary>
    /// Metoda aktualizuje stav pea shootera
    /// </summary>
    protected void UpdateState()
    {
        switch (state)
        {
            case PeaShooterState.Idle:
                CheckZombies();
                break;
        }
    }

    /// <summary>
    /// Metoda vytvori projektil
    /// </summary>
    protected virtual void Shoot()
    {
        stateComplete = true;
        Instantiate(bulletPrefab, gameObject.transform.position + new Vector3(0,0,-3), transform.rotation);
        StartCoroutine(ShootCooldown());
    }

    /// <summary>
    /// Metoda hleda, zda na jeho rade vyskytuje zombik
    /// </summary>
    protected void CheckZombies()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 20f, LayerMask.GetMask("Zombies"));

        if (hit.collider != null)
        {
            stateComplete = true;
        }
    }

    /// <summary>
    /// Metoda zpracuje poskozeni prijate z vnejsich zdroju
    /// </summary>
    /// <param name="damage">Celkove poskozeni</param>
    public override void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metoda proda pea shooter
    /// </summary>
    public virtual void Sell()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Korutina vyckava na dalsi strelbu projektilu
    /// </summary>
    protected IEnumerator ShootCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    /// <summary>
    /// Metoda nastavi pea shooter do puvodniho stavu
    /// </summary>
    public override void ResetObject()
    {
        cooldown = 2f;
        health = 100;
        state = PeaShooterState.Idle;
        stateComplete = false;
        canAttack = true;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullets/Pea Bullet");
    }
}
