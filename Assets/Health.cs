using UnityEngine;

public delegate void OnReceivingDamage(float val);
public delegate void OnHealthBelowZero(float val);

public class Health
{
    private float health = 1;
    private OnReceivingDamage onReceivingDamage;
    private OnHealthBelowZero onHealthBelowZero;

    public Health(float _health, OnReceivingDamage _onReceivingDamage, OnHealthBelowZero _onHealthBelowZero)
    {
        health = _health;
        onReceivingDamage = _onReceivingDamage;
        onHealthBelowZero = _onHealthBelowZero;
    }

    public float GetHealth()
    {
        return health;
    }

    public void ReduceHealth(float val)
    {
        Debug.Log("Reduce total health of " + health + " by " + val);
        if (health - val > 0)
        {
            health -= val;
            onReceivingDamage(val);
        } else
        {
            onHealthBelowZero(val);
        }
    }

    public void IncreaseHealth(float val)
    {
        health += val;
    }
}
