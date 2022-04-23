using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData{
    public int deaths;
    public int points;
    public int lives;
    public int attackDamage;
    public float attackRate;
    public float[] position;
    public string[] lostSouls;
    public float speed;
    public int jumpAmount;
    public string currentLevel;
    public string[] equippedLostSouls;

    public PlayerData(playerController player)
    {

        points = GameManager.Instance.getPoints();
        lives = player.playerLives;
        attackDamage = player.attackDamage;
        attackRate = player.attackRate;
        deaths = player.deaths;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
        currentLevel = player.currentLevel;

        int i = 0;
        int j = 0;
        LostSouls ls;
        equippedLostSouls = new string[3];
        lostSouls = new string[12];

        if (player.lostSouls.TryGetValue("Light", out ls))
        {
            lostSouls[i] = "Light";
            if (ls.isEquiped) {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }

        if (player.lostSouls.TryGetValue("Thorns", out ls))
        {
            lostSouls[i] = "Thorns";
            if (ls.isEquiped)
            {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }

        if (player.lostSouls.TryGetValue("Fireflies", out ls))
        {
            lostSouls[i] = "Fireflies";
            if (ls.isEquiped)
            {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }

        if (player.lostSouls.TryGetValue("StoneBreaker", out ls))
        {
            lostSouls[i] = "StoneBreaker";
            if (ls.isEquiped)
            {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }

        if (player.lostSouls.TryGetValue("OutBurst", out ls))
        {
            lostSouls[i] = "OutBurst";
            if (ls.isEquiped)
            {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }

        if (player.lostSouls.TryGetValue("HardSkin", out ls))
        {
            lostSouls[i] = "HardSkin";
            if (ls.isEquiped)
            {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }

        if (player.lostSouls.TryGetValue("SoulKeeper", out ls))
        {
            lostSouls[i] = "SoulKeeper";
            if (ls.isEquiped)
            {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }

        if (player.lostSouls.TryGetValue("DeflectMissiles", out ls))
        {
            lostSouls[i] = "DeflectMissiles";
            if (ls.isEquiped)
            {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }

        if (player.lostSouls.TryGetValue("HolyWater", out ls))
        {
            lostSouls[i] = "HolyWater";
            if (ls.isEquiped)
            {
                equippedLostSouls[j] = ls.lostSoulName;
                j++;
            }
            i++;
        }



        speed = player.playerVelocity;
        jumpAmount = player.maxJumps;
    }
   
}
