
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuAudioSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer mixer;

    [Header("Menu Panels")]
    public GameObject mainMenuContent;
    public GameObject settingsPanel;

    [Header("Sound Controls")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    void Start()
    {
        musicSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat("MusicVolume", 0.6f)
        );

        sfxSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat("SFXVolume", 0.8f)
        );

        muteToggle.SetIsOnWithoutNotify(
            PlayerPrefs.GetInt("MuteAll", 0) == 1
        );

        musicSlider.onValueChanged.AddListener(OnVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteChanged);

        ApplyVolume();

        mainMenuContent.SetActive(true);
        settingsPanel.SetActive(false);
    }

    void OnVolumeChanged(float value)
    {
        ApplyVolume();
    }

    void OnMuteChanged(bool value)
    {
        ApplyVolume();
    }

    void ApplyVolume()
    {
        float music = muteToggle.isOn ? 0f : musicSlider.value;
        float sfx = muteToggle.isOn ? 0f : sfxSlider.value;

        float musicDB = music <= 0.001f
            ? -80f : Mathf.Log10(music) * 20f;

        float sfxDB = sfx <= 0.001f
            ? -80f : Mathf.Log10(sfx) * 20f;

        mixer.SetFloat("MusicVolume", musicDB);
        mixer.SetFloat("SFXVolume", sfxDB);
    }

    public void OpenSettings()
    {
        mainMenuContent.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetInt("MuteAll", muteToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();

        settingsPanel.SetActive(false);
        mainMenuContent.SetActive(true);
    }
}
