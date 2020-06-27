using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Puzzle Piece", menuName = "Puzzle Piece")]
public class PuzzlePiece : ScriptableObject
{
    public string Name;
    public string Tag;
    public GameObject GameObject;
    public int ScaleX;
    public int ScaleY;

    [Button("Set")]
    void Set()
    {
        GameObject go = Instantiate(GameObject);
        go.transform.SetParent(GameObject.Find("Puzzle").transform);
        go.AddComponent<PuzzlePieceDisplay>().puzzlePiece = this;

    }
}
