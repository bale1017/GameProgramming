using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{

    public AudioSource ButtonSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayButtonSound()
    {
        ButtonSound.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
