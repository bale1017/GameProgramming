using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonOptions : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    void Start()
    {
    }
    void OnMouseUp()
    {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        this.GetComponent<Mouseover>().Reset();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
