using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class PowerUps {


    //|==========================================|
    //         Start of Advanced Mode. 
    //|==========================================|

    // This Script will contain all the functions related to the Advanced-Mode.
    // Therefore this will include the scripting related to each ability/Power-up that we are planning to add in this mode (Check Initial Presentation/Game Plan)
    // We also must consider the addition of an "Power-UP panel".
    

    //============================================
    //               POWER-UPS
    //============================================

    private string name; // Name
    private string desc; // Description
    private Sprite icon; // IMG

    private List<PowerUpsBehaviour> behaviour;
    private bool requiresT;
    private int cooldown; 
    
    //Different Constructors for Different Abilitie (Just the Current Approach)
    public PowerUps(string aname, sprite ic, List<PowerUpsBehaviour> abehaviour )
    {
       name = aname;
       icon = ic;
       behaviour = new List <PowerUpsBehaviour>();
       behaviour = abehaviour;
       cooldown = 0;
       requiresT = false; //Requires Target
       desc = "N/A" ; //Add Description based on Behaviours
    }

    public PowerUps(string aname, string adesc, sprite ic, List<PowerUpsBehaviour> abehaviour, bool arequiresT, int acooldown )
    {
       name = aname;
       icon = ic;
       behaviour = new List <PowerUpsBehaviour>();
       behaviour = abehaviour;
       cooldown = acooldown;
       requiresT = arequiresT;
       desc = adesc ;//Add Description based on Behaviours
    }

    public string PowerUpName
    {
        get {return name;}
    }
    
    public string PowerUpDescription
    {
        get {return desc;}
    }

    public Sprite PowerUpIcon
    {
        get {return icon;}
    }
    

     public int PowerUpCooldown
    {
        get {return cooldown;}
    }
   
    public List<PowerUpsBehaviour> PowerUpsBehaviours
    {
        get {return behaviour;}
    }
   

   //=====================================================
   public void UsePowerUp(){}

}

*/