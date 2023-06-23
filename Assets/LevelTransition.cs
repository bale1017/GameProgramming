using Lean.Transition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelTransition : MonoBehaviour
{

    public static Vector3 center;
    public static Vector3 offset;
    public GameObject panel;
    public Text levelText;

    public UnityEvent OnFadeInComplete = new();

    private void Start()
    {
        center = transform.position;
        offset = new Vector3(Screen.width * 1.5f, 0, 0);
    }

    public void FadeIn()
    {
        panel.transform.position = center + offset;
        panel.transform.positionTransition(center, 0.5f);
        StartCoroutine(then(0.5f, OnFadeInComplete.Invoke));
    }

    IEnumerator then(float sec, Action action)
    {
        yield return new WaitForSeconds(sec);
        action();
    }

    public void FadeOut()
    {
        panel.transform.position = center;
        panel.transform.positionTransition(center - offset, 0.5f);
    }
}
