using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : MonoBehaviour
{
    public Sprite[] sprites;
    private Sprite currentSprite;
    private int counter;
    public float switchTime;

    void Start()
    {
        counter = 0;
        StartCoroutine("SwitchSprite");
        SoundPlayer.current.PlaySound(Sound.FOUNTAIN_SPLASHING,at:this.transform, loop: true);
    }

    void OnGUI()
    {
        this.GetComponent<SpriteRenderer>().sprite = currentSprite;
    }

    private IEnumerator SwitchSprite()
    {
        currentSprite = sprites[counter];
        if (counter < sprites.Length - 1)
        {
            counter++;
        }
        else
        {
            counter = 0;
        }

        yield return new WaitForSeconds(switchTime);
        StartCoroutine("SwitchSprite");
    }
}
