using System.Collections;
using UnityEngine;

/// <summary>
/// Trida reprezentuje magnet
/// </summary>
public class Magnet : Plant, IDamager, ISellable
{
    /// <summary>
    /// Stavy magnetu
    /// </summary>
    private enum MagnetState
    {
        /// <summary>
        /// Magnet nic nedela
        /// </summary>
        Idle,
        /// <summary>
        /// Magnet pritahuje kovove veci
        /// </summary>
        Retrieving,
        /// <summary>
        /// Magnet znicuje kovove veci
        /// </summary>
        Destroying
    }

    /// <summary>
    /// Animator pro zmenu animace
    /// </summary>
    private Animator animator { get; set; }

    /// <summary>
    /// Cilovou vec, kterou znicuje magnet
    /// </summary>
    private GameObject target { get; set; }

    /// <summary>
    /// Aktualni stav magnetu
    /// </summary>
    private MagnetState state { get; set; }

    /// <summary>
    /// Indikator, zda byl aktualni stav dokoncen
    /// </summary>
    private bool stateComplete { get; set; }

    /// <summary>
    /// Zda aktualne muze utocit
    /// </summary>
    private bool canAttack { get; set; }

    /// <summary>
    /// Celkove poskozeni
    /// </summary>
    public int damage { get; set; }

    /// <summary>
    /// Rychlost pritahovani veci
    /// </summary>
    private float retrieveSpeed { get; set; }

    /// <summary>
    /// Doba, jak dlouho musi magnet cekat po pouziti utoku do veci
    /// </summary>
    private float cooldown { get; set; }

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
    private void SelectState()
    {
        stateComplete = false;

        if (canAttack)
        {
            state = MagnetState.Destroying;
            animator.Play("Destroy");
        }
        else if (target != null)
        {
            state = MagnetState.Retrieving;
            animator.Play("Retrieve");
        }
        else
        {
            state = MagnetState.Idle;
            animator.Play("Idle");
        }
    }

    /// <summary>
    /// Metoda aktualizuje stav magnetu
    /// </summary>
    private void UpdateState()
    {
        switch (state)
        {
            case MagnetState.Idle:
                FindRemovableMetal();
                break;
            case MagnetState.Retrieving:
                RetrieveItem();
                break;
            case MagnetState.Destroying:
                Attack();
                break;
        }
    }

    /// <summary>
    /// Metoda proda magnet
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
    /// Metoda postupne pritahuje cilovou vec na svoji pozici
    /// </summary>
    private void RetrieveItem()
    {
        float step = retrieveSpeed * Time.deltaTime;
        target.transform.position = Vector3.MoveTowards(target.transform.position, gameObject.transform.position, step);
 
        if (Vector3.Distance(gameObject.transform.position, target.transform.position) < 0.01f)
        {
            target.transform.position = gameObject.transform.position;
            canAttack = true;
            stateComplete = true;
        }
    }

    /// <summary>
    /// Metoda vyhleda vec, ktery splnuje kontrakt IRemoveable<Metal> - Jeho zpusob odebirani veci
    /// </summary>
    private void FindRemovableMetal()
    {
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            IRemoveable<Metal> removableMetal = obj.GetComponent<IRemoveable<Metal>>();

            if (removableMetal != null && obj.GetComponent<IRemoveable<Metal>>().isRemoved == false && target == null)
            {
                target = obj.GetComponent<IRemoveable<Metal>>().TakeItem();
                target.transform.SetParent(transform);
                stateComplete = true;
                return;
            }
        }

    }

    /// <summary>
    /// Metoda zpusobi celkove poskozeni
    /// </summary>
    public virtual void Attack()
    {
        if (canAttack == true && target != null)
        {
            canAttack = false;
            StartCoroutine(AttackCooldown());
            target.GetComponent<IDamagable>().TakeDamage(damage);

        } else if(target == null)
        {
            StopAllCoroutines();

            canAttack = false;

            stateComplete = true;
        }
    }

    /// <summary>
    /// Korutina vyckava na dalsi utok na vec
    /// </summary>
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    /// <summary>
    /// Metoda nastavi magnet do puvodniho stavu
    /// </summary>
    public override void ResetObject()
    {
        if(target != null)
        {
            Destroy(target);
            target = null;
        }


        damage = 6;
        cooldown = 0.5f;
        health = 100;
        retrieveSpeed = 8f;
        state = MagnetState.Idle;
        target = null;
        stateComplete = false;
        canAttack = false;
    }
}
