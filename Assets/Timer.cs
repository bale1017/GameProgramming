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
    private double testTime = 60;
    private double previousTime = 0;

    public Game game;


    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        previousTime = game.GetTime();
    }

    // Update is called once per frame
    void Update()
    {
        double time = game.GetTime();
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
        Debug.Log("anim");
        Vector3 scale = text.transform.localScale;
        text.transform.localScaleTransition_xy(scale * 1.4f, .15f)
            .JoinTransition()
            .localScaleTransition_xy(scale, .15f);
    }
}
