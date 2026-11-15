using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    public AudioSource source;   // the ClickSound object
    public AudioClip clip;       // the click sound file

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (source != null && clip != null) source.PlayOneShot(clip);
        });
    }
}