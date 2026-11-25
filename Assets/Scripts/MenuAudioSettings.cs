
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
        SaveData data = SaveSystem.Load();

        musicSlider.SetValueWithoutNotify(data.musicVolume);
        sfxSlider.SetValueWithoutNotify(data.sfxVolume);
        muteToggle.SetIsOnWithoutNotify(data.muteAll);

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
        SaveSettings();
    }

    void OnMuteChanged(bool value)
    {
        ApplyVolume();
        SaveSettings();
    }

    void ApplyVolume()
    {
        ApplyVolume(mixer, musicSlider.value, sfxSlider.value, muteToggle.isOn);
    }

    internal static void ApplyVolume(AudioMixer mixer, float music, float sfx, bool mute)
    {
        mixer.SetFloat("MusicVolume", ToDecibels(mute ? 0f : music));
        mixer.SetFloat("SFXVolume", ToDecibels(mute ? 0f : sfx));
    }

    private static float ToDecibels(float volume)
    {
        return volume <= 0.001f ? -80f : Mathf.Log10(volume) * 20f;
    }

    void SaveSettings()
    {
        SaveData data = SaveSystem.Load();

        data.musicVolume = musicSlider.value;
        data.sfxVolume = sfxSlider.value;
        data.muteAll = muteToggle.isOn;

        SaveSystem.Save(data);
    }

    public void OpenSettings()
    {
        mainMenuContent.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        SaveSettings();

        settingsPanel.SetActive(false);
        mainMenuContent.SetActive(true);
    }
}
