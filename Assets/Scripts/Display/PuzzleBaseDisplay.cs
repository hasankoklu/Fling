﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;

public class PuzzleBaseDisplay : MonoBehaviour
{
    public static PuzzleBaseDisplay instance;

    public GameObject Ground7xPrefab;

    public GameObject LookAtHere;

    public List<GameObject> GameBasePiecePrefabList = new List<GameObject>();
    public CinemachineVirtualCamera virtualCam;
    private int ortoograpphicSize;

    private void Start()
    {
        StartCoroutine(ortSizer());
        virtualCam.m_Lens.OrthographicSize = 14;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    [Button("Set Base")]
    void SetDefaultGameBase(Vector2 sizeDelta)
    {
        for (int i = 0; i < sizeDelta.x; i++)
        {
            for (int j = 0; j < sizeDelta.y; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    GameObject gameBasePiece = Instantiate(GameBasePiecePrefabList[0]);
                    gameBasePiece.transform.SetParent(transform);
                    gameBasePiece.transform.position = new Vector3(i, 0, j);
                }
                else
                {
                    GameObject gameBasePiece = Instantiate(GameBasePiecePrefabList[1]);
                    gameBasePiece.transform.SetParent(transform);
                    gameBasePiece.transform.position = new Vector3(i, 0, j);
                }
            }
        }


        Camera.main.transform.position = new Vector3((sizeDelta.x - 1) / 2, Camera.main.transform.position.y, Camera.main.transform.position.z);

        if (sizeDelta.x < 11 && sizeDelta.y < 11)
        {
            GameObject go = Instantiate(Ground7xPrefab);
            go.transform.position = new Vector3((sizeDelta.x - 1) / 2, 0, sizeDelta.y / 2);
            LookAtHere.transform.position = go.transform.position;
        }
    }

    [Button("Clear Base")]
    void PuzzleBase()
    {
        while (GameObject.Find("PuzzleBase").transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        DestroyImmediate(GameObject.FindGameObjectWithTag("Ground"));
    }


    IEnumerator ortSizer()
    {
        yield return new WaitForSeconds(2f);

        foreach (Transform item in GameObject.Find("PuzzleBase").transform)
        {
            if (item.position.x > ortoograpphicSize)
            {
                ortoograpphicSize = (int)(item.position.x) + 3;
            }
        }

        while (virtualCam.m_Lens.OrthographicSize > ortoograpphicSize + 1)
        {
            virtualCam.m_Lens.OrthographicSize -= 0.01f;
            yield return new WaitForEndOfFrame();
        }
    }
}
