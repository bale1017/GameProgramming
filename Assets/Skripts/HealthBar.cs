using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Health health;
    private Slider slider;

    public UnityEvent OnRaise;
    public UnityEvent OnLower;

    void Start()
    {
        slider = GetComponent<Slider>();
        UnityAction<float> a = h => {
            float before = slider.value;
            slider.value = h / health.maxHealth;
            (before < slider.value ? OnRaise : OnLower).Invoke();
        };
        a(health.initHealth);
        health.OnHealthChange.AddListener(a);
        StartCoroutine(UpdateHealthBarOnLevelStart());
    }

    public IEnumerator UpdateHealthBarOnLevelStart()
    {
        yield return new WaitForFixedUpdate();
        var _health = health.GetHealth();
        if (_health > 0)
        {
            slider.value = _health / health.maxHealth;  //adapt value at beginning of next level
        }
    }
}
