using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Trida ma na starost vlny zombiku, pocet zombiku a jejich spawnovani
/// </summary>
public class WaveManager : MonoBehaviour
{
    /// <summary>
    /// Trida reprezentuje vlnu zombiku a potrebene informace
    /// </summary>
    private class Wave
    {
        /// <summary>
        /// Pocet zombiku, kteri se maji spawnout v normalni vlne
        /// </summary>
        public int normalWaveZombieCount { get; set; }

        /// <summary>
        /// Pocet zombiku, kteri se maji spawnout v vlajkove vlne
        /// </summary>
        public int flagWaveZombieCount { get; set; }

        /// <summary>
        /// Interval spawnovani zombiku v normalni vlne
        /// </summary>
        public float normalWaveSpawnInterval { get; set; }

        /// <summary>
        /// Interval spawnovani zombiku v vlajkove vlne
        /// </summary>
        public float flagWaveSpawnInterval { get; set; }

        public Wave(int normalWaveZombieCount, int flagWaveZombieCount, float normalWaveSpawnInterval, float flagWaveSpawnInterval)
        {
            this.normalWaveZombieCount = normalWaveZombieCount;
            this.flagWaveZombieCount = flagWaveZombieCount;
            this.normalWaveSpawnInterval = normalWaveSpawnInterval;
            this.flagWaveSpawnInterval = flagWaveSpawnInterval;
        }
    }

    /// <summary>
    /// Trida reprezentuje zombie, ktery se muze vyskytnout ve hre 
    /// </summary>
    private class ZombieItem
    {
        /// <summary>
        /// Prefab zombie, ktery se ma spawnout
        /// </summary>
        public GameObject prefabZombie { get; set; }

        /// <summary>
        /// Pravdepodobnost vyskytu pri spawnuti
        /// </summary>
        public float probability { get; set; }

        public ZombieItem(GameObject prefabZombie, float probability)
        {
            this.prefabZombie = prefabZombie;
            this.probability = probability;
        }
    }

    /// <summary>
    /// Seznam pozic, ve kterych se muze spawnout zombik
    /// </summary>
    private List<GameObject> spawnPoints { get; set; }

    /// <summary>
    /// Prefab vlajky pro UI
    /// </summary>
    private GameObject flagPrefab { get; set; }

    /// <summary>
    /// UI posuvnik pro zobrazeni stavu kola
    /// </summary>
    private Slider roundSliderProgress { get; set; }

    /// <summary>
    /// Seznam aktivnich zombiku ve hre
    /// </summary>
    private List<GameObject> activeZombies { get; set; }

    /// <summary>
    /// Seznam moznych typu zombie, ktere se mohou vyskytnout ve hre
    /// </summary>
    private List<ZombieItem> zombieItems { get; set; }

    /// <summary>
    /// Seznam vln, ktere se pouziji v danem kole
    /// </summary>
    private List<Wave> waves { get; set; }

    /// <summary>
    /// Seznam aktivnich UI vlajek polozenych v posuvniku
    /// </summary>
    private List<Image> flags { get; set; }

    /// <summary>
    /// Aktualni cislo kola
    /// </summary>
    private int currentRound { get; set; }

    /// <summary>
    /// Aktualni pocet probehnutych vln v danem kole
    /// </summary>
    private int currentWaveCompleted { get; set; }

    /// <summary>
    /// Maximalni pocet vln, ktere muzou byt na danem kole
    /// </summary>
    private int maxWaveCountPerRound { get; set; }

    /// <summary>
    /// Celkovy pocet zombiku, kteri se vyskytnou v danem kole - Berou se pouze normalni vlny
    /// </summary>
    private int totalZombieCount { get; set; }

    /// <summary>
    /// Zbyvajici pocet zombiku, kteri se jeste mohou spawnout v danem kole - Berou se pouze normalni vlny
    /// </summary>
    private int remainderTotalZombieCount { get; set; }

    /// <summary>
    /// Zda byla vlajkova vlna dokoncena
    /// </summary>
    private bool isCompletedFlagWave { get; set; }

    /// <summary>
    /// Zda byla normalni vlna dokoncena
    /// </summary>
    private bool isCompletedNormalWave { get; set; }

    /// <summary>
    /// Zda je Wave Manager aktivni
    /// </summary>
    private bool isWaveManagerActive { get; set; }

    /// <summary>
    /// Delegat, ktery upozorni ostatni, ze kolo skoncilo
    /// </summary>
    public event Action OnRoundEnd;

    void Awake()
    {
        flagPrefab = Resources.Load<GameObject>("Prefabs/UI/Flag");

        currentRound = 1;
        currentWaveCompleted = 0;
        maxWaveCountPerRound = 10;
        totalZombieCount = 0;
        remainderTotalZombieCount = totalZombieCount;

        isCompletedFlagWave = true;
        isCompletedNormalWave = false;
        isWaveManagerActive = false;

        activeZombies = new List<GameObject>();
        flags = new List<Image>();
        spawnPoints = new List<GameObject>();

        zombieItems = GetDatabaseEnemy();
    }

