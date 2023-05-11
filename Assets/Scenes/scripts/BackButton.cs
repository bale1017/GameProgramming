using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnMouseUp()
    {
        SceneManager.LoadScene("Menu");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
