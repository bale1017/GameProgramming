using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyFramePlayer : MonoBehaviour
{
    private List<KeyFrame> Frames = new List<KeyFrame>();
    private Func<float> TimeScale = () => 1;

    private long Before = 0;
    private long Now = 0;
    
    public void SetTimeScale(Func<float> timeScale)
    {
        this.TimeScale = timeScale;
    }

    public void Play(List<KeyFrame> Frames, long Now)
    {
        this.Frames = new List<KeyFrame>(Frames);
        this.Now = Now;
        this.TimeScale = () => 1;
    }

    public void Stop() {
        this.Frames.Clear();
        this.TimeScale = () => 0;
    }

    private void Update()
    {
        float TimeScale = this.TimeScale();
        if (TimeScale == 0) return;

        Before = Now;
        Now += (long) (Time.deltaTime * 1000 * TimeScale);
        
        foreach (KeyFrame frame in Frames)
        {
            if (TimeScale < 0)
            {
                if (frame.endTimeStamp < Now) continue;
                if (frame.endTimeStamp >= Before) continue;
                frame.PlayRewind(gameObject);
            } else
            {
                if (frame.startTimeStamp > Now) continue;
                if (frame.startTimeStamp <= Before) continue;
                frame.PlayForwards(gameObject);
            }
        }
    }
}
