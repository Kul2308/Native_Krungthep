using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    public static string lastLevel;

    public static void Trigger()
    {
        lastLevel = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("GameOver");
    }

    void Start() { Time.timeScale = 1f; }

    public void Retry()
    {
        if (!string.IsNullOrEmpty(lastLevel)) SceneManager.LoadScene(lastLevel);
        else SceneManager.LoadScene("Main_Menu");
    }

    public void MainMenu() { SceneManager.LoadScene("Main_Menu"); }
}