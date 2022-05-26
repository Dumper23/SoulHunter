using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDemonMovement : MonoBehaviour
{

    private TargetJoint2D targetJ2D;

    [SerializeField]
    private float maxX,
        minX,
        maxY,
        minY,  
        maxMoveTimer = 5f,
        minMoveTimer = 1f;

    private Vector2 targetPos;

    private float moveTimer = 3f,
        startTimer = 0;

    private bool wantToMove = false;

    // Start is called before the first frame update
    void Start()
    {
        targetJ2D = GetComponent<TargetJoint2D>();
        //ChangePos();
    }

    // Update is called once per frame
    void Update()
    {
        if (wantToMove) {
            if (targetJ2D == null)
            {
                Debug.Log("No target!");
            }
            if (startTimer >= moveTimer)
            {
                ChangePos();
                startTimer = 0;
                moveTimer = Random.Range(maxMoveTimer, minMoveTimer);
            }
            else
            {
                startTimer += Time.deltaTime;
            }
        }
    }

    private void ChangePos()
    {
        float posX;
        float posY;

        posX = Random.Range(maxX, minX);
        posY = Random.Range(maxY, minY);

        targetPos = new Vector2(posX, posY);
        targetJ2D.target = targetPos;
    }

    public void WantMove(bool b)
    {
        wantToMove = b;
        /*if (!b)
        {
            targetJ2D.target = transform.position;
        }*/
    }

    public void SetPos(float posX, float posY)
    {
        targetPos = new Vector2(posX, posY);
        targetJ2D.target = targetPos;
    }

    public void SetNewArea(float maX, float miX, float maY, float miY)
    {
        maxX = maX;
        minX = miX;
        maxY = maY;
        minY = miY;
    }
}
