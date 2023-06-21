using Lean.Transition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    Text text;
    private double testTime = 60;
    private double previousTime = 0;

    double getTime()
    {
        return testTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        previousTime = getTime();
    }

    // Update is called once per frame
    void Update()
    {
        double time = getTime();
        text.text = time.ToString("00.00s").Replace(",", ":");
        if (time < 5 && (int) previousTime > (int) time
            || (int) (previousTime / 10) > (int)(time / 10))
        {
            Animate();
        }
        if (time < 10)
        {
            SetColor();
        }
        previousTime = time;
        testTime -= Time.deltaTime;
    }

    private void SetColor()
    {
        text.color = Color.red;
    }

    private void Animate()
    {
        Debug.Log("anim");
        Vector3 scale = text.transform.localScale;
        text.transform.localScaleTransition_xy(scale * 1.4f, .15f)
            .JoinTransition()
            .localScaleTransition_xy(scale, .15f);
    }
}
