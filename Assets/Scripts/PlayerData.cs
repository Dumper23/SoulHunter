using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData{

    public int points;
    public int lives;
    public int attackDamage;
    public float attackRate;
    public float[] position;
    public float speed;
    public int jumpAmount;

    public PlayerData(playerController player)
    {
        points = GameManager.Instance.getPoints();
        lives = player.playerLives;
        attackDamage = player.attackDamage;
        attackRate = player.attackRate;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        speed = player.playerVelocity;
        jumpAmount = player.maxJumps;
    }
   
}
