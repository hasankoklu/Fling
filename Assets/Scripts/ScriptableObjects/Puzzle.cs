﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Puzzle", menuName = "Puzzle")]
public class Puzzle : ScriptableObject
{
    public int Level;
    public List<PuzzleBasePiece> PuzzleBasePieceList;
    public List<PuzzlePiece> PuzzlePieceList;
    public List<ObstaclePiece> ObstaclePieceList;
    public GameObject PuzzleGroundPrefab;
    
}
