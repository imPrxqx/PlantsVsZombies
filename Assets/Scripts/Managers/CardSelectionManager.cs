using UnityEngine;

/// <summary>
/// Trida ma na starost vyber rostlin do hry
/// </summary>
public class CardSelectionManager : MonoBehaviour
{
    /// <summary>
    /// Reference na Plant Card Manager
    /// </summary>
    private PlantCardManager plantCardManager { get; set; }

    void Start()
    {
        plantCardManager = FindObjectOfType<PlantCardManager>();
    }

    /// <summary>
    /// Metoda prida rostlinu do vybranych - pro UI
    /// </summary>
    public void SelectPlant(PlantCardScriptableObject plant)
    {
        plantCardManager.AddPlant(plant);
    }

    /// <summary>
    /// Metoda odebere rostlinu z vybranych - pro UI
    /// </summary>
    public void DeselectPlant(int slotNum)
    {
        plantCardManager.RemovePlant(slotNum);
    }

    /// <summary>
    /// Metoda deaktivuje moznost vybirani karet
    /// </summary>
    public void StopSelect()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Metoda aktivuje moznost vybirani karet
    /// </summary>
    public void StartSelect()
    {
        gameObject.SetActive(true);
    }
}
