using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonLoadLevel : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject LoadLevelMenu;
    void Start()
    {
    }
    void OnMouseUp()
    {
        MainMenu.SetActive(true);
        LoadLevelMenu.SetActive(false);
        this.GetComponent<Mouseover>().Reset();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
