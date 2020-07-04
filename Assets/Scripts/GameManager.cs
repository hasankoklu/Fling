using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector]
    public int currentLevel = 1;

    [HideInInspector]
    public GameObject SelectedObject;
    public float PuzzleMovementSpeed;
    public float SlideDetectionDistance;
    public GameObject InGameCanvas;
    public bool isGameRunning;

    public int backButtonCounter;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level > 0)
        {
            InGameCanvas.SetActive(true);
            GameObject.Find("Info-Text").GetComponent<Text>().text = "Level : " + currentLevel;
            GameObject.Find("Suggest-Button").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Back-Button").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Restart-Button").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("Suggest-Button").GetComponent<Button>().onClick.AddListener(PuzzleDisplay.instance.SuggestButtonClick);
            GameObject.Find("Back-Button").GetComponent<Button>().onClick.AddListener(PuzzleDisplay.instance.BackButtonClick);
            GameObject.Find("Restart-Button").GetComponent<Button>().onClick.AddListener(() => PuzzleDisplay.instance.LoadPuzzle(currentLevel));

            isGameRunning = true;
        }
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("currentLevel") == 0)
        {
            currentLevel = 1;
        }
        else
        {
            currentLevel = PlayerPrefs.GetInt("currentLevel");
        }

    }

    public void GoToNextLevelButtonClick()
    {
        if (currentLevel > 5)
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("currentLevel", GameManager.instance.currentLevel);
        }
        Debug.Log(currentLevel);
        SceneManager.LoadScene("Assets/Scenes/Levels/Level" + currentLevel.ToString() + ".unity");
    }
}