using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Obstacle Piece", menuName = "Obstacle Piece")]
public class ObstaclePiece : ScriptableObject
{

    public string Name;
    public string Tag;
    public GameObject GameObject;
    public int ScaleX;
    public int ScaleY;
    [HideInInspector]
    public float yCoord;
    
    [Button("Set")]
    void Set()
    {
        GameObject go = Instantiate(GameObject);
        go.transform.SetParent(GameObject.Find("Obstacle").transform);
        go.AddComponent<ObstaclePieceDisplay>().obstaclePiece = this;

    }

    [Button("Set Deefault Y Coordinate For Snap")]
    void SetDefaultYCoordinate()
    {
        if (GameObject != null)
        {
            yCoord = GameObject.transform.position.y;
        }
        else
        {
            Debug.Log("Please give a gameobject");
        }
    }
    
}
