using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public int level;
    public GameObject[] rooms;
    public GameObject[] interiors;
    public int RoomWidth; // should be around 3
    public int RoomHeight; // should be around 5

    public void GenerateLayout()
    {
        int roomAmount;
        int roomLayer;

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
                break;
            default:
                roomAmount = Random.Range(6, 9);
                roomLayer = 2;
                break;

        }
        List<(int, int)> roomList = new List<(int, int)>();
        for (int i = 0; i < roomAmount; i++)
        {
            roomList.Add((roomLayer, Random.Range(0, interiors.Length - 1)));
        }
        List<(int, int)> roomLayouts = ChooseLayout(roomAmount);
        for (int i = 0; i < roomList.Count; i++)
        {
            int roomType = roomList[i].Item1;
            int roomInterior = roomList[i].Item2;
            int x = roomLayouts[i].Item1 * RoomHeight;
            int y = roomLayouts[i].Item2 * RoomWidth;
            Instantiate(rooms[roomType], new Vector2(x, y), Quaternion.identity);
            Instantiate(interiors[roomInterior], new Vector2(x, y), Quaternion.identity);
        }
    }


    private List<(int,int)> ChooseLayout(int roomAmount)
    {
        List<(int, int)> placedRooms = new()
        {
            (0, 0)
        };

        int availableRooms = roomAmount - 1;
        int i = 0;
        while (availableRooms > 0)
        {
            (int, int) current = placedRooms[i];
            (int, int) newRoom = (0, 0);
            // nord, ost, süd, west
            int adjacentRooms = Random.Range(1, 16);
            for (int j = 0; j < 4; j++)
            {
                if ((adjacentRooms >> j & 1) == 1 && availableRooms > 0)
                {
                    switch (j)
                    {
                        case 0:
                            newRoom = (current.Item1, current.Item2 + 1);
                            break;
                        case 1:
                            newRoom = (current.Item1 + 1, current.Item2);
                            break;
                        case 2:
                            newRoom = (current.Item1, current.Item2 - 1);
                            break;
                        case 3:
                            newRoom = (current.Item1 - 1, current.Item2);
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

