using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Trida reprezentuje kartu s rostlinou
/// </summary>
public class CardSlot : MonoBehaviour
{
    /// <summary>
    /// Rostlina, kterou obsahuje karta
    /// </summary>
    public PlantCardScriptableObject cardScript { get; set; }

    /// <summary>
    /// Odkaz na PlantCardManagera
    /// </summary>
    public PlantCardManager plantCardManager { get; set; }

    /// <summary>
    /// Poradove cislo karty
    /// </summary>
    public int cardNumber { get; set; }

    /// <summary>
    /// Reference kde se bude zobrazovat ikona rostliny
    /// </summary>
    public Image icon { get; set; }

    /// <summary>
    /// Reference pro vizualizaci cooldownu
    /// </summary>
    public RectTransform cooldownImage { get; set; }

    /// <summary>
    /// Reference na text s cenou rostliny 
    /// </summary>
    public TextMeshProUGUI costText { get; set; }

    /// <summary>
    /// Doba, kdy se ukonci cekani na opetovne pouziti karty do
    /// </summary>
    public double cooldownTimer { get; set; }

    /// <summary>
    /// Maximalni vyska ukazatele cooldownu
    /// </summary>
    public int maxTop { get; set; } 

    void Awake()
    {

        cooldownTimer = 0;
        maxTop = 185;
    }

    /// <summary>
    /// Metoda inicializuje kartu 
    /// </summary>
    public void Init(PlantCardScriptableObject plant, PlantCardManager plantCardManager, int cardNumber)
    {
        icon = gameObject.transform.GetChild(0).GetComponent<Image>();
        cooldownImage = gameObject.transform.GetChild(1).GetComponent<RectTransform>();
        costText = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        cardScript = plant;

        this.plantCardManager = plantCardManager;
        this.cardNumber = cardNumber;
        icon.sprite = cardScript.icon;
        costText.text = cardScript.cost.ToString();
        cooldownImage.offsetMax = new Vector2(cooldownImage.offsetMax.x, -maxTop);
    }

    /// <summary>
    /// Metoda aktualizuje cooldown karty
    /// </summary>
    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownImage.offsetMax = new Vector2(cooldownImage.offsetMax.x, -185 * (float)((cardScript.cooldown - cooldownTimer) / cardScript.cooldown));
        }
    }

    /// <summary>
    /// Metoda vybere kartu - pro UI
    /// </summary>
    public void Select()
    {
        plantCardManager.SelectCard(cardNumber);
    }

    /// <summary>
    /// Funkce vrati, zda neni karta na cooldownu
    /// </summary>
    /// <returns>Je na cooldownu</returns>
    public bool IsAvalible()
    {
        return cooldownTimer <= 0;
    }

    /// <summary>
    /// Funkce vráti cenu rostliny
    /// </summary>
    /// <returns>Cena rostliny</returns>
    public int GetCost()
    {
        return cardScript.cost;
    }

    /// <summary>
    /// Metoda nastavi cooldown
    /// </summary>
    public PlantCardScriptableObject PlantCard()
    {
        cooldownTimer = cardScript.cooldown;
        cooldownImage.offsetMax = new Vector2(cooldownImage.offsetMax.x, 0);
        return cardScript;
    }

    /// <summary>
    /// Metoda obnovi cooldown na dokonceni cekani na kartu rostliny
    /// </summary>
    public void ResetCard()
    {
        cooldownTimer = 0;
        cooldownImage.offsetMax = new Vector2(cooldownImage.offsetMax.x, -maxTop * (float)((cardScript.cooldown - cooldownTimer) / cardScript.cooldown));
    }
    
    /// <summary>
    /// Funkce vrati, zda existuje moznost vylepseni rostliny
    /// </summary>
    /// <param name="original">Rostlina</param>
    /// <returns>Moznost vylepseni</returns>
    public bool CanUpgradeFrom(Plant original)
    {
        Plant newPlant = cardScript.plant.GetComponent<Plant>();

        Type typeArg = original.GetType();
        Type template = typeof(IUpgradeable<>);
        Type genericType = template.MakeGenericType(typeArg);

        if (genericType.IsAssignableFrom(newPlant.GetType()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
