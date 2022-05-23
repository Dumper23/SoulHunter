using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPFunctionality : MonoBehaviour
{
    [SerializeField]
    private Transform startPlayer;

    [SerializeField]
    private Transform[] lavaSpots;

    [SerializeField]
    private GameObject end,
        lava,
        lavaFace;

    [SerializeField]
    private float lavaSpeed,
        lavaFaceSpeed;

    [SerializeField]
    private bool isHorizontal;

    private Transform player;

    private bool isActivated;
    private bool finished;

    private Vector3 playerSavedPos;
    private Vector3 cameraSavedPos;

    [SerializeField]
    private AudioSource audio;

    [SerializeField]
    private AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        isActivated = false;
        finished = false;
        player = GameObject.FindObjectOfType<playerController>().gameObject.transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated && !finished) {
            if (end.GetComponent<RedPortal>().endTP())
            {
                endTP();
            }
            if (isHorizontal)
            {
                lava.transform.position = Vector3.MoveTowards(lava.transform.position,new Vector3( lavaSpots[1].position.x, lava.transform.position.y, lava.transform.position.z), lavaSpeed * Time.deltaTime);
                lavaFace.transform.position = Vector3.MoveTowards(lavaFace.transform.position,new Vector3(lavaFace.transform.position.x, player.position.y, lavaFace.transform.position.z), lavaFaceSpeed * Time.deltaTime);
                
            }
            else
            {
                lava.transform.position = Vector3.MoveTowards(lava.transform.position, new Vector3(lava.transform.position.x, lavaSpots[1].position.y, lava.transform.position.z), lavaSpeed * Time.deltaTime);
                lavaFace.transform.position = Vector3.MoveTowards(lavaFace.transform.position, new Vector3(player.position.x, lavaFace.transform.position.y, lavaFace.transform.position.z), lavaFaceSpeed * Time.deltaTime);
            }
        }
    }

    public void startTP()
    {
        isActivated = true;
        playerSavedPos = player.transform.position;
        cameraSavedPos = Camera.main.GetComponent<cameraMovement>().getPos();
        Camera.main.GetComponent<cameraMovement>().setPos(startPlayer.position);
        player.transform.position = startPlayer.position;
        audio.clip = clips[Random.Range(0, clips.Length)];
        audio.Play();
    }

    private void endTP()
    {
        player.transform.position = playerSavedPos;
        Camera.main.GetComponent<cameraMovement>().setPos(cameraSavedPos);
        finished = true;
    }

    public Vector3 getStartPosition()
    {
        return startPlayer.position;
    }

    public bool hasBeenActivated()
    {
        return isActivated;
    }

    public bool hasFinished()
    {
        return finished;
    }
}
