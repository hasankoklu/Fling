using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneDisplay : MonoBehaviour
{

    public static GameSceneDisplay instance;
    //public GameObject WindParticlePrefab;
    public GameObject LockPanel;
    public Text InfoText;
    public GameObject FinishPopUpRect;
    public GameObject InfoPopUpRect;

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

        DontDestroyOnLoad(this.gameObject);
        gameObject.SetActive(false);
    }

    public void RestartButtonClick()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void WatchButtonClick()
    {
        GameManager.instance.backButtonCounter = 5;
    }

}
