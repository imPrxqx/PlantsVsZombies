using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trida reprezentuje specialni typ zakladniho zombika s kuzele
/// </summary>
public class ConeZombie : BasicZombie, IImmunityEffects
{
    /// <summary>
    /// Zakladni zombik vlastni kuzel
    /// </summary>
    private Cone cone { get; set; }

    /// <summary>
    /// Seznam statusovych efektu, na ktere je immuni
    /// </summary>
    public List<Type> immunityEffectsList => new List<Type>() { typeof(FireStatus) };


    void Awake()
    {
        cone = Instantiate(Resources.Load<GameObject>("Prefabs/Items/Plastic/Cone"), transform).GetComponent<Cone>();

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
    /// Metoda zpracuje poskozeni prijate z vnejsich zdroju
    /// </summary>
    /// <param name="damage">Celkove poskozeni</param>
    public override void TakeDamage(int damage)
    {
        int remainderDamage = damage;

        if (cone != null)
        {
            remainderDamage = Mathf.Max(0, damage - cone.health);            
            cone.GetComponent<IDamagable>().TakeDamage(damage);
        }

        health -= remainderDamage;

        if (health <= 0)
        {
            stateComplete = true;
        }
    }
}
