using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    public GameObject dialogPanel;
    public Text dialogText;
    [TextArea]
    public string textToShow;

    void Start()
    {
        dialogText.text = textToShow;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            dialogText.text = textToShow;
            dialogPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            dialogPanel.SetActive(false);
        }
    }
}
