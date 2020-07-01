using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabToPlay : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(GameManager.instance.GoToNextLevelButtonClick);
    }
}
