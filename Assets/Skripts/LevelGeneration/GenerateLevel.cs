using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Pathfinding;
using UnityEngine;


public class GenerateLevel : MonoBehaviour
{
    public int level;
    public GameObject[] rooms;
    public GameObject[] interiors;
    public GameObject[] Walls;
    public GameObject exitRoom;
    public float RoomWidth;
    public float RoomHeight;
    [HideInInspector]
    public List<GameObject> createdObjects;

    public void GenerateLayout()
    {
        createdObjects = new List<GameObject>();
        int roomAmount;
        int roomLayer;
        float roomOffset = 0;

        switch(level)
        {
            case 1:
            case 2:
                roomLayer = 0;
                roomAmount = Random.Range(3, 6);
                
                break;
            case 3:
            case 4:
                roomAmount = Random.Range(4, 8);
                roomLayer = 1;
                roomOffset = 3.2F;
                break;
            default:
                roomAmount = Random.Range(6, 9);
                roomLayer = 2;
                roomOffset = 6.4F;
                break;

        }
        List<(int, int)> roomList = new List<(int, int)>();
        for (int i = 0; i < roomAmount; i++)
        {
            roomList.Add((roomLayer, Random.Range(0, interiors.Length)));
        }
        List<Room> roomLayouts = ChooseLayout(roomAmount);
        /*
        Room test = roomLayouts[0];
        Debug.Log(test.hasNorth);
        Debug.Log(test.hasEast);
        Debug.Log(test.hasSouth);
        Debug.Log(test.hasWest);
        */
        int maxX = 0;
        int minX = 0;
        int maxY = 0;
        int minY = 0;

        GameObject newObject = null;
        for (int i = 0; i < roomList.Count; i++)
        {
            int roomType = roomList[i].Item1;
            int roomInterior = roomList[i].Item2;
            Room item = roomLayouts[i]; 
            float x = item.x * RoomWidth;
            float y = item.y * RoomHeight;  

            if (item.x > maxX) maxX = item.x;
            if (item.y > maxY) maxY = item.y;
            if (item.x < minX) minX = item.x;
            if (item.y < minY) minY = item.y;

            newObject = (GameObject) Instantiate(rooms[roomType], new Vector2(x, y + roomOffset), Quaternion.identity);
            createdObjects.Add(newObject);
            if (i == roomList.Count - 1)
            {
                newObject = (GameObject)Instantiate(exitRoom, new Vector2(x, y), Quaternion.identity);
                createdObjects.Add(newObject);
            }
            else if (i != 0)
            {
                newObject = (GameObject)Instantiate(interiors[roomInterior], new Vector2(x, y), Quaternion.identity);
                createdObjects.Add(newObject);
            }
            if (!item.hasNorth){
                if (roomLayer == 0)
                {
                    newObject = (GameObject) Instantiate(Walls[0], new Vector2(x, y), Quaternion.identity);
                    createdObjects.Add(newObject);
                }
                else
                {
                    newObject = (GameObject) Instantiate(Walls[0 + 4], new Vector2(x, y), Quaternion.identity);
                    createdObjects.Add(newObject);
                }
            }
            if (!item.hasEast)
            {
                if (roomLayer == 0)
                {
                    newObject = (GameObject) Instantiate(Walls[1], new Vector2(x, y), Quaternion.identity);
                    createdObjects.Add(newObject);
                }
                else
                {
                    newObject = (GameObject) Instantiate(Walls[1 + 4], new Vector2(x, y), Quaternion.identity);
                    createdObjects.Add(newObject);
                }

            }
            if (!item.hasSouth)
            {
                if (roomLayer == 0)
                {
                    newObject = (GameObject) Instantiate(Walls[2], new Vector2(x, y), Quaternion.identity);
                    createdObjects.Add(newObject);
                }
                else
                {
                    newObject = (GameObject) Instantiate(Walls[2 + 4], new Vector2(x, y), Quaternion.identity);
                    createdObjects.Add(newObject);
                }

            }
            if (!item.hasWest)
            {
                if (roomLayer == 0)
                {
                    newObject = (GameObject) Instantiate(Walls[3], new Vector2(x, y), Quaternion.identity);
                    createdObjects.Add(newObject);
                }
                else
                {
                    newObject = (GameObject) Instantiate(Walls[3 + 4], new Vector2(x, y), Quaternion.identity);
                    createdObjects.Add(newObject);
                }

            }
        }
        AstarPath.active.Scan();
    }

    class Room
    {
        public int x;
        public int y;
        public bool hasNorth;
        public bool hasEast;
        public bool hasSouth;
        public bool hasWest;
        public Room(int X, int Y, bool n=false, bool e=false, bool s=false, bool w=false)
        {
            x = X; y = Y; hasNorth = n; hasEast = e; hasSouth = s; hasWest = w;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Room;
            return item.x == x && item.y == y;
        }
    }

    // private List<(int, int, int, int, int, int)> ChooseLayout(int roomAmount)
    private List<Room> ChooseLayout(int roomAmount)
    {

        List<Room> placedRooms = new() { new Room(0, 0) };
        int availableRooms = roomAmount - 1;
        int i = 0;
        while (availableRooms > 0)
        {
            // nord, ost, süd, west
            Room current = placedRooms[i];
            Room newRoom = new Room(0, 0);

            int adjacentRooms = Random.Range(1, 16); // 1111 - 0001 
            for (int j = 0; j < 4; j++)
            {
                if ((adjacentRooms >> j & 1) == 1 && availableRooms > 0)
                {
                    switch (j)
                    {
                        case 0: // North
                            newRoom = new Room(current.x, current.y + 1, s: true);
                            current.hasNorth = true;

                            break;
                        case 1: // East
                            newRoom = new Room(current.x + 1, current.y, w: true);
                            current.hasEast = true;
                            break;
                        case 2: // South
                            newRoom = new Room(current.x, current.y - 1, n: true);
                            current.hasSouth = true;
                            break;
                        case 3: // West
                            newRoom = new Room(current.x - 1, current.y, e: true);
                            current.hasWest = true;
                            break;
                        default: break;
                    }

                    if (!placedRooms.Contains(newRoom))
                    {
                        placedRooms.Add(newRoom);
                        availableRooms--;
                    }
                }
            }
            i++;
        }
        return placedRooms;
    }
}


