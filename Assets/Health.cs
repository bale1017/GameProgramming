using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float initHealth = 1;
    public float maxHealth = 1;
    public bool vulnerable = true;
    private float health;
    public UnityEvent<float> OnHealthChange;
    public UnityEvent<float> OnHealthDecreaseBy;
    public UnityEvent<float> OnHealthIncreaseBy;
    public UnityEvent OnDeath;

    private void Start()
    {
        health = initHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public void AffectHealth(float val)
    {
        if (!vulnerable) return;
        Debug.Log("Reduce total health of " + health + " by " + val);
        if (health + val < 0)
        {
            float cur = health;
            GetComponent<ReTime>().AddKeyFrame(
                g => {
                    health = 0;
                    OnDeath.Invoke();
                    OnHealthDecreaseBy.Invoke(cur);
                    OnHealthChange.Invoke(0);
                },
                g => {
                    health = cur;
                    OnHealthIncreaseBy.Invoke(cur);
                    OnHealthChange.Invoke(health);
                }
                );
            return;
        }
        if (health + val > maxHealth)
        {
            float cur = health;
            GetComponent<ReTime>().AddKeyFrame(
                g => {
                    health = maxHealth;
                    OnHealthChange.Invoke(maxHealth);
                    OnHealthIncreaseBy.Invoke(maxHealth - cur);
                },
                g => {
                    health = cur;
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
                        health -= val;
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
}
