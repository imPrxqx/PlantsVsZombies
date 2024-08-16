using UnityEngine;

/// <summary>
/// Trida reprezentuje kartu rostliny
/// </summary>
[CreateAssetMenu(menuName = "Cards/Plant Card", fileName = "New Plant Card")]
public class PlantCardScriptableObject : ScriptableObject
{
    /// <summary>
    /// Prefab rostliny, ktery bude vytvoren po pouziti karty
    /// </summary>
    public GameObject plant;

    /// <summary>
    /// Ikonka karty, ktera se bude zobrazovat
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// Cenovka karty, kterou hrac zaplati za pouziti rostliny
    /// </summary>
    public int cost;

    /// <summary>
    /// Cenovka, kterou dostane hrac zpet po prodani rostliny
    /// </summary>
    public int refundCost;

    /// <summary>
    /// Doba pro opetovne pouziti karty
    /// </summary>
    public float cooldown;

}
