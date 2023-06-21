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
        if (Input.GetKeyDown(RewindKey))
        {
            IsRewinding = true;
            OnRewindStart.Invoke();
        } else if (Input.GetKeyUp(RewindKey))
        {
            IsRewinding = false;
            OnRewindEnd.Invoke();
        }

        if (IsRewinding)
        {
            timer += Time.deltaTime;
        } else
        {
            timer -= Time.deltaTime;
        }
    }

    public double GetTime()
    {
        return timer;
    }
}
