using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Trida ma na starost stav hru a komunikace mezi ostatnimi managery
/// </summary>
public class GameManager : MonoBehaviour
{

    /// <summary>
    /// Reference na Plant Card Manager 
    /// </summary>
    public PlantCardManager plantCardManager { get; set; }

    /// <summary>
    /// Reference na Deat Manager 
    /// </summary>
    public DeathManager deathManager { get; set; }

    /// <summary>
    /// Reference na Wave Manager 
    /// </summary>
    public WaveManager waveManager { get; set; }

    /// <summary>
    /// Reference na Sun Manager 
    /// </summary>
    public SunManager sunManager { get; set; }

    /// <summary>
    /// Reference na Card Selection Manager 
    /// </summary>
    public CardSelectionManager cardSelectionManager { get; set; }

    /// <summary>
    /// Reference na Hand Manager 
    /// </summary>
    public HandManager handManager { get; set; }

    /// <summary>
    /// Reference na vsechny Plant Slot 
    /// </summary>
    public PlantSlot[] plantSlots { get; set; }

    /// <summary>
    /// Reference na vsechny Lawnmower
    /// </summary>
    public Lawnmower[] lawnmowers { get; set; }

    void Start()
    {
        plantCardManager = FindObjectOfType<PlantCardManager>();

        waveManager = FindObjectOfType<WaveManager>();
        waveManager.OnRoundEnd += EndRound;

        sunManager = FindObjectOfType<SunManager>();
        cardSelectionManager = FindObjectOfType<CardSelectionManager>();
        deathManager = FindObjectOfType<DeathManager>();
        deathManager.OnPlayerDeath += StopGame;

        handManager = FindObjectOfType<HandManager>();

        plantSlots = FindObjectsOfType<PlantSlot>();
        lawnmowers = FindObjectsOfType<Lawnmower>();

        StartGame();
    }
  
    /// <summary>
    /// Metoda spusti hru 
    /// </summary>
    public void StartGame()
    {
        foreach (var slot in plantSlots)
        {
            slot.StopShow();
        }

        cardSelectionManager.StartSelect();
        handManager.StopHand();
    }

    /// <summary>
    /// Metoda zapne nove kolo
    /// </summary>
    public void StartRound()
    {

        if (plantCardManager.slots.Count() <= 0)
        {
            return;
        }

        foreach (var slot in plantSlots)
        {
            slot.StartShow();
        }

        cardSelectionManager.StopSelect();
        sunManager.StartGenerate();
        waveManager.StartRound();
        handManager.StartHand();
    }

    /// <summary>
    /// Metoda ukonci predchozi kolo
    /// </summary>
    public void EndRound()
    {
        foreach (var slot in plantSlots)
        {
            slot.StopShow();
        }

        foreach (var lawnmower in lawnmowers)
        {
            if(lawnmower == null)
            {
                continue;
            }

            if(lawnmower.activated == true)
            {
                Destroy(lawnmower.gameObject);
            }
        }

        cardSelectionManager.StartSelect();
        sunManager.StopGenerate();
        handManager.StopHand();
        plantCardManager.ResetCooldowns();

        Projectile[] projectiles = FindObjectsOfType<Projectile>();

        foreach (Projectile projectile in FindObjectsOfType<Projectile>())
        {
            Destroy(projectile.gameObject);
        }
    }

    /// <summary>
    /// Metoda ukonci hru
    /// </summary>
    private void StopGame()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Leader Board", LoadSceneMode.Single);

    }

    /// <summary>
    /// Metoda spusti po skonceni hry zobrazeni hodnoceni hracu
    /// </summary>
    /// <param name="scene">Scena</param>
    /// <param name="mode">Mode nacitani</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Leader Board")
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            GameObject leaderboardPrefab = Resources.Load<GameObject>("Prefabs/UI/Add Player");
            GameObject player = Instantiate(leaderboardPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
            player.GetComponent<AddPlayer>().player = new Player("", 0);
            player.GetComponent<AddPlayer>().player.score = plantCardManager.GetCountSun() * waveManager.GetCountRound();

        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
