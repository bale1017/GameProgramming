using Lean.Transition;
using Lean.Transition.Method;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public float AnimationDuration = .3f;
    public CanvasGroup Backdrop;
    public GameObject[] Buttons;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Game.current.IsPaused())
            {
                Game.current.ResumeGame();
            } else
            {
                Game.current.PauseGame();
            }
        }
    }

    public void FadeIn()
    {
        Backdrop.GetComponent<CanvasGroup>().alphaTransition(1, AnimationDuration);
        float width = Screen.width + 100;
        float delay = 0;
        foreach (GameObject button in Buttons)
        {
            button.transform
                .positionTransition_x(-width, 0)
                .positionTransition_x(-width, delay)
                .JoinTransition()
                .localPositionTransition_x(0, AnimationDuration);
            delay += AnimationDuration / 8;
        }
    }

    public void FadeOut()
    {
        Backdrop.GetComponent<CanvasGroup>().alphaTransition(-1, AnimationDuration);
        float width = Screen.width + 100;
        float delay = 0;
        foreach (GameObject button in Buttons)
        {
            button.transform
                .localPositionTransition_x(0, delay)
                .JoinTransition()
                .localPositionTransition_x(width, AnimationDuration);
            delay += AnimationDuration / 8;
        }
    }
}
