using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnMouseUp()
    {
        SceneManager.LoadScene("LevelSelection");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
