using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            GameManager.Instance.nextLevel(nextLevelName, collision.GetComponent<playerController>());
        }
    }
}
