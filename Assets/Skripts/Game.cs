using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{

    public static bool IsRewinding = false;

    public KeyCode RewindKey = KeyCode.R;
    public UnityEvent OnRewindStart = new ();
    public UnityEvent OnRewindEnd = new ();

    double timer = 60;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // start and stop if Rewind Key is pressed and game is not paused
        if (!PauseMenu.Paused)
        {
            if (Input.GetKeyDown(RewindKey))
            {
                StartRewind();
            }
            else if (Input.GetKeyUp(RewindKey))
            {
                StopRewind();
            }
        }

        // make as listener
        if (IsRewinding)
        {
            timer += Time.deltaTime;
        } else
        {
            timer -= Time.deltaTime;
        }
    }

    public void StartRewind()
    {
        IsRewinding = true;
        // Invoke start event so that all listeners know that time is now rewinding.
        OnRewindStart.Invoke();
    }

    public void StopRewind()
    {
        IsRewinding = false;
        // Invoke stop event so that all listeners know that time is no longer rewinding.
        OnRewindEnd.Invoke();
    }

    public double GetTime()
    {
        return timer;
    }
}
