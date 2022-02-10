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
    public string[] lostSouls;
    public float speed;
    public int jumpAmount;
    public string currentLevel;

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
        currentLevel = player.currentLevel;

        int i = 0;
        LostSouls ls;
        lostSouls = new string[12];

        if (player.lostSouls.TryGetValue("Light", out ls))
        {
            lostSouls[i] = "Light";
            i++;
        }

        if (player.lostSouls.TryGetValue("Thorns", out ls))
        {
            lostSouls[i] = "Thorns";
            i++;
        }

        if (player.lostSouls.TryGetValue("Fireflies", out ls))
        {
            lostSouls[i] = "Fireflies";
            i++;
        }

        speed = player.playerVelocity;
        jumpAmount = player.maxJumps;
    }
   
}
