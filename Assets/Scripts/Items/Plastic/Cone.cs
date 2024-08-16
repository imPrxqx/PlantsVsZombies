using UnityEngine;

/// <summary>
/// Trida reprezentuje kuzel
/// </summary>
public class Cone : Plastic, IDamagable
{

    /// <summary>
    /// Aktualni stav vyznaceny za pomoci sprite
    /// </summary>
    private SpriteRenderer spriteRenderer { get; set; }

    /// <summary>
    /// Maximalni zdravi
    /// </summary>
    public int maxHealth { get; set; }

    /// <summary>
    /// Aktualni zdravi
    /// </summary>
    public int health { get; set; }

    void Awake()
    {
        health = 100;
        maxHealth = health;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Metoda zpracuje poskozeni prijate z vnejsich zdroju
    /// </summary>
    /// <param name="damage">Celkove poskozeni</param>
    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateSprite();

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Metoda aktualizuje stav za pomoci sprite podle aktualniho procenta zdravi
    /// </summary>
    public void UpdateSprite()
    {
        float healthPercentage = (float)health / maxHealth;

        if (healthPercentage > 0.5f)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Items/Plastic/Cone - 2");
        }
        else if (healthPercentage > 0.25f)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Items/Plastic/Cone - 3");
        }
    }
}
