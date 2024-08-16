using System.Collections;
using UnityEngine;

/// <summary>
/// Abstraktni trida reprezentuje Zakladni slunecnici
/// </summary>
public abstract class AbstractSunFlower : Plant, ISellable
{
    /// <summary>
    /// Stavy slunecnice
    /// </summary>
    protected enum SunFlowerState
    {
        /// <summary>
        /// Slunecnice nic nedela
        /// </summary>
        Idle,
        /// <summary>
        /// Slunecnice Generuje slunce
        /// </summary>
        Generating
    }

    /// <summary>
    /// Prefab slunce, kterou generuje slunecnice
    /// </summary>
    protected GameObject sunPrefab { get; set; }

    /// <summary>
    /// Aktualni stav slunecnice
    /// </summary>
    protected SunFlowerState state { get; set; }

    /// <summary>
    /// Indikator, zda byl aktualni stav dokoncen
    /// </summary>
    protected bool stateComplete { get; set; }

    /// <summary>
    /// Animator pro zmenu animace
    /// </summary>
    protected Animator animator { get; set; }

    /// <summary>
    /// Zda aktualne muze generovat slunce
    /// </summary>
    protected bool canGenerate { get; set; }

    /// <summary>
    /// Doba, jak dlouho musi slunecnice pro generovani slunce
    /// </summary>
    protected float cooldown { get; set; }

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
    public void SelectState()
    {
        stateComplete = false;

        if (canGenerate)
        {
            state = SunFlowerState.Generating;
            animator.Play("Generate");
        }
        else
        {
            state = SunFlowerState.Idle;
            animator.Play("Idle");
        }
    }

    /// <summary>
    /// Metoda aktualizuje stav slunecnice
    /// </summary>
    public void UpdateState()
    {
        switch (state)
        {
            case SunFlowerState.Idle:
                Idle();
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
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metoda proda slunecnici
    /// </summary>
    public void Sell()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Metoda kontroluje, zda muze generovat slunce
    /// </summary>
    public void Idle()
    {
        if (canGenerate)
        {
            stateComplete = true;
        }
    }

    /// <summary>
    /// Metoda generuje slunce
    /// </summary>
    public virtual void Generate()
    {
        stateComplete = true;
        Instantiate(sunPrefab, new Vector3(transform.position.x, transform.position.y, sunPrefab.transform.position.z), transform.rotation);
        StartCoroutine(GenerateCooldown());
    }

    /// <summary>
    /// Korutina vyckava na dalsi slunce
    /// </summary>
    public IEnumerator GenerateCooldown()
    {
        canGenerate = false;
        yield return new WaitForSeconds(cooldown);
        canGenerate = true;
    }
}
