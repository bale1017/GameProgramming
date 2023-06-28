using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    VICTORY,
    TIME_UP,
    TIME_TICKING,

    TRAPDOOR_OPEN
}

[Serializable]
public struct SoundPair
{
    public Sound sound;
    public AudioClip clip;
}

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer current { get; private set; }

    [SerializeField]
    public SoundPair[] soundFileArray;
    public Dictionary<Sound, AudioClip> soundFiles = new();

    private AudioSource audioSource;

    private void Start()
    {
        current = this;
        foreach (SoundPair entry in soundFileArray)
        {
            soundFiles[entry.sound] = entry.clip;
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlaySound(Sound sound)
    {
        Debug.Log("Play sound " + sound);
        if (!soundFiles.TryGetValue(sound, out var clip)) return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}
