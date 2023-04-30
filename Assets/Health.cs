using System;

public class Health
{
    private float health = 1;
    private Action ifLessOrEqualZero;

    public Health(float _health, Action _ifLessOrEqualZero)
    {
        health = _health;
        ifLessOrEqualZero = _ifLessOrEqualZero;
    }

    public float getHealth()
    {
        return health;
    }

    public void ReduceHealth(float val)
    {
        health -= val;
        if (health <= 0)
        {
            ifLessOrEqualZero();
        }
    }

    public void IncreaseHealth(float val)
    {
        health += val;
    }
}
