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

    void Start()
    {
        slider = GetComponent<Slider>();

        UnityAction<float> a = h => {
            slider.value = h / health.maxHealth;
        };
        a(health.initHealth);
        health.OnHealthChange.AddListener(a);
    }
}
