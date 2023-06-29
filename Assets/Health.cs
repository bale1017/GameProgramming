using System;
using System.Runtime.ConstrainedExecution;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float initHealth = 1;
    public float maxHealth = 1;
    public bool vulnerable = true;
    [SerializeField]
    private float health;
    private bool updatedPlayerHealth = false;
    public UnityEvent<float> OnHealthChange;
    public UnityEvent<float> OnHealthDecreaseBy;
    public UnityEvent<float> OnHealthIncreaseBy;
    public UnityEvent OnDeath;

    private void Start()
    {
        if (!updatedPlayerHealth)
        {
            health = initHealth;
        }        
    }

    public float GetHealth()
    {
        return health;
    }

    public void AffectHealth(float val)
    {
        if (!vulnerable) return;
        Debug.Log("Reduce total health of " + health + " by " + val);
        if (health + val <= 0)
        {
            float cur = health;
            GetComponent<ReTime>().AddKeyFrame(
                g => {
                    g.GetComponent<Health>().health = 0;
                    OnHealthChange.Invoke(0);
                    OnHealthDecreaseBy.Invoke(cur);
                    OnDeath.Invoke();
                },
                g => {
                    g.GetComponent<Health>().health = cur;
                    OnHealthChange.Invoke(health);
                    OnHealthIncreaseBy.Invoke(cur);
                }
                );
            return;
        }
        if (health + val > maxHealth)
        {
            float cur = health;
            GetComponent<ReTime>().AddKeyFrame(
                g => {
                    g.GetComponent<Health>().health = maxHealth;
                    OnHealthChange.Invoke(maxHealth);
                    OnHealthIncreaseBy.Invoke(maxHealth - cur);
                },
                g => {
                    g.GetComponent<Health>().health = cur;
                    OnHealthChange.Invoke(health);
                    OnHealthDecreaseBy.Invoke(maxHealth - cur);
                }
            );
            
        } else
        {
            Action<GameObject> increase = g => {
                health += val;
                OnHealthChange.Invoke(health);
                if (val < 0)
                {
                    OnHealthDecreaseBy.Invoke(val);
                }
                else
                {
                    OnHealthIncreaseBy.Invoke(val);
                }
            };
            if (!TryGetComponent<ReTime>(out var retime))
            {
                increase(gameObject);
            } else {
                retime.AddKeyFrame(
                    increase, g => {
                        g.GetComponent<Health>().health -= val;
                        OnHealthChange.Invoke(health);
                    }
                );
            }
        }
    }

    public void IncreaseHealth(float val)
    {
        health += val;
    }

    public void MakeInvulnerable()
    {
        vulnerable = false;
    }

    public void MakeVulnerable()
    {
        vulnerable = true;
    }

    public void SetHealthOnLevelStart(float _health)
    {
        health = _health;
        updatedPlayerHealth = true;
        GameObject.Find("HealthBar_Player").GetComponent<HealthBar>().UpdateHealthBarOnLevelStart(_health);
    }
}
