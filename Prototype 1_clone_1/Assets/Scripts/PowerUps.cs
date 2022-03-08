using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField]
    GameObject GameManager;

    [System.Serializable]
    public class PowerUp
    {
        public enum PowerUpType
        {
            NUKE,
            ARMOUR,
            SATELITE,
            RADAR
        }

        public enum PowerUpAlign
        {
            DEFENSE,
            OFFENSE
        }

        public enum PowerUpHitable
        {
            ANYWHERE,
            NOTHIT,
            SHIPS
        }

        public PowerUpType type;
        public PowerUpAlign alignment;
        public PowerUpHitable targetable;

        public PowerUp()
        {
            Debug.Log("PowerUp created");
        }
    }

    public int numberOfPowerUps = 4;
    public PowerUp[] powerups = new PowerUp[4];
}
