using Lean.Transition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteMenu : MonoBehaviour
{

    public GameObject backgroundVictory;
    public GameObject backgroundDefeat;
    public GameObject textVictory;
    public GameObject textDefeat;

    private GameObject background;
    private GameObject text;

    private void Start()
    {
        backgroundVictory.SetActive(false);
        backgroundDefeat.SetActive(false);
        textVictory.SetActive(false);
        textDefeat.SetActive(false);
    }

    public void FadeIn(bool victory)
    {
        background = victory ? backgroundVictory : backgroundDefeat;
        text = victory ? textVictory : textDefeat;
        Vector3 bPos = background.transform.position;
        Vector3 tPos = text.transform.position;
        Debug.Log(bPos);

        background.SetActive(true);
        background.transform
            .positionTransition_x(+Screen.width * 2, 0)
            .JoinTransition()
            .positionTransition_x(bPos.x, .3f);
        text.SetActive(true);
        text.transform
            .positionTransition_x(-Screen.width * 2, 0)
            .JoinTransition()
            .positionTransition_x(tPos.x, 0.15f);
    }

    public void FadeOut()
    {
        background.SetActive(false); 
        text.SetActive(false);
    }
}
