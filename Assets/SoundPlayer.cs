using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;


[Serializable]
public enum Sound
{
    PLAYER_DAMAGE,
    PLAYER_DEATH,
    PLAYER_HEAL,
    PLAYER_SWORD_SLASH,

    BAT_DAMAGE,
    BAT_DEATH,
    BAT_ATTACK,

    SKELETON_DAMAGE,
    SKELETON_DEATH,
    SKELETON_SWORD_SLASH_SLOW,
    SKELETON_SWORD_SLASH_FAST,

    SPIKE_OUT,
    SPIKE_IN,

    VICTORY,
    DEFEAT,
    TIME_TICKING,

    TRAPDOOR_OPEN,

    UI_WHOOSH,
}

[Serializable]
public struct SoundPair
{
    public Sound sound;
    public AudioClip clip;
}

public struct PlayingSound { }

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer current { get; private set; }

    [SerializeField]
    public SoundPair[] soundFileArray = { };
    public Dictionary<Sound, AudioClip> soundFiles = new();
    public Dictionary<PlayingSound, AudioSource> playingSounds = new();

    private AudioSource audioSource;

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            current = this;
        }
        foreach (SoundPair entry in soundFileArray)
        {
            soundFiles[entry.sound] = entry.clip;
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlaySound(Sound sound)
    {
        PlaySound(sound, false);
    }

    public PlayingSound PlaySound(Sound sound, bool looped)
    {
        if (!soundFiles.TryGetValue(sound, out var clip)) return new PlayingSound();

        Debug.Log("Play loop " + sound + ": " + looped);
        if (!looped)
        {
            audioSource.PlayOneShot(clip);
            return new PlayingSound();
        }
        PlayingSound soundRef = new();
        GameObject go = new GameObject();
        go.transform.SetParent(transform, false);
        AudioSource source = go.AddComponent<AudioSource>();
        playingSounds[soundRef] = source;
        source.clip = clip;
        source.loop = looped;
        source.Play();
        return soundRef;
    }
}
