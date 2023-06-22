using Lean.Transition;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CW.Common.CwInputManager;

public class CameraMovement : MonoBehaviour
{
    public Vector3 StartLocationCamera = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 StartLocationPlayer = new Vector3(0.0f, 0.0f, 0.0f);

    Vector3 FlipPlayerOffset = new Vector3(1.0f, 1.0f, 0.0f);

    Vector3 FlipCameraOffset = new Vector3(1.0f, 1.0f, 0.0f);



    public GameObject Player;
    public GameObject Game;

    void Start()
    {
        StartLocationCamera = transform.position;
        StartLocationPlayer = Player.transform.position;
        FlipPlayerOffset.x = Game.GetComponent<GenerateLevel>().RoomWidth;
        FlipPlayerOffset.y = Game.GetComponent<GenerateLevel>().RoomHeight;

        FlipCameraOffset.x = Game.GetComponent<GenerateLevel>().RoomWidth;
        FlipCameraOffset.y = Game.GetComponent<GenerateLevel>().RoomHeight;
    }

    void Update()
    {
        Vector3 PlayerDifference = Player.transform.position - StartLocationPlayer;
        if (FlipPlayerOffset.x == 0 || FlipPlayerOffset.y == 0)//dont divide by zero
        {
            return;
        }
        //calculate how many screens the player is away from the starting tile
        float overlapsX = PlayerDifference.x / FlipPlayerOffset.x;
        float overlapsY = PlayerDifference.y / FlipPlayerOffset.y;
        int overlapsXRounded = (int)Mathf.Round(overlapsX);
        int overlapsYRounded = (int)Mathf.Round(overlapsY);
        // Debug.Log("camerastart: " + StartLocationCamera + " player start" + StartLocationPlayer + "PlayerDifferencex: " 
        //    + PlayerDifference.x + " y" + PlayerDifference.y 
        //    + "overlapsXRounded: " + overlapsXRounded);
        //add the cameraoffset times the amount of screens we are away from the start
        Vector3 target = StartLocationCamera + new Vector3(FlipCameraOffset.x * overlapsXRounded, FlipCameraOffset.y * overlapsYRounded, 0.0f);

        transform.positionTransition(target, 0.04f);

    }
}
