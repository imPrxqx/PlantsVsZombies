using System.Collections;
using UnityEngine;

/// <summary>
/// Trida ma na starost spawnovani slunci v urcenem oblasti
/// </summary>
public class SunManager : MonoBehaviour
{

    /// <summary>
    /// Prefab slunce, ktery bude spawnovan
    /// </summary>
    private GameObject sunPrefab { get; set; }

    /// <summary>
    /// Doba jak dlouho bude padat slunce po spawnuti
    /// </summary>
    private float timeFall { get; set; }

    /// <summary>
    /// Interval mezi generovanim slunci
    /// </summary>
    private float generationDelay { get; set; }

    /// <summary>
    /// Oblast, kde se budou spawnovat slunce
    /// </summary>
    private Vector2 position { get; set; }

    /// <summary>
    /// Delka oblasti od pozice
    /// </summary>
    private float width { get; set; }

    /// <summary>
    /// Vyska oblasti od pozice
    /// </summary>
    private float height { get; set; }

    void Awake()
    {
        sunPrefab = Resources.Load<GameObject>("Prefabs/Enviroment/Sun");
        generationDelay = 10f;
        timeFall = 6f;
    }
 
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        position = rectTransform.position;
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
    }

    /// <summary>
    /// Metoda zastavi generovani slunci a odstrani vsechny aktivni slunce ve hre
    /// </summary>
    public void StopGenerate()
    {
        StopAllCoroutines();

        GameObject[] suns = GameObject.FindGameObjectsWithTag("Sun");

        foreach (GameObject sun in suns)
        {
            Destroy(sun);
        }
    }

    /// <summary>
    /// Metoda spusti generovani slunci
    /// </summary>
    public void StartGenerate()
    {
        StartCoroutine(WaitGenerateSun());
    }

    /// <summary>
    /// Metoda vygeneruje slunce na nahodnou pozici v urcene oblasti
    /// </summary>
    private void GenerateSun()
    {
        GameObject sun = Instantiate(sunPrefab, new Vector3(Random.Range(position.x - width / 2, position.x + width / 2), Random.Range(position.y - height / 2, position.y + height / 2), sunPrefab.transform.position.z), transform.rotation);
        sun.GetComponent<Sun>().timeFall = timeFall;
    }

    /// <summary>
    /// Korutina vyckava urcitou dobu a pote spusti generovani slunci
    /// </summary>
    private IEnumerator WaitGenerateSun()
    {
        while (true)
        {
            GenerateSun();
            yield return new WaitForSeconds(generationDelay);
        }
    }
}
