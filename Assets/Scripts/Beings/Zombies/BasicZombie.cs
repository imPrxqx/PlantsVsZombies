using System.Collections;
using UnityEngine;

/// <summary>
/// Trida reprezentujici zakladniho zombika
/// </summary>
public class BasicZombie : Zombie, IMoveable, IDamager
{

    /// <summary>
    /// Stavy zakladniho zombika
    /// </summary>
    protected enum BasicZombieState
    {
        /// <summary>
        /// zakladni zombik se pohybuje
        /// </summary>
        Moving,
        /// <summary>
        /// zakladni zombik ji rostlinu
        /// </summary>
        Eating,
        /// <summary>
        /// zakladni zombik umira
        /// </summary>
        Dying
    }


    /// <summary>
    /// Animator pro zmenu animace
    /// </summary>
    protected Animator animator { get; set; }

    /// <summary>
    /// Cilovy herni objekt, ktery zakladni zombik ji
    /// </summary>
    protected GameObject target { get; set; }

    /// <summary>
    /// Aktualni stav zakladniho zombika
    /// </summary>    
    protected BasicZombieState state { get; set; }

    /// <summary>
    /// Indikator, zda byl aktualni stav dokoncen
    /// </summary>
    protected bool stateComplete { get; set; }

    /// <summary>
    /// Indikator, zda muze jist rostlinu
    /// </summary>
    protected bool canAttack { get; set; }

    /// <summary>
    /// Aktualni celkove poskozneni, ktere objekt zpusobuje
    /// </summary>
    public int damage { get; set; }

    /// <summary>
    /// Doba, jak dlouho musi zakladni zombik cekat po pouziti snedeni casti rostliny
    /// </summary>
    protected float cooldown { get; set; }

    /// <summary>
    /// Aktualni rychlost zakladniho zombika
    /// </summary>
    public float movementSpeed { get; set; }

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

        if (health <= 0)
        {
            state = BasicZombieState.Dying;
            animator.Play("Die");
        }
        else if (target != null)
        {
            state = BasicZombieState.Eating;
            animator.Play("Eat");
        }
        else
        {
            state = BasicZombieState.Moving;
            animator.Play("Move");
        }
    }

    /// <summary>
    /// Metoda aktualizuje stav zakladniho zombika
    /// </summary>
    protected void UpdateState()
    {
        switch (state)
        {
            case BasicZombieState.Moving:
                Move();
                break;
            case BasicZombieState.Eating:
                Attack();
                break;
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
            stateComplete = true;
        }
    }

    /// <summary>
    /// Metoda zpusobi celkove poskozeni
    /// </summary>
    public virtual void Attack()
    {
        if (canAttack == true && target != null)
        {
            IDamagable damageTarget = target.GetComponent<IDamagable>();
            damageTarget.TakeDamage(damage);

            StartCoroutine(AttackCooldown());
        }
    }

    /// <summary>
    /// Metoda zpracovava smrt zakladniho zombika 
    /// </summary>
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Metoda zajistuje pohyb
    /// </summary>
    public virtual void Move()
    {
        transform.position = transform.position + new Vector3(-(movementSpeed * Time.deltaTime), 0.0f, 0.0f);
    }

    /// <summary>
    /// Metoda zpracuje prichozi kolizi s rostlinou
    /// </summary>
    /// <param name="collision">Kolize objektu</param>
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Plants") && collision.GetComponent<IDamagable>() != null)
        {
            target = collision.gameObject;
            stateComplete = true;
        }
    }

    /// <summary>
    /// Metoda zpracuje konec kolizi s rostlinou
    /// </summary>
    /// <param name="collision">Kolize objektu</param>
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Plants"))
        {
            target = null;
            stateComplete = true;
        }
    }

    /// <summary>
    /// Korutina vyckava na dalsi snedeni casti rostliny
    /// </summary>
    protected IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
