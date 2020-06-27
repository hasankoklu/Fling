using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RandomMovement());
        StartCoroutine(RandomRotate());
    }

    IEnumerator RandomRotate()
    {
        while (true)
        {
            transform.Rotate(Random.Range(0.2f, 0.7f), Random.Range(0.2f, 0.7f), Random.Range(0.2f, 0.7f));
            yield return new WaitForEndOfFrame();
        }

    }
    IEnumerator RandomMovement()
    {
        while (true)
        {
            for (int i = 0; i < 200; i++)
            {
                transform.Translate(Vector3.up * 0.001f);
                yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < 200; i++)
            {
                transform.Translate(Vector3.down * 0.001f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
