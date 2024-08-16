/// <summary>
/// Trida reprezentuje hrace
/// </summary>
public class Player
{
    /// <summary>
    /// Prezdivka hrace
    /// </summary>
    public string nickname { get; set; }

    /// <summary>
    /// Skore hrace
    /// </summary>
    public int score { get; set; }

    public Player(string nickname, int score)
    {
        this.nickname = nickname;
        this.score = score;
    }
}
