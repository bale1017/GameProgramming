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


    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        previousTime = Game.current.GetTime();

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
        Game.current.timer -= Time.deltaTime * timeScale;
        double time = Game.current.GetTime();
        text.text = time.ToString("00.00s").Replace(",", ":");
        if (time < 5 && (int) previousTime > (int) time
            || (int) (previousTime / 10) > (int)(time / 10))
        {
            Animate();
        }
        if (time < 10)
        {
            SetColor();
        }
        previousTime = time;
    }

    private void SetColor()
    {
        text.color = Color.red;
    }

    private void Animate()
    {
        Vector3 scale = text.transform.localScale;
        text.transform.localScaleTransition_xy(scale * 1.4f, .15f)
            .JoinTransition()
            .localScaleTransition_xy(scale, .15f);
    }
}
