using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Trida ma na starost zobrazovani hodnoceni hracu, pridavani a nacteni hracu a jejich vyhledavani
/// </summary>
public class LeaderboardManager : MonoBehaviour
{

    /// <summary>
    /// Prefab hrace, ktery se bude zobrazovat
    /// </summary>
    private GameObject prefabPlayer { get; set; }

    /// <summary>
    /// Misto, kde se maji pridavat hodnoceni hracu
    /// </summary>
    private GameObject contentPlace { get; set; }

    /// <summary>
    /// Misto, kde se ma zobrazovat seznam sloupcu
    /// </summary>
    private GameObject columnsPlace { get; set; }

    /// <summary>
    /// Input vstup, ve kterem lze vyhledavat hrace
    /// </summary>
    private TMP_InputField inputField { get; set; }

    /// <summary>
    /// Seznam hracu v Leader Boardu
    /// </summary>
    private List<Player> players { get; set; }

    /// <summary>
    /// Seznam vytvorenych prefab hracu a moznost jeho opetovnemu pouziti
    /// </summary>
    private List<GameObject> playersPool { get; set; }

    /// <summary>
    /// Maximalni pocet hracu, kteri mohou byt v Leader Boardu
    /// </summary>
    private int maxPlayersCount { get; set; }

    /// <summary>
    /// Oznaceny hrac v Leader Boardu, ktery je nove pridany
    /// </summary>
    private Player markedPlayer { get; set; }

    /// <summary>
    /// Cesta k json souboru, kde jsou ulozeny hodnoceni hracu
    /// </summary>
    private string filePath { get; set; }


    void Awake()
    {
        filePath = Path.Combine(Application.streamingAssetsPath, "players.json");
        inputField = GetComponent<TMP_InputField>();
        inputField.onValueChanged.AddListener(OnInputChanged);
        prefabPlayer = Resources.Load<GameObject>("Prefabs/UI/Player");
        contentPlace = GameObject.FindWithTag("Content");
        columnsPlace = GameObject.FindWithTag("Columns");
        playersPool = new List<GameObject>();
        maxPlayersCount = 500;
        markedPlayer = null;
        LoadData();
        OnInputChanged("");

        List<string> columnsName = GetColumnsName<Player>();
        DisplayColumns(columnsName);
    }

    /// <summary>
    /// Metoda nacte data hracu ze souboru a ulozi je do seznamu hracu. Pokud nastane nejaka chyba, vytvori se defaultni data a ty se pouziji
    /// </summary>
    private void LoadData()
    {
        try
        {
            string json = File.ReadAllText(filePath);

            List<Player> loadedPlayers = JsonConvert.DeserializeObject<List<Player>>(json);

            if (loadedPlayers == null || !loadedPlayers.Any())
            {
                throw new Exception();
            }

            players = loadedPlayers;
            CheckIntegrity();

        }
        catch
        {
            players = new List<Player>
            {
                new Player("Alice", 1500),
                new Player("Bob", 2500),
                new Player("Charlie", 1000),
                new Player("David", 3000)
            };

            Save();
        }
    }

    /// <summary>
    /// Metoda ulozi noveho hrace do seznamu hracu, aktualizuje soubor a oznaci ho v Leader Boardu
    /// Specialni pripad je, ze oznaceny hrac se nemusi vubec dostat do Leader Boardu, protoze muze mit velice nizke skore - coz je dobre
    /// </summary>
    /// <param name="newPlayer">Novy hrac</param>
    public void SaveData(Player newPlayer)
    {
        players.Add(newPlayer);
        markedPlayer = newPlayer;
        CheckIntegrity();
        OnInputChanged("");
    }

    /// <summary>
    /// Metoda zkontroluje spravny pocet hracu, pripadne odstrani hrace s nejnizsim skorem 
    /// </summary>
    private void CheckIntegrity()
    {
        while (players.Count > maxPlayersCount)
        {
            Player lowestScorePlayer = players.OrderBy(player => player.score).FirstOrDefault();
            
            if(lowestScorePlayer == markedPlayer)
            {
                markedPlayer = null;
            }

            players.Remove(lowestScorePlayer);
        }

        Save();
    }

