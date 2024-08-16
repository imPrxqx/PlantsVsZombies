using UnityEngine;

/// <summary>
/// Trida reprezentuje specialni typ Pea shooter
/// </summary>
public class FirePeaShooter : PeaShooter
{
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
    /// Metoda nastavi specialni typ Pea Shooter do puvodniho stavu
    /// </summary>
    public override void ResetObject()
    {
        cooldown = 2f;
        health = 100;
        state = PeaShooterState.Idle;
        stateComplete = false;
        canAttack = true;
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullets/Fire Pea Bullet");
    }
}
