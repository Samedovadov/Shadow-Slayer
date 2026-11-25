using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class SoundEffects : MonoBehaviour
{
    public enum Cue
    {
        Jump,
        WindUp,
        Death,
        Click,
        Win,
        Lose
    }

    private const int SampleRate = 22050;
    private static SoundEffects instance;

    private readonly Dictionary<Cue, AudioClip[]> clips = new();
    private AudioSource source;
    private AudioSource[] variedSources;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        AudioMixer mixer = Resources.Load<AudioMixer>("ShadowSlayerMixer");
        if (mixer == null)
        {
            Debug.LogError("SoundEffects: ShadowSlayerMixer is missing.");
            return;
        }

        AudioMixerGroup[] groups = mixer.FindMatchingGroups("SFX");
        if (groups.Length == 0)
        {
            Debug.LogWarning("SoundEffects: SFX mixer group is missing.");
            return;
        }

        GameObject host = new GameObject("Persistent Sound Effects");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<SoundEffects>();
        instance.source = instance.AddSource(groups[0], 1f);
        instance.variedSources = new[]
        {
            instance.AddSource(groups[0], 0.92f),
            instance.AddSource(groups[0], 1.08f)
        };

        SceneManager.sceneLoaded += instance.OnSceneLoaded;
    }

    private void Start()
    {
        SaveData settings = SaveSystem.Load();
        MenuAudioSettings.ApplyVolume(
            source.outputAudioMixerGroup.audioMixer,
            settings.musicVolume, settings.sfxVolume, settings.muteAll
        );
    }

    private AudioSource AddSource(AudioMixerGroup group, float pitch)
    {
        AudioSource channel = gameObject.AddComponent<AudioSource>();
        channel.playOnAwake = false;
        channel.spatialBlend = 0f;
        channel.outputAudioMixerGroup = group;
        channel.pitch = pitch;
        return channel;
    }

    public static void Play(Cue cue)
    {
        if (instance == null)
        {
            return;
        }

        AudioClip[] variants = instance.GetClips(cue);
        instance.source.PlayOneShot(variants[Random.Range(0, variants.Length)]);
    }

    public static void PlayVaried(AudioClip clip, float volume = 1f)
    {
        if (instance == null || clip == null)
            return;

        AudioSource[] channels = instance.variedSources;
        channels[Random.Range(0, channels.Length)].PlayOneShot(clip, volume);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                button.onClick.RemoveListener(PlayClick);
                button.onClick.AddListener(PlayClick);
            }
        }

        if (scene.name == "WinScene")
        {
            Play(Cue.Win);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (instance == this)
        {
            instance = null;
        }
    }

    private static void PlayClick()
    {
        Play(Cue.Click);
    }

    private AudioClip[] GetClips(Cue cue)
    {
        if (clips.TryGetValue(cue, out AudioClip[] variants))
        {
            return variants;
        }

        variants = new[]
        {
            CreateClip(cue, 0.94f),
            CreateClip(cue, 1.06f)
        };

        clips.Add(cue, variants);
        return variants;
    }

    private static AudioClip CreateClip(Cue cue, float pitch)
    {
        (float start, float end, float duration) = cue switch
        {
            Cue.Jump => (290f, 760f, 0.13f),
            Cue.WindUp => (170f, 530f, 0.22f),
            Cue.Death => (420f, 75f, 0.42f),
            Cue.Click => (1050f, 700f, 0.055f),
            Cue.Win => (440f, 880f, 0.65f),
            _ => (280f, 105f, 0.65f)
        };

        float[] samples = new float[Mathf.CeilToInt(duration * SampleRate)];
        float phase = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            float progress = (float)i / (samples.Length - 1);
            float frequency = Mathf.Lerp(start, end, progress) * pitch;
            phase += 2f * Mathf.PI * frequency / SampleRate;

            float envelope = Mathf.Min(progress * 35f, 1f) * (1f - progress);
            samples[i] = envelope * 0.28f
                * (Mathf.Sin(phase) + 0.2f * Mathf.Sin(phase * 2f));
        }

        AudioClip clip = AudioClip.Create(
            cue.ToString(), samples.Length, 1, SampleRate, false
        );

        clip.SetData(samples, 0);
        return clip;
    }
}
