using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cinemachine;
using System;
using System.Diagnostics;

public class PuzzleBaseDisplay : MonoBehaviour
{
    public static PuzzleBaseDisplay instance;

    public GameObject SquareGround;
    public GameObject RectangeGround;
    public GameObject LookAtHere;
    public GameObject CameraSystem;

    public List<GameObject> GameBasePiecePrefabList = new List<GameObject>();
    public CinemachineVirtualCamera virtualCam;
    private float ortoograpphicSize;

    private void Start()
    {
        virtualCam.m_Lens.OrthographicSize = 14;
        StartCoroutine(ortSizer());
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
                    gameBasePiece.transform.position = new Vector3(i, 0.25f, j);
                }
                else
                {
                    GameObject gameBasePiece = Instantiate(GameBasePiecePrefabList[1]);
                    gameBasePiece.transform.SetParent(transform);
                    gameBasePiece.transform.position = new Vector3(i, 0.25f, j);
                }
            }
        }

        Camera.main.transform.position = new Vector3((sizeDelta.x - 1) / 2, Camera.main.transform.position.y, Camera.main.transform.position.z);

        if (sizeDelta.x < 21 && sizeDelta.y < 21)
        {
            if (sizeDelta.x / sizeDelta.y > 1.5f)
            {
                GameObject go = Instantiate(RectangeGround);
                go.transform.position = new Vector3((sizeDelta.x - 3) / 2, 0, (sizeDelta.y + 6) / 2);
                //go.transform.localScale = go.transform.localScale
                LookAtHere.transform.position = go.transform.position;
            }
            else
            {
                int k = (int)(sizeDelta.x - 8);
                int l = (int)(sizeDelta.y - 8);
                GameObject go = Instantiate(SquareGround);
                go.transform.position = new Vector3((sizeDelta.x - 1) / 2, -0.35f, sizeDelta.y / 2);
                go.transform.localScale = new Vector3((1 + k * 0.14f), (1 + ((k + l) / 2f * 0.14f)), (1 + l * 0.14f));
                CameraSystem.transform.position = new Vector3(0.5f + k * 0.5f, CameraSystem.transform.position.y, CameraSystem.transform.position.z);
                LookAtHere.transform.position = new Vector3(sizeDelta.x / 2, go.transform.position.y, sizeDelta.y / 2);
            }
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
                ortoograpphicSize = (int)(item.position.x) + 3.5f;
            }
        }

        while (virtualCam.m_Lens.OrthographicSize > ortoograpphicSize)
        {
            virtualCam.m_Lens.OrthographicSize -= 0.023f;
            yield return new WaitForEndOfFrame();
        }
    }
}
