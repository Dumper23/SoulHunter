using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyObject : MonoBehaviour
{
    public float timeToDestroy = 3f;
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

}
