using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

[ExecuteInEditMode]
public class PuzzleBasePieceDisplay : MonoBehaviour
{

    [OnValueChanged("UpdateBaseType")]
    public BaseType baseType;

    [HideInInspector]
    public ObstaclePiece obstaclePiece;

    private void Start()
    {
        if (baseType == BaseType.Water)
        {
            obstaclePiece = new ObstaclePiece();
            obstaclePiece.GameObject = gameObject;
            if (ObstacleDisplay.instance != null)
                ObstacleDisplay.instance.currentObstaclePieceList.Add(obstaclePiece);
        }
    }

    void UpdateBaseType()
    {
        GameObject go = Instantiate(GameObject.Find("PuzzleBase").GetComponent<PuzzleBaseDisplay>().GameBasePiecePrefabList[baseType.GetHashCode()]);
        go.transform.SetParent(transform.parent);
        go.transform.position = transform.position;
        go.gameObject.GetComponent<PuzzleBasePieceDisplay>().baseType = baseType;
        StartCoroutine(ien());
    }

    IEnumerator ien()
    {
        yield return new WaitForSeconds(0.1f);
        DestroyImmediate(gameObject);
    }

    public enum BaseType { Earth, Green, Sand, Water };
}