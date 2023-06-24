using Lean.Transition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

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
        Vector3 bPos = background.transform.position;
        Vector3 tPos = text.transform.position;

        StartCoroutine(then(.2f, () =>
        {
            background.transform
                .positionTransition_x(bPos.x - Screen.width, .3f);
            text.transform
                .positionTransition_x(tPos.x - Screen.width, 0.15f);

            StartCoroutine(then(.3f, () => {
                background.SetActive(false);
                text.SetActive(false);
            }));
        }));
    }

    IEnumerator then(float sec, UnityAction then)
    {
        yield return new WaitForSeconds(sec);
        then();
    }
}
