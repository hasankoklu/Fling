using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossProcessDisplay : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
