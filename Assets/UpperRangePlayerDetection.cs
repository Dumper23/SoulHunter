using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperRangePlayerDetection : MonoBehaviour
{
    private bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            isPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            isPlayer = false;
        }
    }

    public bool IsInRange()
    {
        return isPlayer;
    }
}
