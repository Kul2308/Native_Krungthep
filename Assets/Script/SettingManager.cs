using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Graphics")]
    public GameObject[] qualityTicks;   // 0 = Low, 1 = Medium, 2 = High

    [Header("Audio")]
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Graphics
        int q = PlayerPrefs.GetInt("Quality", 1);
        SetQuality(q);

        // Audio
        SetupSlider(masterSlider, "MasterVol");
        SetupSlider(musicSlider, "MusicVol");
        SetupSlider(sfxSlider, "SFXVol");
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);

        for (int i = 0; i < qualityTicks.Length; i++)
            qualityTicks[i].SetActive(i == index);
    }

    void SetupSlider(Slider slider, string param)
    {
        float v = PlayerPrefs.GetFloat(param, 1f);
        slider.SetValueWithoutNotify(v);
        ApplyVolume(param, v);
        slider.onValueChanged.AddListener(value => ApplyVolume(param, value));
    }

    void ApplyVolume(string param, float value)
    {
        mixer.SetFloat(param, Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat(param, value);
    }
}