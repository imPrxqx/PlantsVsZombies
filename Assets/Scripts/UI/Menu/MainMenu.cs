using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Trida ma na starost hlavni menu hry - tlacitka
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Metoda zobrazi hru
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Level", LoadSceneMode.Single);
    }

    /// <summary>
    /// Metoda zobrazi hlavni menu
    /// </summary>
    public void Menu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);

    }

    /// <summary>
    /// Metoda zobrazi Leader Board
    /// </summary>
    public void LeaderBoard()
    {
        SceneManager.LoadScene("Leader Board", LoadSceneMode.Single);
    }

    /// <summary>
    /// Metoda vypne hru
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
