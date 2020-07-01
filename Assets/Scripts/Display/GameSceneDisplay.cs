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
    public GameObject FinishPopUpRect;

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
        SceneManager.LoadScene(0);
    }

}
