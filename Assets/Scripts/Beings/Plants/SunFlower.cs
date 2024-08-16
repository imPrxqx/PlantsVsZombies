using UnityEngine;

/// <summary>
/// Trida reprezentuje slunecnici
/// </summary>
public class SunFlower : AbstractSunFlower
{
    void Awake()
    {
        ResetObject();
    }

    /// <summary>
    /// Metoda nastavi slunecnici do puvodniho stavu
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
