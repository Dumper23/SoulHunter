using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float speed = 2f;
    public float maxTime = 1f;

    private float time = 0;
    void Update()
    {
        time += Time.deltaTime;
        if (time <= maxTime)
        {
            transform.Translate(Vector2.up * Time.deltaTime * speed);
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }
}
