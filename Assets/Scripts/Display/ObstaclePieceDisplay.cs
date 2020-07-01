using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePieceDisplay : MonoBehaviour
{
    [HideInInspector]
    public ObstaclePiece obstaclePiece;


    [Button("Snap The Object")]
    public void SnapObject()
    {
        obstaclePiece = new ObstaclePiece();

        obstaclePiece.GameObject = gameObject;
        if (obstaclePiece.ScaleX == 0)
        {
            obstaclePiece.ScaleX = 1;
        }
        if (obstaclePiece.ScaleY == 0)
        {
            obstaclePiece.ScaleY = 1;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if ((transform.rotation.z / 90) % 2 == 0) // vertical
        {
            if (obstaclePiece.ScaleX % 2 == 1) // snap x
            {
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, transform.position.z);
            }
            else if (obstaclePiece.ScaleX % 2 == 0)
            {
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x - 0.5f) + 0.5f, transform.position.y, transform.position.z);
            }

            if (obstaclePiece.ScaleY % 2 == 1) // snap z
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.RoundToInt(transform.position.z));
            }
            else if (obstaclePiece.ScaleY % 2 == 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.RoundToInt(transform.position.z - 0.5f) + 0.5f);
            }
        }
        else // horizontal
        {
            if (obstaclePiece.ScaleY % 2 == 1) // snap x
            {
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, transform.position.z);
            }
            else if (obstaclePiece.ScaleY % 2 == 0)
            {
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x - 0.5f) + 0.5f, transform.position.y, transform.position.z);
            }

            if (obstaclePiece.ScaleX % 2 == 1) // snap z
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.RoundToInt(transform.position.z));
            }
            else if (obstaclePiece.ScaleX % 2 == 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.RoundToInt(transform.position.z - 0.5f) + 0.5f);
            }
        }
    }

    [Button("Rotate The Object")]
    void RotateObject()
    {
        transform.Rotate(0f, 90f, 0f);
    }

    private void Start()
    {
        obstaclePiece = new ObstaclePiece();
        obstaclePiece.GameObject = gameObject;
        ObstacleDisplay.instance.currentObstaclePieceList.Add(obstaclePiece);
    }

}
