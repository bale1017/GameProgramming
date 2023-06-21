using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsButton : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    void Start()
    {
        OptionsMenu.SetActive(false);
    }

    void OnMouseUp()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        this.GetComponent<Mouseover>().Reset();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
