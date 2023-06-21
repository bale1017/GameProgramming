using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject LoadLevelMenu;
    // Start is called before the first frame update
    void Start()
    {
        LoadLevelMenu.SetActive(false);
    }
    void OnMouseUp()
    {
        MainMenu.SetActive(false);
        LoadLevelMenu.SetActive(true);
        this.GetComponent<Mouseover>().Reset();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
