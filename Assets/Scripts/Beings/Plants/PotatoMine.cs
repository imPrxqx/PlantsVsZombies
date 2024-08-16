using System.Collections;
using UnityEngine;

/// <summary>
/// Trida reprezentuje potato mine
/// </summary>
public class PotatoMine : Plant, ISellable, IDamager
{
    /// <summary>
    /// Stavy potato mine
    /// </summary>
    private enum PotatoMineState
    {
        /// <summary>
        /// Potato mine je schovany pod travnikem
        /// </summary>
        IdleDown,
        /// <summary>
        /// Potato mine neni schovany pod travnikem
        /// </summary>
        IdleUp,
        /// <summary>
        /// Potato mine vybuchuje
        /// </summary>
        Exploding
    }


    /// <summary>
    /// Aktualni stav potato mine
    /// </summary>
    private PotatoMineState state { get; set; }

    /// <summary>
    /// Animator pro zmenu animace
    /// </summary>
    private Animator animator { get; set; }

    /// <summary>
    /// Indikator, zda byl aktualni stav dokoncen
    /// </summary>
    private bool stateComplete { get; set; }

    /// <summary>
    /// Zda je potato mine pripraveny k vybuchu
    /// </summary>
    private bool isReady { get; set; }

    /// <summary>
    /// Celkove poskozeni
    /// </summary>
    public int damage { get; set; }

    /// <summary>
    /// Radius vybuchu
    /// </summary>
    private float explosionRadius { get; set; }

    /// <summary>
    /// Doba, jak dlouho je potato mine schovany
    /// </summary>
    private float waitTime { get; set; }


    void Awake()
    {
        ResetObject();
    }
      
    void Start()
    {        
        animator = GetComponent<Animator>();

        StartCoroutine(GettingReady());
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

    private void SelectState()
    {
        stateComplete = false;

        if (isReady == true)
        {
            state = PotatoMineState.Exploding;
            animator.Play("Explode");
        }
        else
        {
            state = PotatoMineState.IdleUp;
            animator.Play("Idle Up");
            isReady = true;
        }
    }

    /// <summary>
    /// Metoda aktualizuje stav potato mine
    /// </summary>
    private void UpdateState()
    {
        switch (state)
        {
            case PotatoMineState.IdleUp:
                CheckZombies();
                break;
        }
    }

    /// <summary>
    /// Metoda zpusobi poskozeni na nekolik cilovych hernich objektu v okoli
    /// </summary>
    public void Attack()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamagable target = hitCollider.GetComponent<IDamagable>();

            if (target != null && target is Zombie)
            {
                target.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Metoda proda potato mine
    /// </summary>
    public void Sell()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Metoda zpracuje poskozeni prijate z vnejsich zdroju
    /// </summary>
    /// <param name="damage">Celkove poskozeni</param>
    public override void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metoda kontroluje zda jsou v blizkosti zombici
    /// </summary>
    private void CheckZombies()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, explosionRadius - 0.01f, LayerMask.GetMask("Zombies"));

        if (hit.collider != null)
        {
            stateComplete = true;
        }
    }

    /// <summary>
    /// Korutina vyckava, az bude potato mine pripraven vylezt ven z diry
    /// </summary>
    private IEnumerator GettingReady()
    {
        yield return new WaitForSeconds(waitTime);
        stateComplete = true;
    }

    /// <summary>
    /// Metoda nastavi potato mine do puvodniho stavu
    /// </summary>
    public override void ResetObject()
    {
        waitTime = 10f;
        health = 100;
        damage = 500;
        explosionRadius = 0.8f;
        state = PotatoMineState.IdleDown;
        stateComplete = false;
        isReady = false;
    }
}
