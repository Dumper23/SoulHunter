using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [Header("Camera settings")]
    public Vector3 offset = new Vector3(0, 0, -10);
    [Range(1, 10)]
    public float smoothFactor;
    public GameObject target;
    public bool follow = true;

    void FixedUpdate()
    {
        if (follow)
        {
            cameraSmoothing();
        }

    }

    void cameraSmoothing()
    {
        Vector3 targetPos = target.transform.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothFactor * Time.deltaTime);
        transform.position = smoothPos;
    }
}
