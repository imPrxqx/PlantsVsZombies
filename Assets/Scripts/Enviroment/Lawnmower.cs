using System.Collections;
using UnityEngine;

/// <summary>
/// Trida reprezentuje sekacku
/// </summary>
public class Lawnmower : MonoBehaviour, IMoveable
{
    /// <summary>
    /// Celkove poskozeni sekacky
    /// </summary>
    private int damage { get; set; }

    /// <summary>
    /// Doba zivotnosti sekacky
    /// </summary>
    private float duration { get; set; }

    /// <summary>
    /// Stav sekacky
    /// </summary>    
    public bool activated { get; set; }

    /// <summary>
    /// Aktualni rychlost sekacky
    /// </summary>
    public float movementSpeed { get; set; }

    void Awake()
    {
        damage = 2000;
        duration = 7f;
        movementSpeed = 4f;
        activated = false;
    }

    void Update()
    {
        if (activated == true)
        {
            Move();
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
            if (activated == false)
            {
                activated = true;
                StartCoroutine(LawnmowerDistance());
            }

            IDamagable target = collision.gameObject.GetComponent<IDamagable>();

            if (target != null)
            {
                Attack(target);
            }
        }
    }

    /// <summary>
    /// Metoda zpusobi celkove poskozeni
    /// </summary>
    /// <param name="target">Cilový objekt</param>
    public void Attack(IDamagable target)
    {
        target.TakeDamage(damage);
    }

    /// <summary>
    /// Metoda zajistuje pohyb sekacky
    /// </summary>
    public void Move()
    {
        transform.position += new Vector3(movementSpeed * Time.deltaTime, 0.0f, 0.0f);
    }

    /// <summary>
    /// Korutina vyckava na konec zivotnosti sekacky a pote ji znici 
    /// </summary>
    private IEnumerator LawnmowerDistance()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
