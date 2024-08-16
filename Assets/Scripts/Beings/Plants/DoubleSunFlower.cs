using UnityEngine;

/// <summary>
/// Trida reprezentuje specialni typ slunecnice
/// </summary>
public class DoubleSunFlower : AbstractSunFlower, IUpgradeable<SunFlower>
{
    void Awake()
    {
        ResetObject();
    }

    /// <summary>
    /// Metoda generuje slunce
    /// </summary>
    public override void Generate()
    {
        stateComplete = true;
        Instantiate(sunPrefab, new Vector3(transform.position.x + 0.3f, transform.position.y, -1), transform.rotation);
        Instantiate(sunPrefab, new Vector3(transform.position.x - 0.3f, transform.position.y, -1), transform.rotation);
        StartCoroutine(GenerateCooldown());
    }

    /// <summary>
    /// Metoda nastavi specialni slunecnici do puvodniho stavu
    /// </summary>
    public override void ResetObject()
    {
        cooldown = 10f;
        health = 100;
        state = SunFlowerState.Idle;
        stateComplete = false;
        canGenerate = true;
        sunPrefab = Resources.Load<GameObject>("Prefabs/Enviroment/Sun");

    }
}