    /// <summary>
    /// Metoda ulozi seznam hracu do souboru json
    /// </summary>
    private void Save()
    {
        string json = JsonConvert.SerializeObject(players, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Metoda aktualizuje Leader Board zobrazovani hracu podle input vstupu
    /// </summary>
    /// <param name="inputText">Text zadany do vstupniho pole</param>
    private void OnInputChanged(string inputText)
    {
        List<Expression> customExpression = GetCustomExpression<Player>(inputText);

        List<Expression> Expressions = GetSpecialExpressionsFromExpressions<Player>(customExpression);
        List<Func<Player, bool>> filters = GetFinalQueryFromExpressions(Expressions);

        DisplayPlayers(filters, inputText);
    }

    /// <summary>
    /// Metoda zobrazi nazvy sloupcu v Leader Boardu
    /// </summary>
    /// <param name="columnsName">Seznam nazvu sloupcu</param>
    private void DisplayColumns(List<string> columnsName)
    {
        GameObject newPlayer = Instantiate(prefabPlayer, columnsPlace.transform);

        for (int i = 0; i < columnsName.Count; i++)
        {
            newPlayer.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = columnsName[i];
        }
    }

    /// <summary>
    /// Metoda zobrazi hrace na zaklade zadanych filtrovacich vyrazu za pomoci delegatu
    /// </summary>
    /// <param name="filters">Seznam filtrovacich vyrazu pro vyhledavani hracu</param>
    /// <param name="inputText">Textovy vstup pro vyhledavani</param>
    private void DisplayPlayers(List<Func<Player, bool>> filters, string inputText)
    {
        List<Player> filteredPlayers = new List<Player>();

        if (filters.Count > 0)
        {
            filteredPlayers = players.Where(player => filters.All(filter => filter(player))).OrderByDescending(player => player.score).ToList();
        }
        else if (inputText.Length != 0)
        {
            filteredPlayers = players.Where(player => player.nickname.IndexOf(inputText, StringComparison.OrdinalIgnoreCase) >= 0).OrderByDescending(player => player.score).ToList();
        }
        else
        {
            filteredPlayers = players.OrderByDescending(player => player.score).ToList();
        }

        foreach (GameObject child in playersPool)
        {
            child.SetActive(false);
        }

        for (int i = 0; i < filteredPlayers.Count; i++)
        {

            if (i >= playersPool.Count)
            {
                GameObject newPlayer = Instantiate(prefabPlayer, contentPlace.transform);
                newPlayer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = filteredPlayers[i].nickname;
                newPlayer.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filteredPlayers[i].score.ToString();
                playersPool.Add(newPlayer);
            }
            else
            {
                playersPool[i].SetActive(true);
                playersPool[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = filteredPlayers[i].nickname;
                playersPool[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filteredPlayers[i].score.ToString();
            }

            Image image = playersPool[i].GetComponent<Image>();

            if (markedPlayer != null && markedPlayer == filteredPlayers[i])
            {
                image.color = Color.yellow;
            }
            else
            {
                image.color = Color.magenta;
            }
        }

    }

    /// <summary>
    /// Funkce vraci nazvy sloupcu z vlastnosti typu T - pouziti reflexe
    /// </summary>
    /// <typeparam name="T">Typ objektu, jehoz vlastnosti se maji pouzit</typeparam>
    /// <returns>Seznam nazvu sloupcu</returns>
    private List<string> GetColumnsName<T>()
    {
        List<string> columnsList;
        columnsList = typeof(T).GetProperties().Select(property => property.Name).ToList();
        return columnsList;
    }

    /// <summary>
    /// Funkce vraci vlastni filtrovaci vyrazy na zaklade zadaneho vstupu
    /// </summary>
    /// <typeparam name="T">Typ objektu, ktery se filtruje</typeparam>
    /// <param name="query">Dotaz pro filtrovani</param>
    /// <returns>Seznam filtru</returns>
    private List<Expression> GetCustomExpression<T>(string input)
    {
        List<Expression> customFilters = new List<Expression>();
        char splitterPart = ';';
        char splitterExpressionPart = ':';

        string[] parts = input.Split(splitterPart);

        foreach (var part in parts)
        {
            if (part.Length == 0 || part.Contains(splitterExpressionPart) == false)
            {
                continue;
            }

            string[] Expression = part.Split(splitterExpressionPart);
            string column = Expression[0].ToLower().Trim(' ');
            string value = Expression[1].ToLower().Trim(' ');

            if (column.Length == 0 || value.Length == 0 || typeof(T).GetProperty(column) == null)
            {
                continue;
            }

            customFilters.Add(new Expression(column, value, ExpressionType.Default));
        }

        return customFilters;
    }

    /// <summary>
    /// Funkce vraci nove specialni filtrovaci vyrazy za stare
    /// </summary>
    /// <typeparam name="T">Typ objektu, ktery se filtruje</typeparam>
    /// <param name="Expressions">Seznam puvodnich filtrovacich vyrazu</param>
    /// <returns>Seznam novych filtrovacich vyrazu</returns>
    private List<Expression> GetSpecialExpressionsFromExpressions<T>(List<Expression> Expressions)
    {
        List<Expression> newExpressions = new List<Expression>();

        foreach (Expression Expression in Expressions)
        {
            if (typeof(T).GetProperty(Expression.column).PropertyType == typeof(int))
            {
                string value = Expression.value;

                if (TryAddIntervalExpression(Expression, value, newExpressions, "-", ExpressionType.Interval))
                {
                    continue;
                }

                if (TryAddComparisonExpression(Expression, value, newExpressions, "<=", ExpressionType.LessEqual))
                {
                    continue;
                }

                if (TryAddComparisonExpression(Expression, value, newExpressions, ">=", ExpressionType.GreaterEqual))
                {
                    continue;
                }

                if (TryAddComparisonExpression(Expression, value, newExpressions, "<", ExpressionType.Less))
                {
                    continue;
                }

                if (TryAddComparisonExpression(Expression, value, newExpressions, ">", ExpressionType.Greater))
                {
                    continue;
                }
            }

            newExpressions.Add(Expression);
        }

        return newExpressions;
    }

    /// <summary>
    /// Funkce vrati zda se jedna o intervalovy filtrovaci vyraz
    /// </summary>
    /// <param name="Expression">Filtrovaci vyraz</param>
    /// <param name="value">Hodnota vyrazu</param>
    /// <param name="Expressions">Seznam novych filtrovacich vyrazu</param>
    /// <param name="operatorType">Typ filtrovaciho vyraz pro interval</param>
    /// <param name="ExpressionType">Typ filtrovaciho vyrazu</param>
    /// <returns>Je intervalovy filtrovaci vyraz</returns>
    private bool TryAddIntervalExpression(Expression Expression, string value, List<Expression> Expressions, string operatorType, ExpressionType expressionType)
    {
        if (value.Contains(operatorType) == false)
        {
            return false;
        }

        string[] keyValue = value.Split(operatorType);

        if (keyValue.Length == 2 && int.TryParse(keyValue[0], out _) && int.TryParse(keyValue[1], out _))
        {
            Expressions.Add(new Expression(Expression.column, keyValue[0] + " " + keyValue[1], expressionType));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Funkce vrati zda se jedna o porovnavaci filtrovaci vyraz
    /// </summary>
    /// <param name="Expression">Filtrovaci vyraz</param>
    /// <param name="value">Hodnota vyrazu</param>
    /// <param name="Expressions">Seznam novych filtrovacich vyrazu</param>
    /// <param name="operatorType">Typ filtrovaciho vyraz pro interval</param>
    /// <param name="ExpressionType">Typ filtrovaciho vyrazu</param>
    /// <returns>Je porovnavaci filtrovaci vyraz</returns>
    private bool TryAddComparisonExpression(Expression Expression, string value, List<Expression> Expressions, string operatorType, ExpressionType expressionType)
    {
        if (value.Contains(operatorType) == false)
        {
            return false;
        }

        string[] keyValue = value.Split(operatorType);

        if (keyValue.Length == 2 && int.TryParse(keyValue[1], out _))
        {
            Expressions.Add(new Expression(Expression.column, keyValue[1], expressionType));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Funkce vytvari finalni filtrovaci vyraz na zaklade seznamu filtrovacich vyrazu
    /// </summary>
    /// <param name="Expressions">Seznam filtrovacich vyrazu pro seznam finalnich filtrovacich vyraz</param>
    /// <returns>Seznam finalnich filtrovacich vyrazu</returns>
    private List<Func<Player, bool>> GetFinalQueryFromExpressions(List<Expression> Expressions)
    {
        List<Func<Player, bool>> filters = new List<Func<Player, bool>>();

        foreach (Expression Expression in Expressions)
        {
            Func<Player, bool> queryExpression = CreateQueryFromExpression(Expression);

            if (queryExpression != null)
            {
                filters.Add(queryExpression);
            }
        }

        return filters;
    }

    /// <summary>
    /// Funkce vrati delegat na zaklade filtracniho vyrazu
    /// </summary>
    /// <param name="Expression">Filtrovaci vyraz</param>
    /// <returns>Dotaz jako delegat pro filtraci hracu</returns>
    private Func<Player, bool> CreateQueryFromExpression(Expression Expression)
    {
        switch (Expression.expressionType)
        {
            case ExpressionType.Greater:
                return player => CompareProperty(player, Expression.column, Expression.value, (propValue, condValue) => propValue > condValue);
            case ExpressionType.GreaterEqual:
                return player => CompareProperty(player, Expression.column, Expression.value, (propValue, condValue) => propValue >= condValue);
            case ExpressionType.Less:
                return player => CompareProperty(player, Expression.column, Expression.value, (propValue, condValue) => propValue < condValue);
            case ExpressionType.LessEqual:
                return player => CompareProperty(player, Expression.column, Expression.value, (propValue, condValue) => propValue <= condValue);
            case ExpressionType.Interval:
                return player => {
                    string[] keyValue = Expression.value.Split(' ');
                    int lowerValue = int.Parse(keyValue[0]);
                    int upperValue = int.Parse(keyValue[1]);
                    int propValue = Convert.ToInt32(typeof(Player).GetProperty(Expression.column).GetValue(player));
                    return propValue >= lowerValue && propValue <= upperValue;
                };
            case ExpressionType.Default:
                return player => {
                    string propertyValue = typeof(Player).GetProperty(Expression.column).GetValue(player)?.ToString().ToLower();
                    string ExpressionValue = Expression.value.ToString().ToLower();
                    return propertyValue != null && propertyValue.Contains(ExpressionValue);
                };
            default:
                throw new Exception();
        }
    }

    /// <summary>
    /// Funkce porovnava hodnotu vlastnosti hrace s filtracni hodnotou - Jedna se podstate o delegata, ktery se vraci
    /// </summary>
    /// <param name="player">Hrac, jehoz vlastnost se porovnava</param>
    /// <param name="column">Nazev sloupce, ktery se porovnava</param>
    /// <param name="value">Hodnota filtru pro porovnani</param>
    /// <param name="comparison">Funkce pro porovnani hodnot</param>
    /// <returns>Vysledek porovnani</returns>
    private bool CompareProperty(Player player, string column, string value, Func<int, int, bool> comparison)
    {
        int propertyValue = Convert.ToInt32(typeof(Player).GetProperty(column).GetValue(player));
        int ExpressionValue = Convert.ToInt32(value);
        return comparison(propertyValue, ExpressionValue);
    }
}

/// <summary>
/// Trida reprezentuje filtrovaci vyraz pro vyhledavani
/// </summary>
public class Expression
{
    /// <summary>
    /// Nazev sloupce
    /// </summary>
    public string column { get; set; }

    /// <summary>
    /// Hodnota v sloupci
    /// </summary>
    public string value { get; set; }

    /// <summary>
    /// Typ filtrace
    /// </summary>
    public ExpressionType expressionType { get; set; }


    public Expression(string column, string value, ExpressionType expressionType)
    {
        this.column = column;
        this.value = value;
        this.expressionType = expressionType;
    }
}

/// <summary>
/// Typy filtrovacich vyrazu pro vyhledavani
/// </summary>
public enum ExpressionType
{
    /// <summary>
    /// Vychozi filtrovaci vyraz, ktery hleda hledany string
    /// </summary>
    Default,

    /// <summary>
    /// Filtrovaci vyraz, ktery hleda ciselne hodnoty mensi nez zadana hodnota
    /// </summary>
    Less,

    /// <summary>
    /// Filtrovaci vyraz, ktery hleda ciselne hodnoty vetsi nez zadana hodnota
    /// </summary>
    Greater,

    /// <summary>
    /// Filtrovaci vyraz, ktery hleda ciselne hodnoty mensi nebo rovno nez zadana hodnota
    /// </summary>
    LessEqual,

    /// <summary>
    /// Filtrovaci vyraz, ktery hleda ciselne hodnoty vetsi nebo rovno nez zadana hodnota
    /// </summary>
    GreaterEqual,

    /// <summary>
    /// Filtrovaci vyraz, ktery hleda ciselne hodnoty v rozmezi dvou hodnot
    /// </summary>
    Interval
}