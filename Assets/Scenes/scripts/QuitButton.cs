using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnMouseUp()
    {
        Application.Quit();
        Debug.Log("Hello: " + gameObject.name);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
