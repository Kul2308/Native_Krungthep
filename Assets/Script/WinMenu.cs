using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    [Header("Scene names (must match your Build Profiles list)")]
    public string nextLevelScene = "Level2";   // the level AFTER this boss
    public string mainMenuScene = "MainMenu";
    public string gameScene = "Level1";        // your FIRST level (for Play Again)

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public float delayBeforeLoad = 0.15f;

    public void NextLevel()
    {
        StartCoroutine(PlayClickThenLoad(nextLevelScene));
    }

    public void PlayAgain()
    {
        StartCoroutine(PlayClickThenLoad(gameScene));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(PlayClickThenLoad(mainMenuScene));
    }

    public void QuitGame()
    {
        PlayClick();
        Application.Quit();   // only works in a built game, not in the editor
    }

    public void PlayClick()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    IEnumerator PlayClickThenLoad(string sceneName)
    {
        Time.timeScale = 1f;
        PlayClick();
        yield return new WaitForSecondsRealtime(delayBeforeLoad);
        SceneManager.LoadScene(sceneName);
    }
}