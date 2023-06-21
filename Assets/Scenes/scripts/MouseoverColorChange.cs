using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Mouseover : MonoBehaviour
{
    void Start()
    {

        //GetComponent<Text>().color = new Color32(204, 204, 204, 255);
    }

    void OnMouseEnter()
    {
        //GetComponent<Text>().color = new Color32(100, 0, 0, 255);
    }

    void OnMouseExit()
    {
        //GetComponent<Text>().color = new Color32(204, 204, 204, 255);
    }
    public void Reset()
    {
       // GetComponent<Text>().color = new Color32(204, 204, 204, 255);
    }
}
