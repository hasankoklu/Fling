using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDisplay : MonoBehaviour
{

    public static ObstacleDisplay instance;

    public List<ObstaclePiece> currentObstaclePieceList;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    [Button("Clear This Obstacles")]
    void ClearPuzzle()
    {
        while (GameObject.Find("Obstacle").transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}