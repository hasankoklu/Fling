using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Puzzle
{
    [SerializeField]
    public int Level;
    [SerializeField]
    public List<PuzzlePieceList> MyStepList;
}