using UnityEngine;

/// <summary>
/// Trida ma na starost, co se prave drzi na ruce - lopata/rostlina
/// </summary>
public class HandManager : MonoBehaviour
{

    /// <summary>
    /// Zda je ruka aktivovana - slouzi pro Game Manager
    /// </summary>
    public bool isFree { get; set; }

    /// <summary>
    /// Zda drzi lopatu nebo rostlinu
    /// </summary>
    public bool isHoldingShovel { get; set; }

    /// <summary>
    /// Reference na Plant Card Manager
    /// </summary>
    public PlantCardManager plantCardManager { get; set; }

    /// <summary>
    /// Co se prave drzi na ruce
    /// </summary>
    public GameObject handObject { get; set; }

    private void Awake()
    {
        isFree = false;
        isHoldingShovel = false;
    }

    private void Start()
    {
        plantCardManager = FindObjectOfType<PlantCardManager>();
        plantCardManager.OnPlayerSelect += RemoveSelect;
    }

    void Update()
    {
        if(isFree == false)
        {
            return;
        }

        if(handObject != null )
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            handObject.transform.position = mousePosition;
        } 
        else
        {
            int index = plantCardManager.GetSelectedIndex();

            if (index == -1)
            {
                return;
            }
            isHoldingShovel = false;

            handObject = new GameObject("TempPlant");

            SpriteRenderer tempRenderer = handObject.AddComponent<SpriteRenderer>();
            tempRenderer.sprite = plantCardManager.slots[index].cardScript.icon;
            tempRenderer.sortingOrder = 2;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            handObject.transform.position = mousePosition;
        }

        if (Input.GetMouseButtonDown(1))
        {
            isHoldingShovel = false;
            plantCardManager.SelectCard(-1);
        }
    }


    /// <summary>
    /// Metoda pro sazeni rostlin
    /// </summary>
    public PlantCardScriptableObject GetPlantCard(Plant ocupant)
    {
        if (isFree == false)
        {
            return null;
        }

        return plantCardManager.PlantCard(ocupant);
    }

    /// <summary>
    /// Metoda vrati slunce za prodanou rostlinu
    /// </summary>
    public void Refund(int amount)
    {
        plantCardManager.AddSun(amount);
    }

    /// <summary>
    /// Metoda nastavi drzeni lopaty
    /// </summary>
    public void SelectShovel()
    {
        if (isFree == false)
        {
            return;
        }

        bool tmpShovel = isHoldingShovel;
        plantCardManager.SelectCard(-1);
        isHoldingShovel = !tmpShovel;

        if (isHoldingShovel)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            handObject = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Shovel"), mousePosition, new Quaternion() ,null);
        }

    }

    /// <summary>
    /// Metoda vypne funkcnost HandMangera
    /// </summary>
    public void StopHand()
    {
        isFree = false;
    }
    /// <summary>
    /// metoda zapne funkcnost HandManagera
    /// </summary>
    public void StartHand()
    {
        isFree = true;

    }

    internal void RemoveSelect()
    {
        Destroy(handObject);
        isHoldingShovel = false;
    }
}
