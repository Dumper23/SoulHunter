using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    public GameObject dialogPanel;
    public Text dialogText;

    public List<GameObject> imagesToDisplay;

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

            foreach (GameObject img in imagesToDisplay)
            {
                img.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            foreach (GameObject img in imagesToDisplay)
            {
                img.SetActive(false);
            }
            dialogPanel.SetActive(false);
        }
    }
}
