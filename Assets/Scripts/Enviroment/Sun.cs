using System.Collections;
using UnityEngine;

/// <summary>
/// Trida reprezentuje slunce
/// </summary>
public class Sun : MonoBehaviour, IMoveable
{
    /// <summary>
    /// Stavy slunce
    /// </summary>
    private enum SunState
    {
        /// <summary>
        /// Slunce pada
        /// </summary>
        Falling,
        /// <summary>
        /// Slunce zustava na miste
        /// </summary>
        Staying
    }

    /// <summary>
    /// Hodnota slunce
    /// </summary>
    public int sunValue { get; set; }

    /// <summary>
    /// Rychlost padu 
    /// </summary>
    public float movementSpeed { get; set; }

    /// <summary>
    /// Doba padu slunce
    /// </summary>
    public float timeFall { get; set; }

    /// <summary>
    /// Doba zivotnosti slunce
    /// </summary>
    private float lifeTime { get; set; }

    /// <summary>
    /// Aktualni stav slunce
    /// </summary>
    private SunState state { get; set; }

    /// <summary>
    /// Indikator, zda byl aktualni stav dokoncen
    /// </summary>
    private bool stateComplete { get; set; }

    void Awake()
    {
        sunValue = 50;
        state = SunState.Falling; 
        stateComplete = false; 
        lifeTime = 20f; 
        timeFall = 0.2f;
        movementSpeed = 2f;
    }

    void Start()
    {
        StartCoroutine(TimeFall());
        StartCoroutine(LifeTime());
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
        state = SunState.Staying;
    }

    /// <summary>
    /// Metoda aktualizuje stav slunce
    /// </summary>
    private void UpdateState()
    {
        switch (state)
        {
            case SunState.Falling:
                Move();
                break;
        }
    }

    /// <summary>
    /// Metoda prida hodnotu slunce hraci
    /// </summary>
    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            FindObjectOfType<PlantCardManager>().AddSun(sunValue);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metoda zajistuje pad slunce
    /// </summary>
    public void Move()
    {
        transform.Translate(-transform.up * Time.deltaTime * movementSpeed, Space.World);
    }

    /// <summary>
    /// Korutina vyckava na konec zivotnosti slunce a pote ji znici 
    /// </summary>
    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime); 
        Destroy(gameObject); 
    }


    /// <summary>
    /// Korutina vyckava na konec padu a pote ukonci stav padani 
    /// </summary>
    private IEnumerator TimeFall()
    {
        yield return new WaitForSeconds(timeFall);
        stateComplete = true;
    }
}
