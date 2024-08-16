using UnityEngine;

/// <summary>
/// Trida reprezentuje wall nut
/// </summary>
public class WallNut : Plant, ISellable
{
    /// <summary>
    /// Stavy wall nut
    /// </summary>
    private enum WallNutState
    {
        /// <summary>
        /// Wall nut nic nedela
        /// </summary>
        Idle,
        /// <summary>
        /// Wall nut tankuje dmg od okoli
        /// </summary>
        Tanking,

    }

    /// <summary>
    /// Animator pro zmenu animace
    /// </summary>
    private Animator animator { get; set; }

    /// <summary>
    /// Aktualni stav wall nut
    /// </summary>
    private WallNutState state { get; set; }

    /// <summary>
    /// Indikator, zda byl aktualni stav dokoncen
    /// </summary>
    private bool stateComplete { get; set; }

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
    }

    /// <summary>
    /// Metoda vybere novy stav po dokonceni predchoziho stavu
    /// </summary>
    private void SelectState()
    {
        stateComplete = false;

        if (state == WallNutState.Tanking)
        {
            animator.Play("Tank");
        } else
        {
            animator.Play("Idle");
        }
    }

    /// <summary>
    /// Metoda proda wall nut
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

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metoda nastavi do puvodniho stavu
    /// </summary>
    public override void ResetObject()
    {
        health = 200;
        state = WallNutState.Idle;
        stateComplete = false;
    }

    /// <summary>
    /// Metoda zpracuje kolidujici s zombikem
    /// </summary>
    /// <param name="collision">Kolize objektu</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Zombies"))
        {
            state = WallNutState.Tanking;
            stateComplete = true;
        }
    }

    /// <summary>
    /// Metoda zpracuje konec kolizi s zombikem
    /// </summary>
    /// <param name="collision">Kolize objektu</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Zombies"))
        {
            state = WallNutState.Idle;
            stateComplete = true;
        }
    }

}
