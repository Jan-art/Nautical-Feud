using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUps : MonoBehaviour
{

    public GameObject GameManager;
    public GameObject PowerUpToggle;
    public AudioSource RadarSound;
    public AudioSource NukeSound;
    public AudioSource SatelliteSound;

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

        public PowerUpType getPowerUpType()
        {
            return type;
        }

        public PowerUpAlign getPowerUpAlign()
        {
            return alignment;
        }

        public PowerUpHitable getPowerUpHitable()
        {
            return targetable;
        }

        public bool getDefinitive()
        {
            return definitive;
        }

        public PowerUpType type;
        public PowerUpAlign alignment;
        public PowerUpHitable targetable;
        public bool definitive;
        public int cooldown;
        public int startCooldown;
        public int scoreCost;

        public string getStringAlignment()
        {
            if (alignment == PowerUpAlign.OFFENSE)
            {
                return "offense";
            }
            else
            {
                return "defense";
            }
        }

        public string getStringHitable()
        {
            if (targetable == PowerUpHitable.ANYWHERE)
            {
                return "anywhere";
            }
            else if (targetable == PowerUpHitable.NOTHIT)
            {
                return "notHit";
            }
            else
            {
                return "ships";
            }
        }


    }

    public int numberOfPowerUps = 3;
    public PowerUp[] powerups = new PowerUp[3];
    public Button[] powerUpButtons = new Button[3];
    public Button[] purchaseButtons = new Button[3];
    public bool[] usable = new bool[3];
    public int lastDisabled = -1;
    public int score;
    public Text scoreText;
    //MISSILE
    public GameObject missilePrefab;
    float altitude = 3f;
    float Timer;

    public GameObject explosionPrefab;
    public GameObject sonarPrefab;

    bool soundActive = false;

    void Start()
    {
        InitialiseUsable();
    }

    public void ToGameManager(int listPosition)
    {
        PowerUpToggle.GetComponent<ToggleScript>().Toggle();
        string alignment = powerups[listPosition].getStringAlignment();
        string hitable = powerups[listPosition].getStringHitable();
        Debug.Log("String created and being sent to GameManager" + alignment + hitable);
        if (lastDisabled != -1)
        {
            powerUpButtons[lastDisabled].interactable = true;
        }
        lastDisabled = listPosition;
        powerUpButtons[listPosition].interactable = false;
        GameManager.GetComponent<GameManager>().EnablePowerUp(listPosition, alignment, hitable, powerups[listPosition].definitive);
    }

    public PowerUp returnPowerUp(int listPosition)
    {
        return powerups[listPosition];
    }

    IEnumerator RadarFunctionality(int x, int z, int listPosition, int rival)
    {
        //Need to get correct gameboard from gamemanager so that tiles can be accessed and changed

        //Can use count if some text stating how many ships are sround it will be displayed
        TileInfo info;
        int count = 0;
        RadarSound.Play();

        info = GameManager.GetComponent<GameManager>().players[rival].pgb.TileInfoRequest(x, z);
        Vector3 aimPos = info.gameObject.transform.position;
        GameObject Sonar = Instantiate(sonarPrefab, aimPos, Quaternion.identity);
        
        for (int i = x - 1; i < x + 2; i++)
        {
            if (i < 10 && i > -1)
            {
                for (int j = z - 1; j < z + 2; j++)
                {
                    if (j < 10 && j > -1)
                    {
                        info = GameManager.GetComponent<GameManager>().players[rival].pgb.TileInfoRequest(i, j);
                        if (!info.GetHit())
                        {
                            if (GameManager.GetComponent<GameManager>().players[rival].myGrid[i, j].IsOccupied() && GameManager.GetComponent<GameManager>().players[rival].revealGrid[i, j] == false)
                            {
                                count++;

                                //Can use activate top if a unique sprite has been made to fit it
                                info.ActivateTop(4, false);
                            }
                            else
                            {
                                
                                info.ActivateTop(2, true);
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("Number of tiles with ships detected by radar" + count);
        lastDisabled = -1;

        yield return new WaitForSeconds(1f);
        Destroy(Sonar);
        lastDisabled = -1;
        yield break;
    }

    public void SateliteFunctionality(int x, int z, int listPosition, int rival)
    {
        TileInfo info;
        info = GameManager.GetComponent<GameManager>().players[rival].pgb.TileInfoRequest(x, z);
        SatelliteSound.Play();
        if (GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z].IsOccupied())
        {
            //Damage SHIP

            bool sunk = GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z].placedShip.AbsorbDamage();
            GameManager.GetComponent<GameManager>().players[rival].revealGrid[x, z] = true;

            if (sunk)
            {
                GameManager.GetComponent<GameManager>().players[rival].placedShipList.Remove(GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z].placedShip.gameObject);
                GameManager.GetComponent<GameManager>().shipTextUpdate(rival);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (x - i > -1)
                    {
                        if (GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z].getOccupation() == GameManager.GetComponent<GameManager>().players[rival].myGrid[x - i, z].getOccupation())
                        {

                            if (GameManager.GetComponent<GameManager>().players[rival].revealGrid[x - i, z] == false)
                            {
                                SateliteFunctionality(x - i, z, listPosition, rival);
                            }
                        }
                    }
                    if (x + i < 10)
                    {
                        if (GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z].getOccupation() == GameManager.GetComponent<GameManager>().players[rival].myGrid[x + i, z].getOccupation())
                        {

                            if (GameManager.GetComponent<GameManager>().players[rival].revealGrid[x + i, z] == false)
                            {
                                SateliteFunctionality(x + i, z, listPosition, rival);
                            }
                        }
                    }
                    if (z - i > -1)
                    {
                        if (GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z].getOccupation() == GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z - i].getOccupation())
                        {

                            if (GameManager.GetComponent<GameManager>().players[rival].revealGrid[x, z - i] == false)
                            {
                                SateliteFunctionality(x, z - i, listPosition, rival);
                            }
                        }
                    }
                    if (z + i < 10)
                    {
                        if (GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z].getOccupation() == GameManager.GetComponent<GameManager>().players[rival].myGrid[x, z + i].getOccupation())
                        {

                            if (GameManager.GetComponent<GameManager>().players[rival].revealGrid[x, z + i] == false)
                            {
                                SateliteFunctionality(x, z + i, listPosition, rival);
                            }
                        }
                    }
                }
            }

            //HIGHLIGHT TILE
            //ADD [EXPLOSION + SOUND HERE]
            info.ActivateTop(3, true);
        }
        else
        {
            //ADD [EXPLOSION + SOUND HERE]

            //NOT HIT
            info.ActivateTop(2, true);
        }
        lastDisabled = -1;
    }

    IEnumerator NukeFunctionality(int x, int z, int listPosition, int rival)
    {
        Debug.Log("Nuke Functionality running");
        TileInfo info;

        info = GameManager.GetComponent<GameManager>().players[rival].pgb.TileInfoRequest(x, z);
        //MISSILE
        Vector3 startPos = Vector3.zero;
        Vector3 aimPos = info.gameObject.transform.position;

        GameObject missile = Instantiate(missilePrefab, startPos, Quaternion.identity);
        Debug.Log("Missile instantiated");
        while (MoveToTile(startPos, aimPos, 0.5f, missile))
        {
            yield return null;
        }

        Destroy(missile);
        Timer = 0; //Reset missile timer
        GameObject explosion = Instantiate(explosionPrefab, aimPos, Quaternion.identity);

        for (int i = x - 1; i < x + 2; i++)
        {
            if (i < 10 && i > -1)
            {
                for (int j = z - 1; j < z + 2; j++)
                {
                    if (j < 10 && j > -1)
                    {
                        info = GameManager.GetComponent<GameManager>().players[rival].pgb.TileInfoRequest(i, j);
                        if (GameManager.GetComponent<GameManager>().players[rival].myGrid[i, j].IsOccupied())
                        {
                            if (!info.GetHit())
                            {
                                //Damage SHIP
                                bool sunk = GameManager.GetComponent<GameManager>().players[rival].myGrid[i, j].placedShip.AbsorbDamage();

                                if (sunk)
                                {
                                    GameManager.GetComponent<GameManager>().players[rival].placedShipList.Remove(GameManager.GetComponent<GameManager>().players[rival].myGrid[i, j].placedShip.gameObject);
                                    GameManager.GetComponent<GameManager>().shipTextUpdate(rival);
                                }

                                //HIGHLIGHT TILE
                                //ADD [EXPLOSION + SOUND HERE]
                                info.ActivateTop(3, true);
                            }

                        }
                        else
                        {   //HIGHLIGHT TILE
                            //ADD [EXPLOSION + SOUND HERE]

                            //NOT HIT
                            info.ActivateTop(2, true);
                        }

                        //REVEAL TILE
                        GameManager.GetComponent<GameManager>().players[rival].revealGrid[i, j] = true;

                    }
                    else
                    {
                        //Pass
                    }
                }

            }
            else
            {
                //Pass
            }
        }
        yield return new WaitForSeconds(1f);
        Destroy(explosion);
        lastDisabled = -1;
        yield break;
    }


    bool MoveToTile(Vector3 startPos, Vector3 aimPos, float speed, GameObject missile)
    {
        Debug.Log("MoveToTile running");
        Timer += speed * Time.deltaTime;
        Vector3 nextPos = Vector3.Lerp(startPos, aimPos, Timer);
        nextPos.y = altitude * Mathf.Sin(Mathf.Clamp01(Timer) * Mathf.PI);
        missile.transform.LookAt(nextPos);

        return aimPos != (missile.transform.position = Vector3.Lerp(missile.transform.position, nextPos, Timer));
    }

    public void ActivatePowerUp(int x, int z, int listPosition, int rival)
    {
        if (listPosition == 1)
        {

            StartCoroutine(RadarFunctionality(x, z, listPosition, rival));

        }
        else if (listPosition == 0)
        {
            this.SateliteFunctionality(x, z, listPosition, rival);
        }
        else if (listPosition == 2)
        {
            soundActive = false;
            StartCoroutine(NukeFunctionality(x, z, listPosition, rival));
            if (!soundActive)
            {
                Debug.Log("if (!soundActive) running");
                NukeSound.Play();
                soundActive = true;
            }
        }
    }

    public void EnableButton(int listPosition)
    {
        powerUpButtons[listPosition].interactable = true;
    }

    public void DisableButton(int listPosition)
    {
        powerUpButtons[listPosition].interactable = false;
    }

    /*
    public void TickCooldowns()
    {
        for (int i = 0; i < powerups.Length; i++)
        {
            if (usable[i] == false)
            {
                if (powerups[i].cooldown > 0)
                {
                    powerups[i].cooldown -= 1;
                }
                if (powerups[i].cooldown == 0)
                {
                    Debug.Log("PowerUp " + i + "renabled");
                    EnablePower(i);
                }
                Debug.Log("Cooldown for current PowerUp: " + powerups[i].cooldown);
            }
        }
    }
    */

    //Will be called to intialise setUsable and change PowerUp cooldowns. Any powers-up not permitted due to low level should have cooldowns changed to -1 and usable set to false
    public void InitialiseUsable()
    {
        for (int i = 0; i < usable.Length; i++)
        {
            DisablePower(i);
        }
        SetPurchaseButtons();
    }

    public void SetPurchaseButtons()
    {
        for (int p = 0; p < usable.Length; p++)
        {
            if (score >= powerups[p].scoreCost && usable[p] == false)
            {
                purchaseButtons[p].interactable = true;
            }
            else
            {
                purchaseButtons[p].interactable = false;
            }
        }
    }

    public void DisablePower(int listPosition)
    {
        Debug.Log("DisablePower called for powerup" + listPosition);
        usable[listPosition] = false;
        powerUpButtons[listPosition].interactable = false;
    }

    public void EnablePower(int listPosition)
    {
        usable[listPosition] = true;
        powerUpButtons[listPosition].interactable = true;
        //powerups[listPosition].cooldown = powerups[listPosition].startCooldown;
    }

    public void IncreaseScore(int increase)
    {
        Debug.Log("Score increased by" + increase);
        score += increase;
        ChangeScoreText();
    }

    public void DecreaseScore(int decrease)
    {
        Debug.Log("Score decreased by" + decrease);
        score -= decrease;
        ChangeScoreText();
    }

    public void PurchaseButtonPressed(int index)
    {
        DecreaseScore(powerups[index].scoreCost);
        SetPurchaseButtons();
        purchaseButtons[index].interactable = false;
        powerUpButtons[index].interactable = true;
        usable[index] = true;

    }

    public void ChangeScoreText()
    {
        Debug.Log("ChangeScoreText running");
        scoreText.text = "Score: " + score.ToString();
    }
}
