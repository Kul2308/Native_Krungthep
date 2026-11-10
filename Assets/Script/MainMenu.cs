using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame() { SceneManager.LoadScene("SampleScene"); }
    public GameObject optionsPanel;

    public void OpenOptions() { optionsPanel.SetActive(true); }
}