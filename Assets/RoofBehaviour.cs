using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofBehaviour : MonoBehaviour
{
    [SerializeField]
    private ChargerBehaviour cb;

    [SerializeField]
    private GameObject roofR,
        roofL;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        roofL.transform.localScale = new Vector3(cb.GetPercentage() / 10, 1, 1);
        roofR.transform.localScale = new Vector3(cb.GetPercentage() / 10, 1, 1);
    }
}
