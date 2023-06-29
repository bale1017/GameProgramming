using Lean.Transition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    Text text;
    private double timeScale = 0;
    private double previousTime = 0;

    public double timer = 60;
    private double _timer;

    PlayingSound ticking;

    // Start is called before the first frame update
    void Start()
    {
        _timer = timer;
        text = GetComponent<Text>();
        previousTime = _timer;

        Game.current.OnGameStart.AddListener(() => {
            timeScale = 1;
            ticking = SoundPlayer.current.PlaySound(Sound.TIME_TICKING, true);
        });
        double before = 0;
        Game.current.OnGamePause.AddListener(() => {
            before = timeScale;
            timeScale = 0;
            SoundPlayer.current.StopSound(ticking);
        });
        Game.current.OnGameUnpause.AddListener(() => {
            timeScale = before;
            ticking = SoundPlayer.current.PlaySound(Sound.TIME_TICKING, true);
        });
        Game.current.OnGameCompletion.AddListener(() => { 
            timeScale = 0;
            SoundPlayer.current.StopSound(ticking);
        });
        Game.current.OnRewindStart.AddListener(() => {
            timeScale = -1;
            SoundPlayer.current.StopSound(ticking);
        });
        Game.current.OnRewindEnd.AddListener(() => {
            timeScale = 1;
            SoundPlayer.current.StopSound(ticking);
            ticking = SoundPlayer.current.PlaySound(Sound.TIME_TICKING, true);
        });

    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime * timeScale * (Game.IsRewinding ? GetComponent<ReTime>().RewindSpeed : 1);
        if (_timer > timer)
        {
            _timer = timer;
            Game.current.StopRewind();
        }
        if (timeScale != 0 && _timer <= 0)
        {
            _timer = 0;
            Game.current.FailGame();
            return;
        }

        text.text = _timer.ToString("00.00s").Replace(",", ":");
        if (_timer < 5 && (int) previousTime > (int)_timer
            || (int) (previousTime / 10) > (int)(_timer / 10))
        {
            Animate();
        }
        SetColor();
        previousTime = _timer;
    }

    private void SetColor()
    {
        if (timeScale < 0)
        {
            text.color = Color.cyan;
        } else if (_timer < 10)
        {
            text.color = Color.red;
        } else
        {
            text.color = Color.white;
        }
    }

    private void Animate()
    {
        Vector3 scale = text.transform.localScale;
        text.transform.localScaleTransition_xy(scale * 1.4f, .15f)
            .JoinTransition()
            .localScaleTransition_xy(scale, .15f);
    }
}
