using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PuzzleDisplay : MonoBehaviour
{
    public static PuzzleDisplay instance;

    [HideInInspector]
    public List<PuzzlePiece> currentPuzzlePieceList;

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

    [Button("Clear This Puzzle")]
    void ClearPuzzle()
    {
        while (GameObject.Find("Puzzle").transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

    }

    [Button("Save This Puzzle")]
    void SavePuzzle(int level)
    {


    }

    public class MyPuzzleStep
    {
        public int level;
        public List<PuzzlePiece> currentStepPuzzlePiece;
    }
}