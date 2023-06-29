using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BackroundMusic : MonoBehaviour
{
    PlayingSound sound;

    // Start is called before the first frame update
    void Start()
    {

        Game.current.OnGameStart.AddListener(() => {
            sound = SoundPlayer.current.PlaySound(Sound.BACKGROUND_MUSIC, 0.05f, true);
        });
        Game.current.OnGamePause.AddListener(() => {
            SoundPlayer.current.StopSound(sound);
        });
        Game.current.OnGameUnpause.AddListener(() => {
            sound = SoundPlayer.current.PlaySound(Sound.BACKGROUND_MUSIC, 0.05f, true);
        });
        Game.current.OnGameCompletion.AddListener(() => {
            SoundPlayer.current.StopSound(sound);
        });
        Game.current.OnRewindStart.AddListener(() => {
            SoundPlayer.current.StopSound(sound);
        });
        Game.current.OnRewindEnd.AddListener(() => {
            SoundPlayer.current.StopSound(sound);
            sound = SoundPlayer.current.PlaySound(Sound.BACKGROUND_MUSIC, 0.05f, true);
        });
    }
}
