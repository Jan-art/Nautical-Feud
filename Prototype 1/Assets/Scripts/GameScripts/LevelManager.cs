using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{

    //============================================================
    // Here we will script how the levelling works in our game
    //============================================================


//sets variables for xp, level, xp needed to level
    private int xp;
    private int level;
    private int xpToNextLevel;
    //constructor initialises
    public LevelManager() {
        level = 0;
        xp = 0;
        xpToNextLevel = 100;

    }

//adds xp, if xp goes over the amount for next level then level increases, then - the xp for next level as it has been reset.
    public void addXP(int amount) {
    xp += amount;
    if (xp >= xpToNextLevel) {
        level++;
        xp -= xpToNextLevel;
    }
    }

    //gets and returns level

    public int getLevel() {
        return level;
    }

    //gets and returns xp as a normalised float, this is used for the xp bar in LevelWindow
    
    public float getXp() {
        return (float) (xp / xpToNextLevel);
    }
}