    void Start()
    {
        roundSliderProgress = GetComponent<Slider>();

        foreach (GameObject spawnPointObject in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            spawnPoints.Add(spawnPointObject);
        }
    }

    void Update()
    {
        if (!isWaveManagerActive)
        {
            return;
        }

        UpdateSlider();

        RemoveInactiveZombies();

        if (isCompletedNormalWave && activeZombies.Count == 0)
        {
            StartFlagWave();
        }
        else if (currentWaveCompleted < waves.Count && isCompletedFlagWave)
        {
            StartNormalWave();
        }
        else if (isCompletedFlagWave && activeZombies.Count == 0 && currentWaveCompleted >= waves.Count)
        {
            EndRound();
        }
    }

    /// <summary>
    /// Metoda odstrani neaktivni zombiky ze seznamu aktivnich zombiku
    /// </summary>
    private void RemoveInactiveZombies()
    {
        for (int i = 0; i < activeZombies.Count; i++)
        {
            if (activeZombies[i] == null)
            {
                activeZombies.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Metoda spusti vlajkovou vlnu
    /// </summary>
    private void StartFlagWave()
    {
        isCompletedNormalWave = false;
        StartCoroutine(IntervalSpawnFlagWave(waves[currentWaveCompleted]));
        currentWaveCompleted++;
    }

    /// <summary>
    /// Metoda spusti normalni vlnu
    /// </summary>
    private void StartNormalWave()
    {
        isCompletedFlagWave = false;
        StartCoroutine(IntervalSpawnNormalWave(waves[currentWaveCompleted]));
    }

    /// <summary>
    /// Metoda pripravi nove kolo a nasledne ji spusti
    /// </summary>
    public void StartRound()
    {
        InitRound();
        roundSliderProgress.gameObject.SetActive(true);
        isWaveManagerActive = true;
        isCompletedFlagWave = true;
        isCompletedNormalWave = false;
    }

    /// <summary>
    /// Metoda ukonci kolo a upozorni ostatni, ze skoncilo dane kolo
    /// </summary>
    public void EndRound()
    {
        currentRound++;
        roundSliderProgress.value = 0f;
        isCompletedFlagWave = false;
        isWaveManagerActive = false;
        roundSliderProgress.gameObject.SetActive(false);
        OnRoundEnd?.Invoke();
    }

    /// <summary>
    /// Funkce vrati nahodne vybraneho zombika ze seznamu moznych typu zombiku, kteri se muzou vyskytnout ve hre
    /// </summary>
    /// <returns>Nahodny zombik</returns>
    public GameObject GetRandomZombie()
    {
        float randomValue = UnityEngine.Random.value;

        for (int i = zombieItems.Count - 1; i >= 0; i--)
        {
            if (randomValue <= zombieItems[i].probability)
            {
                return zombieItems[i].prefabZombie;
            }
        }

        return zombieItems[0].prefabZombie;
    }

    /// <summary>
    /// Funkce vrati nahodnou pozici ze seznamu pozic, ve kterych se muze spawnout zombik
    /// </summary>
    /// <returns>Nahodna pozice</returns>
    public GameObject GetRandomSpawnPoint()
    {
        int randomSpawnPoint = UnityEngine.Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomSpawnPoint];
    }

    /// <summary>
    /// Funkce vrati aktualni cislo kola
    /// </summary>
    /// <returns>Aktualni cislo kola</returns>
    public int GetCountRound()
    {
        return currentRound;
    }

    /// <summary>
    /// Metoda pripravi nove kolo, ktere se bude hrat
    /// </summary>
    private void InitRound()
    {
        currentWaveCompleted = 0;
        totalZombieCount = 0;
        UpdateProbabilities();
        CreateWaves();

        foreach (var wave in waves)
        {
            totalZombieCount += wave.normalWaveZombieCount;
        }

        remainderTotalZombieCount = totalZombieCount;
        CreateFlags();
    }

    /// <summary>
    /// Funkce vrati predem pripraveny seznam moznych typu zombiku, kteri se budou vyskytovat
    /// </summary>
    /// <returns>Seznam moznych typu zombiku</returns>
    private List<ZombieItem> GetDatabaseEnemy()
    {
        List<ZombieItem> zombieItems = new List<ZombieItem>();

        zombieItems.Add(new ZombieItem(Resources.Load<GameObject>("Prefabs/Beings/Zombies/Basic Zombie"), 0));
        zombieItems.Add(new ZombieItem(Resources.Load<GameObject>("Prefabs/Beings/Zombies/Cone Zombie"), 0));
        zombieItems.Add(new ZombieItem(Resources.Load<GameObject>("Prefabs/Beings/Zombies/Bucket Zombie"), 0));

        return zombieItems;
    }

    /// <summary>
    /// Metoda vytvori pro dane kolo vlny, ktere se budou spoustet
    /// </summary>
    private void CreateWaves()
    {
        List<Wave> newWaves = new List<Wave>();

        int totalWaves = currentRound;

        if (totalWaves >= maxWaveCountPerRound)
        {
            totalWaves = maxWaveCountPerRound;
        }

        for (int i = 0; i < totalWaves; i++)
        {
            newWaves.Add(new Wave(i + currentRound + totalWaves + 1 + 15, i + currentRound + totalWaves + 1 * (1 / 4) + 4, 10 / currentRound, 0.3f));
        }

        waves = newWaves;
    }

    /// <summary>
    /// Metoda vytvori vlajky podle poctu vln, ktere se budou spoustet 
    /// </summary>
    private void CreateFlags()
    {
        foreach (Image flag in flags)
        {
            Destroy(flag.gameObject);
        }

        flags.Clear();

        for (int i = 0; i < waves.Count; i++)
        {
            GameObject flagObject = Instantiate(flagPrefab, gameObject.transform);
            Image flagImage = flagObject.GetComponent<Image>();
            flags.Add(flagImage);
        }

        UpdateFlagsPosition();
    }

    /// <summary>
    /// Metoda nastavi vlajky na urcite pozice podle vln
    /// </summary>
    private void UpdateFlagsPosition()
    {
        RectTransform sliderRectTransform = roundSliderProgress.GetComponent<RectTransform>();
        float sliderWidth = sliderRectTransform.rect.width;

        float sliderOffset = 0f;
        for (int i = 0; i < flags.Count; i++)
        {
            sliderOffset += waves[i].normalWaveZombieCount / (float)totalZombieCount;
            float flagXPosition = sliderWidth * sliderOffset - (sliderWidth / 2);
            RectTransform flagRectTransform = flags[i].GetComponent<RectTransform>();
            flagRectTransform.anchoredPosition = new Vector2(flagXPosition, flagRectTransform.anchoredPosition.y);
        }
    }

    /// <summary>
    /// Metoda aktualizuje posuvnik podle stavu hry
    /// </summary>
    private void UpdateSlider()
    {
        roundSliderProgress.value = Mathf.MoveTowards(roundSliderProgress.value, (totalZombieCount - remainderTotalZombieCount) / (float)totalZombieCount, 0.25f * Time.deltaTime);
    }

    /// <summary>
    /// Metoda vypocita pro kazdy mozny typ zombika novou pravdepodobnost podle daneho kola - Pouziva se exponencialni rozdeleni
    /// </summary>
    private void UpdateProbabilities()
    {
        float lambda = (1f / (currentRound * 0.5f));

        for (int i = 0; i < zombieItems.Count; i++)
        {
            zombieItems[i].probability = Mathf.Exp(-lambda * i);
        }
    }

    /// <summary>
    /// Korutina pro vybranou vlajkovou vlnu postupne spawnuje zombiky do hry
    /// </summary>
    /// <param name="wave">Vlna, ktera je na rade</param>
    private IEnumerator IntervalSpawnFlagWave(Wave wave)
    {
        while (wave.flagWaveZombieCount > 0)
        {
            yield return new WaitForSeconds(wave.flagWaveSpawnInterval);
            GameObject spawnpoint = GetRandomSpawnPoint();
            GameObject zombie = GetRandomZombie();
            wave.flagWaveZombieCount -= 1;
            activeZombies.Add(Instantiate(zombie, new Vector3(spawnpoint.transform.position.x, spawnpoint.transform.position.y, zombie.transform.position.z), Quaternion.identity));
        }

        isCompletedFlagWave = true;
    }

    /// <summary>
    /// Korutina pro vybranou normalni vlnu postupne spawnuje zombiky do hry
    /// </summary>
    /// <param name="wave">Vlna, ktera je na rade</param>
    private IEnumerator IntervalSpawnNormalWave(Wave wave)
    {
        while (wave.normalWaveZombieCount > 0)
        {
            yield return new WaitForSeconds(wave.normalWaveSpawnInterval);
            GameObject spawnpoint = GetRandomSpawnPoint();
            GameObject zombie = GetRandomZombie();
            wave.normalWaveZombieCount -= 1;
            remainderTotalZombieCount--;
            activeZombies.Add(Instantiate(zombie, new Vector3(spawnpoint.transform.position.x, spawnpoint.transform.position.y, zombie.transform.position.z), Quaternion.identity));
        }

        isCompletedNormalWave = true;
    }
}
