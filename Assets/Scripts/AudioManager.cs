using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioSource[] footstepsSoundSources;

    [SerializeField] AudioSource[] objectsSoundSources;

    [SerializeField] AudioSource customSFXSource;

    [SerializeField] AudioSource bridgeMusic;

    [SerializeField] AudioSource lifeMusic;

    bool cantStartBridgeMusic;

    void Start()
    {
        instance = this;
        cantStartBridgeMusic = true;
    }

    public void PlayCustomSFX(AudioClip customSFX)
    {
        customSFXSource.Stop();
        customSFXSource.clip = customSFX;
        customSFXSource.Play();
    }

    /// <summary>
    /// 0 - grass;
    /// 1 - sand;
    /// 2 - wood;
    /// 3 - metal;
    /// </summary>
    public void PlayFootsteps(int index)
    {
        foreach (AudioSource item in footstepsSoundSources)
        {
            item.Stop();
        }

        footstepsSoundSources[index].pitch = Random.Range(0.95f, 1.05f);
        footstepsSoundSources[index].Play();
    }

    /// <summary>
    /// 0 - grass;
    /// 1 - bird fly;
    /// 2 - cat purr;
    /// 3 - cute blep;
    /// 4 - win;
    /// 5 - lose;
    /// 6 - no mood;
    /// </summary>w
    public void PlayObjectsSFX(int index, bool ignoreOthers = false)
    {
        if (ignoreOthers)
        {
            objectsSoundSources[index].Stop();
            objectsSoundSources[index].pitch = Random.Range(0.95f, 1.05f);
            objectsSoundSources[index].Play();

            return;
        }

        if (!objectsSoundSources[index].isPlaying)
        {
            objectsSoundSources[index].pitch = Random.Range(0.95f, 1.05f);
            objectsSoundSources[index].Play();
        }
    }

    public void PlayBridgeMusic()
    {
        if (cantStartBridgeMusic)
        {
            bridgeMusic.Play();
            StartCoroutine(StartFadeOutAudioSource(footstepsSoundSources[PlayerController.instance.GetCurrentPlayerFootstepsIndex()], 2f, true));
        }
    }

    public void StopBridgeMusic(bool canStartBridegAgain = true)
    {
        if (!canStartBridegAgain)
        {
            cantStartBridgeMusic = false;
        }

        footstepsSoundSources[PlayerController.instance.GetCurrentPlayerFootstepsIndex()].volume = 1f;
        StartCoroutine(StartFadeOutAudioSource(bridgeMusic, 2f));
    }

    public static IEnumerator StartFadeOutAudioSource(AudioSource audioSource, float duration, bool keepVolumeOff = false)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }

        audioSource.Stop();

        if (!keepVolumeOff)
        {
            audioSource.volume = 1f;
        }
        
        yield break;
    }

    public bool IsBridgeMusicPlay()
    {
        return bridgeMusic.isPlaying;
    }

    public void PlayLifeMusic()
    {
        lifeMusic.Play();
    }

}
