using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mouseover : MonoBehaviour
{
    public AudioSource ButtonClick;
    Color OldColor;
    void Start()
    {
        OldColor = GetComponent<Text>().color;
    }

    void OnMouseEnter()
    {
        GetComponent<Text>().color = new Color32(100, 0, 0, 255);
    }

    void OnMouseExit()
    {
        GetComponent<Text>().color = OldColor;
    }
    public void Reset()
    {
        GetComponent<Text>().color = OldColor;
    }
    void OnMouseUp()
    {
        ButtonClick.Play();
    }
}
