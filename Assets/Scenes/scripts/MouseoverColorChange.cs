using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mouseover : MonoBehaviour
{
    void Start()
    {
        TextMeshPro textmeshPro = GetComponent<TextMeshPro>();
        textmeshPro.color = new Color32(0, 0, 0, 255);
    }

    void OnMouseEnter()
    {
        TextMeshPro textmeshPro = GetComponent<TextMeshPro>();
        textmeshPro.color = new Color32(100, 0, 0, 255);
    }

    void OnMouseExit()
    {
        TextMeshPro textmeshPro = GetComponent<TextMeshPro>();
        textmeshPro.color = new Color32(0, 0, 0, 255);
    }
    public void Reset()
    {
        TextMeshPro textmeshPro = GetComponent<TextMeshPro>();
        textmeshPro.color = new Color32(0, 0, 0, 255);
    }
}
