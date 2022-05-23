using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesFollowing : MonoBehaviour
{

    public Transform target;
    public float timeToDestroy;

    public void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }

    void Update()
    {
        transform.position = target.position;
    }
}
