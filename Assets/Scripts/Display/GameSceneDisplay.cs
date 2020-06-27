using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneDisplay : MonoBehaviour
{

    public static GameSceneDisplay instance;
    public GameObject LockPanel;
    public Text InfoText;
    public GameObject SelectedObject;
    public GameObject FinishPopUpRect;
    public bool isGameRunning;

    public float PuzzleMovementSpeed;
    public float SlideDetectionDistance;

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

    private void Start()
    {
        isGameRunning = true;
        StartCoroutine(FinishCheck());
    }

    public void RestartButtonClick()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator FinishCheck()
    {
        while (isGameRunning)
        {
            if (PuzzleDisplay.instance.currentPuzzlePieceList.Count == 1)
            {
                isGameRunning = false;
                FinishPopUpRect.SetActive(true);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
