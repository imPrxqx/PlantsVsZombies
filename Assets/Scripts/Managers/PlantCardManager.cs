using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

/// <summary>
/// Trida ma na starost spravu karet s rostlinami
/// </summary>
public class PlantCardManager : MonoBehaviour
{

    /// <summary>
    /// Seznam vybranych karet s rostlinami
    /// </summary>
    private List<GameObject> slotsGOs { get; set; }

    /// <summary>
    /// Seznam s CardSloty vybranych karet rostlin
    /// </summary>
    public List<CardSlot> slots { get; set; }

    /// <summary>
    /// Text pro zobrazeni ceny pro UI
    /// </summary>
    private TextMeshProUGUI sunText { get; set; }

    /// <summary>
    /// Misto, kde se maji vytvaret nove karty, pri vybirani karet
    /// </summary>
    private GameObject slotParent { get; set; }

    /// <summary>
    /// Prefab karty, kam se budou davat informace
    /// </summary>
    private GameObject slotPrefab { get; set; }

    /// <summary>
    /// Maximalni pocet karet, ktere lze mit
    /// </summary>
    private int plantLimit { get; set; }

    /// <summary>
    /// Aktualne vybrana karta
    /// </summary>
    private int selectedCard { get; set; }

    /// <summary>
    /// Aktualni pocet slunci
    /// </summary>
    private int suns { get; set; }

    /// <summary>
    /// Delegat, ktery upozorni ostatni, ze karta byla vybrana do ruky
    /// </summary>
    public event Action OnPlayerSelect;
    
    void Awake()
    {
        slots = new List<CardSlot>();
        slotsGOs = new List<GameObject>();

        selectedCard = -1;
        plantLimit = 5;
        suns = 50;
        slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Card Slot");

    }

    void Start()
    {
        sunText = GameObject.FindWithTag("Sun Count").GetComponent<TextMeshProUGUI>();
        sunText.text = suns.ToString();

        slotParent = GameObject.FindWithTag("Card Slot Panel");
    }

    /// <summary>
    /// Metoda prida pocet slunci hracovi
    /// </summary>
    public void AddSun(int amount)
    {
        suns += amount;
        sunText.text = suns.ToString();
    }

    /// <summary>
    /// Metoda vybere kartu podle poradoveho cisla
    /// </summary>
    public void SelectCard(int num)
    {
        OnPlayerSelect.Invoke();

        if (selectedCard == num || num == -1 || !slots[num].IsAvalible() || slots[num].GetCost() > suns)
        {
            selectedCard = -1;
        }
        else
        {
            selectedCard = num;
        }
    }

    /// <summary>
    /// Funkce vrati aktualni pocet slunci
    /// </summary>
    /// <returns>Pocet slunci</returns>
    public int GetCountSun()
    {
        return suns;
    }

    /// <summary>
    /// Funkce vrati aktualni index vybrane karty
    /// </summary>
    /// <returns>Index vybrane karty</returns>
    public int GetSelectedIndex()
    {
        return selectedCard;
    }

    /// <summary>
    /// Funkce vrati rostlinu, ktera se pouzije na zasazeni
    /// </summary>
    /// <param name="ocupant">Rostlina</param>
    /// <returns>Rostlina, ktera se pouzije</returns>
    public PlantCardScriptableObject PlantCard(Plant ocupant)
    {
        if (selectedCard == -1)
        {
            return null;
        }

        if (slots[selectedCard].IsAvalible())
        {
            bool isUpgrade = slots[selectedCard].cardScript.plant.GetComponent<Plant>().GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IUpgradeable<>));

            if (isUpgrade && ocupant == null)
            {
                return null;
            }

            if (ocupant != null)
            {
                if (!isUpgrade)
                    return null;

                if (!slots[selectedCard].CanUpgradeFrom(ocupant))
                    return null;
            }

            int cost = slots[selectedCard].GetCost();
            if (suns >= cost)
            {
                suns -= cost;
                sunText.text = suns.ToString();
                return slots[selectedCard].PlantCard();
            }
            else
            {
                OnPlayerSelect.Invoke();
                selectedCard = -1;
            }
        }
        return null;
    }

    /// <summary>
    /// Metoda prida kartu do seznamu vybranych karet
    /// </summary>
    public bool AddPlant(PlantCardScriptableObject plant)
    {
        if (plantLimit <= slots.Count())
        {
            return false;
        }
        foreach (CardSlot slot in slots)
        {
            if (slot.cardScript == plant)
            {
                return false;
            }
        }

        GameObject slotGO = GameObject.Instantiate(slotPrefab, slotParent.transform);
        CardSlot newSlot = slotGO.GetComponent<CardSlot>();
        slotsGOs.Add(slotGO);
        slots.Add(newSlot);
        newSlot.Init(plant, this, slots.Count() - 1);
        return true;
    }

    /// <summary>
    /// Metoda odstrani kartu ze seznamu vybranych karet
    /// </summary>
    public bool RemovePlant(int slotNum)
    {
        if (slots.Count() > slotNum)
        {
            slots.RemoveAt(slotNum);
            Destroy(slotsGOs[slotNum]);
            slotsGOs.RemoveAt(slotNum);
            for (int i = slotNum; i < slots.Count(); i++)
            {
                slots[i].cardNumber--;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Metoda obnovi vsechny cooldowny karet
    /// </summary>
    public void ResetCooldowns()
    {
        foreach (var slot in slots)
        {
            slot.ResetCard();
        }
    }
}
