using Lean.Transition;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;


[Serializable]
public enum Sound
{
    PLAYER_RUNNING,
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
    PLAYER_TIMELINE_END

    TORCH_CRACKLING,
}

[Serializable]
public struct SoundPair
{
    public Sound sound;
    public AudioClip clip;
}

public class PlayingSound {
}

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer current { get; private set; }

    public float maxDistance = 12;

    [SerializeField]
    public SoundPair[] soundFileArray = { };
    public Dictionary<Sound, AudioClip> soundFiles = new();
    public Dictionary<PlayingSound, AudioSource> playingSounds = new();

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            current = this;
        }
        DontDestroyOnLoad(this.gameObject);
        foreach (SoundPair entry in soundFileArray)
        {
            soundFiles[entry.sound] = entry.clip;
        }
    }

    public void StopSound(PlayingSound sound)
    {
        if (playingSounds.TryGetValue(sound, out var source))
        {
            source.Stop();
            Destroy(source.gameObject);
            playingSounds.Remove(sound);
        }
    }

    public PlayingSound PlaySound(Sound sound)
    {
        return PlaySound(sound, false);
    }

    public PlayingSound PlaySound(Sound sound, bool loop)
    {
        return PlaySound(sound, transform, 1, 1, loop);
    }

    public PlayingSound PlaySound(Sound sound, Transform at)
    {
        return PlaySound(sound, at, 1, 1);
    }

    public PlayingSound PlaySound(Sound sound, Transform at, float pitch, float volume)
    {
        return PlaySound(sound, at, pitch, volume, false);
    }

    public PlayingSound PlaySound(Sound sound, Transform at, bool loop)
    {
        return PlaySound(sound, at, 1, 1, loop);
    }

    public PlayingSound PlaySound(Sound sound, Transform at, float pitch, float volume, bool loop)
    {
        if (at.gameObject.TryGetComponent<ReTime>(out var retime))
        {
        }
        return PlaySoundAsKeyFrame(sound, at, pitch, volume, loop);
    }

    public PlayingSound PlaySoundAsKeyFrame(Sound sound, Transform at, float pitch, float volume, bool loop)
    {
        if (!soundFiles.TryGetValue(sound, out var clip)) return new PlayingSound();

        PlayerInput input = FindAnyObjectByType<PlayerInput>();
        if (input == null)
            throw new Exception("Cannot play sound at location while no player exists.");

        bool global = at == transform;
        float dist = (at.position - input.transform.position).magnitude;
        if (!global && dist > maxDistance)
        {
            return new PlayingSound();
        }
        GameObject audio = new();
        DontDestroyOnLoad(audio);
        audio.transform.SetParent(at, false);
        AudioSource source = audio.AddComponent<AudioSource>();
        source.clip = clip;
        source.pitch = pitch;
        source.volume = volume;
        source.loop = loop;
        source.spatialize = !global;
        source.spatialBlend = 1;
        source.maxDistance = global ? float.MaxValue : maxDistance;
        source.Play();

        PlayingSound p = new();
        playingSounds[p] = source;

        if (!loop)
        {
            StartCoroutine(Then(clip.length, () => {
                Destroy(audio);
                playingSounds.Remove(p);
            }));
        }
        return p;
    }

    IEnumerator Then(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
}
