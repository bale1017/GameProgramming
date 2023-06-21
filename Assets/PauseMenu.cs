using Lean.Transition;
using Lean.Transition.Method;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;

    public float AnimationDuration = .3f;
    public CanvasGroup Backdrop;
    public GameObject[] Buttons;
    private float timeScale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
                ResumeGame();
            } else
            {
                PauseGame();

            }
        }
    }

    public void PauseGame()
    {
        timeScale = Time.timeScale;
        Time.timeScale = 0;
        FadeIn();
        Paused = true;
    }

    public void ResumeGame()
    {
        FadeOut();
        Time.timeScale = timeScale;
        Paused = false;
    }

    void FadeIn()
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

    void FadeOut()
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
