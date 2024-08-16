using UnityEngine;
using TMPro;

/// <summary>
/// Trida ma na starost pridani hrace do Leader Boardu
/// </summary>
public class AddPlayer : MonoBehaviour
{
    /// <summary>
    /// Hrac, ktery ma byt pridan do Leader Boardu
    /// </summary>
    public Player player {  get; set; }

    /// <summary>
    /// Metoda prida noveho hrace do Leader Boardu
    /// </summary>
    public void AddPlayerToLeaderBoard()
    {
        player.nickname = GetComponent<TMP_InputField>().text;

        if(player.nickname.Length == 0)
        {
            return;
        }

        LeaderboardManager leaderboard = FindObjectOfType<LeaderboardManager>();

        leaderboard.SaveData(player);
        Destroy(gameObject);
    }

    /// <summary>
    /// Metoda zrusi pridavani noveho hrace do Leader Boardu 
    /// </summary>
    public void CancelAddPlayerToLeaderBoard()
    {
        Destroy(gameObject);    
    }
}