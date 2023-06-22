using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CW.Common.CwInputManager;

public class CameraMovement : MonoBehaviour
{
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("move camera");
        }
           
    }
}
