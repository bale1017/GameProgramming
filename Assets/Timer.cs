using Lean.Transition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    Text text;
    private double timeScale = 1;
    private double previousTime = 0;

    public double timer = 60;
    private double _timer;

    // Start is called before the first frame update
    void Start()
    {
        _timer = timer;
        text = GetComponent<Text>();
        previousTime = _timer;

        double before = 0;
        Game.current.OnGamePause.AddListener(() => {
            before = timeScale;
            timeScale = 0;
        });
        Game.current.OnGameUnpause.AddListener(() => timeScale = before);
        Game.current.OnGameCompletion.AddListener(() => timeScale = 0);
        Game.current.OnRewindStart.AddListener(() => timeScale = -1);
        Game.current.OnRewindEnd.AddListener(() => timeScale = 1);
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime * timeScale;
        if (_timer > timer)
        {
            _timer = timer;
            Game.current.StopRewind();
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
