using UnityEngine;

/// <summary>
/// Trida reprezentuje policko pro polozeni rostliny
/// </summary>
public class PlantSlot : MonoBehaviour
{
    /// <summary>
    /// Aktualne zasazena rostlina
    /// </summary>
    private GameObject plant { get; set; }

    /// <summary>
    /// Zda je policko obsazene
    /// </summary>
    private bool isOccupied { get; set; }

    /// <summary>
    /// Reference na handManager
    /// </summary>
    private HandManager handManager { get; set; }

    /// <summary>
    /// Kolik slunci vratit za prodani zasazene rostliny
    /// </summary>
    private int refund;

    void Awake()
    {
        plant = null;
        isOccupied = false;
        refund = 0;
    }

    void Start()
    {
        handManager = FindObjectOfType<HandManager>();
    }

    void Update()
    {
        if (isOccupied && plant == null)
        {
            isOccupied = false;
            refund = 0;
        }
    }

    /// <summary>
    /// Metoda kontroluje interakci hrace s polickem
    /// </summary>
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (handManager.isHoldingShovel)
            {
                if (isOccupied)
                {
                    HandleShovel();
                }
            }
            else
            {
                HandleCard();
            }
        }
    }

    /// <summary>
    /// Metoda proda rostlinu z policka
    /// </summary>
    private void HandleShovel()
    {
        plant.GetComponent<ISellable>().Sell();
        handManager.Refund(refund);
        refund = 0;
        isOccupied = false;
        handManager.RemoveSelect();
        handManager.isHoldingShovel = false;
    }

    /// <summary>
    /// Metoda zasadi rostlinu na policko
    /// </summary>
    private void HandleCard()
    {
        PlantCardScriptableObject newPlant;

        if (plant == null)
        {
            newPlant = handManager.GetPlantCard(null);
        }
        else
        {
            newPlant = handManager.GetPlantCard(plant.GetComponent<Plant>());
        }

        if (newPlant == null)
        {
            handManager.plantCardManager.SelectCard(-1);
            return; 
        }

        if (isOccupied)
        {
            Destroy(plant);
        }

        refund = newPlant.refundCost;
        isOccupied = true;
        plant = Instantiate(newPlant.plant, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, newPlant.plant.transform.position.z), newPlant.plant.transform.rotation, null);
        handManager.plantCardManager.SelectCard(-1);
    }

    /// <summary>
    /// Metoda vypne rostlinu na policku
    /// </summary>
    public void StopShow()
    {
        if (plant == null)
        {
            return;
        }

        plant.SetActive(false);
    }

    /// <summary>
    /// Metoda zapne rostlinu na policku
    /// </summary>
    public void StartShow()
    {
        if (plant == null)
        {
            return;
        }

        plant.SetActive(true);
        plant.GetComponent<IResetable>().ResetObject();
    }

}
