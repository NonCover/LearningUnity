using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static AudioManager current;

    [Header("环境声音")]
    public AudioClip ambientClip;
    public AudioClip MusicClip;

    [Header("FX")]
    public AudioClip deathFXClip;
    public AudioClip orbFXClip;
    public AudioClip doorFXClip;

    [Header("Robbie音效")]
    public AudioClip[] walkStepClips;
    public AudioClip[] crouchStepClips;

    public AudioClip jumpClip;
    public AudioClip jumpVoiceClip;

    public AudioClip deathClip;
    public AudioClip deathVoiceClip;

    public AudioClip orbVoiceClip;

    AudioSource ambientSource;
    AudioSource musicSource;
    AudioSource fxSource;
    AudioSource playSource;
    AudioSource voiceSource;

    private void Awake()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }
        current = this;

        DontDestroyOnLoad(gameObject);

        ambientSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playSource = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();

        StartLevelAudio();
    }

    void StartLevelAudio() // 开启环境音效
    {
        current.ambientSource.clip = current.ambientClip;
        current.ambientSource.loop = true;
        current.ambientSource.Play();

        current.musicSource.clip = current.MusicClip;
        current.musicSource.loop = true;
        current.musicSource.Play();
    }

    public static void PlayFootStepAudio()
    {
        int index = Random.Range(0,current.walkStepClips.Length);
        current.playSource.clip = current.walkStepClips[index];
        current.playSource.Play();
    }

    public static void PlayCrouchFootstepAudio()
    {
        int index = Random.Range(0, current.crouchStepClips.Length);
        current.playSource.clip = current.crouchStepClips[index];
        current.playSource.Play();
    }

    public static void PlayJumpAudio()
    {
        current.playSource.clip= current.jumpClip;
        current.playSource.Play();

        current.voiceSource.clip = current.jumpVoiceClip;
        current.voiceSource.Play();

    }

    public static void PlayDeathAudio()
    {
        current.playSource.clip = current.deathClip; 
        current.playSource.Play();

        current.voiceSource.clip = current.deathVoiceClip;
        current.voiceSource.Play();

        current.fxSource.clip = current.deathFXClip;
        current.fxSource.Play();
    }

    public static void PlayOrbAudio()
    {
        current.fxSource.clip = current.orbFXClip; 
        current.fxSource.Play();

        current.voiceSource.clip = current.orbVoiceClip;
        current.voiceSource.Play();
    }

    public static void PlayDoorOpenAudio()
    {
        current.fxSource.clip = current.doorFXClip; 
        current.fxSource.PlayDelayed(1.1f);
    }
}
