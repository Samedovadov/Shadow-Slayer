using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class SceneMusic : MonoBehaviour
{
    private const float FadeDuration = 1f;

    private static SceneMusic player;

    private AudioSource[] channels;
    private Coroutine fade;
    private int activeChannel;

    private void Start()
    {
        if (channels != null)
        {
            return;
        }

        AudioSource sceneSource = GetComponent<AudioSource>();

        if (sceneSource.clip == null || sceneSource.outputAudioMixerGroup == null)
        {
            Debug.LogWarning(
                "SceneMusic requires a clip and a mixer group.",
                this
            );
            return;
        }

        if (player == null)
        {
            GameObject musicObject = new GameObject("Persistent Music");
            DontDestroyOnLoad(musicObject);

            player = musicObject.AddComponent<SceneMusic>();
            player.channels = new[]
            {
                musicObject.GetComponent<AudioSource>(),
                musicObject.AddComponent<AudioSource>()
            };

            foreach (AudioSource channel in player.channels)
            {
                channel.playOnAwake = false;
                channel.loop = true;
                channel.spatialBlend = 0f;
            }
        }

        player.SwitchTrack(sceneSource);
    }

    private void SwitchTrack(AudioSource sceneSource)
    {
        AudioSource outgoing = channels[activeChannel];

        if (outgoing.clip == sceneSource.clip && outgoing.isPlaying)
        {
            return;
        }

        if (fade != null)
        {
            StopCoroutine(fade);
            fade = null;
        }

        activeChannel = 1 - activeChannel;
        AudioSource incoming = channels[activeChannel];

        bool alreadyPlaying = incoming.clip == sceneSource.clip && incoming.isPlaying;
        if (!alreadyPlaying)
        {
            incoming.Stop();
            incoming.clip = sceneSource.clip;
            incoming.outputAudioMixerGroup = sceneSource.outputAudioMixerGroup;
            incoming.volume = outgoing.isPlaying ? 0f : sceneSource.volume;
            incoming.Play();
        }

        if (outgoing.isPlaying)
        {
            fade = StartCoroutine(Crossfade(outgoing, incoming, sceneSource.volume));
        }
        else
        {
            incoming.volume = sceneSource.volume;
        }
    }

    private IEnumerator Crossfade(
        AudioSource outgoing,
        AudioSource incoming,
        float targetVolume
    )
    {
        float outgoingStartVolume = outgoing.volume;
        float incomingStartVolume = incoming.volume;
        float elapsed = 0f;

        while (elapsed < FadeDuration)
        {
            yield return null;
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / FadeDuration);
            outgoing.volume = Mathf.Lerp(outgoingStartVolume, 0f, progress);
            incoming.volume = Mathf.Lerp(incomingStartVolume, targetVolume, progress);
        }

        outgoing.Stop();
        fade = null;
    }
}