using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{

    float speed;

    private void Start()
    {
        speed = Random.Range(0.003f, 0.015f);
    }

    void Update()
    {
        if (transform.position.x > 14)
        {
            transform.position = new Vector3(-5f, transform.position.y, transform.position.z);
            speed = Random.Range(0.003f, 0.015f);
        }
        else
        {
            transform.Translate(speed, 0f, 0f);
        }
    }
}
