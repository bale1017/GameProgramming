using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    public static float score = 0;

    public Text scoreText;

    // Update is called once per frame
    void Update()
    {
        scoreText.text = string.Format("Score: {0:00000}", score);
    }
}
