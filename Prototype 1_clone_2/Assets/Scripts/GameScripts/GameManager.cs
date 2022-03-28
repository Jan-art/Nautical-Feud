using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using PlayFab;
using PlayFab.ClientModels;


public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GameManager instance;


    //Proton Event Codes used to identify what reaction needs to be taken to the information recieved
    public const byte OnTileSelected = 1;
    public const byte OnShipPlacementFinished = 2;
    public const byte OnVictory = 3;
    public const byte OnPowerUpUsed = 4;

    [System.Serializable]
    public class Player
    {
        public enum PlayerType //In future versions of the game, can add NPC players that are controlled by AI. 
        {
            HUMAN
        }

        public List<GameObject> placedShipList = new List<GameObject>();
        public PlayerType playerType;
        public Tile[,] myGrid = new Tile[10, 10];
        public bool[,] revealGrid = new bool[10, 10];
        public PhysicalGameBoard pgb;
        public int rival;


        //public LayerMask layerToPlaceOn;

        [Space]
        public GameObject cameraPos;
        public GameObject placePanel;
        public GameObject shootPanel;
        public GameObject enemyTurn;

        //SHOW & HIDE SHIPS

        public Player()
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    OccupationType t = OccupationType.EMPTY;
                    myGrid[x, y] = new Tile(t, null);
                    revealGrid[x, y] = false;
                }
            }
            if (PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    rival = 1;
                }
                else
                {
                    rival = 0;
                }
            }


        }
    }


    int activePlayer; //Track current Turn
    public Player[] players = new Player[2];


    //STATE MACHINE
    public enum GameStates
    {
        P1_PLACE_SHIPS,
        P2_PLACE_SHIPS,
        KILL,
        IDLE
    }

    public GameStates gameState;
    public GameObject WarCamPos;
    bool CamMoved;
    public GameObject placingCanvas;

    //Game Objects related to advanced mode
    [Space]
    public GameObject PowerUpToggle;
    public GameObject AdvModeCheck;
    public GameObject PowerUpCanvas;
    public GameObject PowerUpBar;

    //Game Objects related to HUD
    [Space]
    public GameObject hudCanvasP1;
    public Text ownShipsP1;
    public Text enemyShipsP1;

    public GameObject hudCanvasP2;
    public Text ownShipsP2;
    public Text enemyShipsP2;

    //MISSILE
    public GameObject missilePrefab;
    float altitude = 3f;
    float Timer;

    //ADD SPEED float

    bool isShooting;
    bool validPowerUpUse;

    //Variables associated with power ups
    public bool isPowerUpActive;
    private int listPosition;
    private string alignment;
    private string hitable;
    private bool definitive;


    private int wins;
    private int losses;
    private int shipsSunk;
    private bool won;
    private int shipsDestroyed;

    void Awake()
    {
        instance = this;
        AdvModeCheck = GameObject.FindGameObjectWithTag("AdvModeCheck");
    }

    void Start()
    {
        //Can be commented out and code should run classic mode fine
        if (AdvModeCheck.GetComponent<AdvanceMC>().getAMC() == true)
        {
            PowerUpCanvas.GetComponent<PowerUps>().InitialiseUsable();
            PowerUpToggle.SetActive(false);
            Debug.Log("Game loaded in advanced mode");
        }
        else
        {
            PowerUpToggle.SetActive(false);
            Debug.Log("Game loaded in classic mode");
        }

        //HIDE PANELS
        HideAllPanels();

        placingCanvas.SetActive(false);

        //ACTIVATE PLACE PANEL P1
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                players[activePlayer].placePanel.SetActive(true);
            }
        }
        else
        {
            players[activePlayer].placePanel.SetActive(true);
        }

        gameState = GameStates.IDLE;

        //MOVE CAMERA
    }



    void AddShipToList(GameObject placedShip)
    {
        players[activePlayer].placedShipList.Add(placedShip);
    }

    //Used by PlaceSystemManual to set which tiles a ship is on
    public void UpdateGrid(Transform shipTransform, ShipBehaviour ship, GameObject placedShip)
    {
        foreach (Transform child in shipTransform)
        {
            TileInfo tInfo = child.GetComponent<GhostBehaviour>().GetTileInfo();
            players[activePlayer].myGrid[tInfo.xPos, tInfo.zPos] = new Tile(ship.type, ship);
        }

        AddShipToList(placedShip);
        //DebugGrid();
    }

    //Used by PlaceSystemEvent to set which tiles a ship is on
    public void UpdateGrid(Transform shipTransform, ShipBehaviour ship, GameObject placedShip, int xTile, int zTile, string shipRotation)
    {
        Debug.Log("UpdateGrid running for PlaceSystemEvent");

        //Loops through the tiles the ship will be on to register the ship with them
        for (int i = 0; i < ship.shipLength; i++)
        {
            TileInfo tInfo = players[activePlayer].pgb.TileInfoRequest(xTile, zTile);
            players[activePlayer].myGrid[tInfo.xPos, tInfo.zPos] = new Tile(ship.type, ship);

            //Changes whether the row or column number is incremented to account for the rotation of the ship
            if (shipRotation.Equals("down") || shipRotation.Equals("up"))
            {
                zTile++;
            }
            else
            {
                xTile++;
            }
        }
        AddShipToList(placedShip);
        //DebugGrid();
    }

    public bool CheckIfOccupied(int xPos, int zPos)
    {
        return players[activePlayer].myGrid[xPos, zPos].IsOccupied();
    }


    public void RemoveAllShipsFromList()
    {
        foreach (GameObject ship in players[activePlayer].placedShipList)
        {
            Destroy(ship);
        }
        players[activePlayer].placedShipList.Clear();

        InitialiseGrid();
    }

    void InitialiseGrid()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                OccupationType t = OccupationType.EMPTY;
                players[activePlayer].myGrid[x, y] = new Tile(t, null);
                players[activePlayer].revealGrid[x, y] = false;
            }
        }
    }


    ///[GAME BATTLE SCRIPT]


    void Update()
    {
        switch (gameState)
        {
            case GameStates.P1_PLACE_SHIPS:
                {
                    //DEACTIVATE PANEL
                    players[activePlayer].placePanel.SetActive(false);
                    if (PhotonNetwork.IsConnected)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            placingCanvas.SetActive(true);
                            PlaceSystemManual.instance.SetPlayerField(players[activePlayer].pgb, players[activePlayer].playerType.ToString());
                        }
                    }
                    else
                    {
                        PlaceSystemManual.instance.SetPlayerField(players[activePlayer].pgb, players[activePlayer].playerType.ToString());
                    }
                    StartCoroutine(MoveCamera(players[activePlayer].cameraPos)); //First Camera Action
                    gameState = GameStates.IDLE;
                }
                break;
            case GameStates.IDLE: //WAIT-TIME
                {
                    if (PhotonNetwork.IsConnected)
                    {
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            if (activePlayer == players[0].rival)
                            {
                                players[0].enemyTurn.SetActive(true);
                            }
                            else
                            {
                                players[0].enemyTurn.SetActive(false);
                            }
                        }
                        else
                        {
                            if (activePlayer == players[1].rival)
                            {
                                players[1].enemyTurn.SetActive(true);
                            }
                            else
                            {
                                players[1].enemyTurn.SetActive(false);
                            }
                        }
                    }
                }
                break;
            case GameStates.P2_PLACE_SHIPS:
                {
                    //DEACTIVATE PANEL
                    players[activePlayer].placePanel.SetActive(false);
                    if (PhotonNetwork.IsConnected)
                    {
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            //MOVE CAM
                            StartCoroutine(MoveCamera(players[activePlayer].cameraPos));
                            placingCanvas.SetActive(true);
                            players[0].placePanel.SetActive(false);
                            PlaceSystemManual.instance.SetPlayerField(players[activePlayer].pgb, players[activePlayer].playerType.ToString());
                        }
                        else
                        {
                            placingCanvas.SetActive(false);
                        }
                    }
                    else
                    {
                        PlaceSystemManual.instance.SetPlayerField(players[activePlayer].pgb, players[activePlayer].playerType.ToString());
                    }
                    gameState = GameStates.IDLE;
                }
                break;
            case GameStates.KILL:
                {
                    //WARFARE
                }
                break;
        }
    }

    void HideAllPanels()
    {
        players[0].placePanel.SetActive(false);
        players[0].shootPanel.SetActive(false);

        players[1].placePanel.SetActive(false);
        players[1].shootPanel.SetActive(false);
    }

    //PLACE PANEL BTN P1
    public void P1PlaceShips()
    {
        gameState = GameStates.P1_PLACE_SHIPS;

    }

    //PLACE PANEL BTN P2
    public void P2PlaceShips()
    {
        gameState = GameStates.P2_PLACE_SHIPS;
    }

    //READY BTN
    public void SelectReady()
    {
        if (activePlayer == 0)
        {

            //HIDE SHIPS
            //HideAllShips();



            //PHOTON EVENT TO NOTIFIY OTHER PLAYER, ONLY USED BY PLAYER 1 (MASTER CLIENT)
            if (PhotonNetwork.IsMasterClient)
            {
                sendOnShipPlacementFinishedEvent();
                StartCoroutine(MoveCamera(WarCamPos));
                placingCanvas.SetActive(false);
                SwitchPlayer();
            }
            else
            {
                //SWITCH Player 
                SwitchPlayer();
                //ACTIVATE P2 PANELS
                players[activePlayer].placePanel.SetActive(true);
            }

            //RETURN
            return;
        }

        if (activePlayer == 1)
        {
            //HIDE SHIPS
            //HideAllShips();

            //MOVE CAM
            StartCoroutine(MoveCamera(WarCamPos));

            //PHOTON EVENT TO NOTIFIY OTHER PLAYER, ONLY USED BY PLAYER 2 (NOT MASTER CLIENT)
            if (!PhotonNetwork.IsMasterClient)
            {
                sendOnShipPlacementFinishedEvent();
                //TURN_OFF PLACING CANVAS
                placingCanvas.SetActive(false);
                SwitchPlayer();
                hudCanvasP2.SetActive(true);
            }
            else
            {
                //SWITCH Player
                SwitchPlayer();
                //ACTIVATE P1 KILL PANELS
                players[activePlayer].shootPanel.SetActive(true);
                //placingCanvas.SetActive(false);
            }
        }
    }


    void sendOnShipPlacementFinishedEvent()
    {
        object[] content = new object[28]; //NEEDS TO BE CHANGED FOR ACTUAL CODE
        int index = 0;
        string rotated = "";
        Vector3 shipRotation = new Vector3(0, 0, 0);
        string occupationType;
        bool flag = false;
        //Loops through all tiles and if they are occupied adds the information to a object array that will be sent to the other player
        for (int i = 0; i < 10; i++)
        {
            for (int k = 0; k < 10; k++)
            {
                if (players[activePlayer].myGrid[i, k].IsOccupied())
                {
                    //If a tile is occupied adds what occupies it, tile coordinates and its rotation to the locations list
                    flag = false;
                    occupationType = players[activePlayer].myGrid[i, k].getOccupationString();
                    content[index] = occupationType;
                    index++;
                    content[index] = i;
                    index++;
                    content[index] = k;
                    index++;
                    /*If occupied by a corvette has the systemregister it as rotated down otherwise looks at surrounding tiles to figure out
                        the ships rotation */
                    if (!occupationType.Equals("CORVETTE"))
                    {

                        for (int ships = 0; ships < players[activePlayer].placedShipList.Count; ships++)
                        {
                            if (players[activePlayer].placedShipList[ships].GetComponent<ShipBehaviour>().type == players[activePlayer].myGrid[i, k].getOccupation())
                            {
                                shipRotation = players[activePlayer].placedShipList[ships].GetComponent<Transform>().rotation * new Vector3(1, 1, 1);
                                Debug.Log("shipRotation: " + shipRotation);
                                break;
                            }
                        }
                        if (shipRotation == new Vector3(1f, 1f, 1f))
                        {
                            rotated = "up";
                        }
                        else if (shipRotation == new Vector3(-1.0f, 1.0f, 1.0f))
                        {
                            rotated = "left";
                        }
                        else if (shipRotation == new Vector3(-1f, 1f, -1f))
                        {
                            rotated = "down";
                        }
                        else if (shipRotation == new Vector3(1.0f, 1.0f, -1.0f))
                        {
                            rotated = "right";
                        }
                        else
                        {
                            //Important debug
                            Debug.Log("Else hit in sending ship rotations, error has occurred");
                            rotated = "down";
                        }
                    }
                    else
                    {
                        rotated = "down";
                    }
                    content[index] = rotated;
                    index++;
                    Debug.Log("Location added to list. Occupied by" + content[index - 4] + content[index - 3] + content[index - 2] + content[index - 1]);
                }
            }
        }
        //Sets the recivers (other player) and information (object array formed above) and then sends them using RaiseEvent
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(OnShipPlacementFinished, content, raiseEventOptions, SendOptions.SendReliable);
        Debug.Log("Event 'OnShipPlacementFinished' raised");
    }


    void HideAllShips()
    {
        foreach (var ship in players[activePlayer].placedShipList)
        {
            ship.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void UnHideAllShips()
    {
        foreach (var ship in players[activePlayer].placedShipList)
        {
            ship.GetComponent<MeshRenderer>().enabled = true;
        }
    }


    void SwitchPlayer()
    {
        activePlayer++;
        activePlayer %= 2;
    }

    IEnumerator MoveCamera(GameObject camObj)
    {
        if (CamMoved)
        {
            yield break;
        }

        CamMoved = true;

        float t = 0;
        float duration = 0.5f;

        Vector3 startPos = Camera.main.transform.position;
        Quaternion startRot = Camera.main.transform.rotation;

        Vector3 toPos = camObj.transform.position;
        Quaternion toRot = camObj.transform.rotation;

        while (t < duration)
        {
            t += Time.deltaTime;

            Camera.main.transform.position = Vector3.Lerp(startPos, toPos, t / duration);  //Duration always happens between "0 & 1". We need to define it as it can't be Higher than 1. 
                                                                                           //| 0 means 0%. | 0.5 means 50% | 1 means 100 % |

            Camera.main.transform.rotation = Quaternion.Lerp(startRot, toRot, t / duration);


            yield return null;
        }


        CamMoved = false;
    }

  

    //KILL PANEL BTN

    public void KillBtn()
    {
        // Should be commented out for actual release
        //UnHideAllShips();
        // Should be commented out for actual release

        players[activePlayer].shootPanel.SetActive(false);
        gameState = GameStates.KILL;
    }

    int Rival()
    {
        int ap = activePlayer;
        ap++;
        ap %= 2;

        int rival = ap;
        return rival;
    }

    public void CheckShot(int x, int z, TileInfo info)
    {
        StartCoroutine(IdentifyLocation(x, z, info));
    }

    IEnumerator IdentifyLocation(int x, int z, TileInfo info)
    {
        Debug.Log("IdentifyLocation running");
        if (isShooting)
        {
            yield break;
        }
        isShooting = true;
        int rival = Rival();

        if (!isPowerUpActive)
        {
            Debug.Log("No power up active during shot");
            //IF YOUR TILE

            if (!players[rival].pgb.RequestTile(info))
            {
                //print("LOL");
                isShooting = false;
                yield break;
            }

            //IF TILE IS ALREADY HIT
            if (players[rival].revealGrid[x, z] == true)
            {
                //print("Location already Hit");
                isShooting = false;
                yield break;
            }

            //IF SHOT IS FIRED AT THE RIVAL
            if (!(players[activePlayer] == players[rival]) && players[rival].placedShipList.Count != 0)
            {
                //Stores information about the tile hit and sends it to the other player so they can update their version
                object[] content = new object[] { x, z, rival };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(OnTileSelected, content, raiseEventOptions, SendOptions.SendReliable);
                Debug.Log("Event 'OnTileSelected' raised");
            }


            //MISSILE
            Vector3 startPos = Vector3.zero;
            Vector3 aimPos = info.gameObject.transform.position;

            GameObject missile = Instantiate(missilePrefab, startPos, Quaternion.identity);

            while (MoveToTile(startPos, aimPos, 0.5f, missile))
            {
                yield return null;
            }

            Destroy(missile);
            Timer = 0; //Reset missile timer

            //CHECK IF TILE BUSY
            if (players[rival].myGrid[x, z].IsOccupied())
            {
                //Damage SHIP

                bool sunk = players[rival].myGrid[x, z].placedShip.AbsorbDamage();

                if (sunk)
                {
                    players[rival].placedShipList.Remove(players[rival].myGrid[x, z].placedShip.gameObject);
                    shipTextUpdate(rival);
                }

                //HIGHLIGHT TILE
                //ADD [EXPLOSION + SOUND HERE]
                info.ActivateTop(3, true);

                if (PhotonNetwork.IsMasterClient && activePlayer == 0)
                {
                    PowerUpCanvas.GetComponent<PowerUps>().IncreaseScore(20);
                }
                else if (!(PhotonNetwork.IsMasterClient) && activePlayer == 1)
                {
                    PowerUpCanvas.GetComponent<PowerUps>().IncreaseScore(20);
                }
            }
            else
            {   //HIGHLIGHT TILE
                //ADD [EXPLOSION + SOUND HERE]

                //NOT HIT
                info.ActivateTop(2, true);

                if (PhotonNetwork.IsMasterClient && activePlayer == 0)
                {
                    PowerUpCanvas.GetComponent<PowerUps>().IncreaseScore(10);
                }
                else if (!(PhotonNetwork.IsMasterClient) && activePlayer == 1)
                {
                    PowerUpCanvas.GetComponent<PowerUps>().IncreaseScore(10);
                }
            }

            //REVEAL TILE
            players[rival].revealGrid[x, z] = true;

            //====================================================================
            //CHECK WIN STATUS

            CheckWinCondition(rival);
            yield return new WaitForSeconds(1f);

            //======================================================================

            // Should be commented out for actual release
            //HIDE SHIPS
            //HideAllShips();
            // Should be commented out for actual release

            //SWITCH PLAYER
            SwitchPlayer();
            if (AdvModeCheck.GetComponent<AdvanceMC>().getAMC() == true)
            {
                PowerUpToggle.SetActive(false);
                PowerUpBar.SetActive(false);
            }
            PlayerShootingSwitch();
        }
        else
        {
            //Used to track whether the conditions to activate a power up have been met. If it equals 2 by the end then the conditions have been met
            Debug.Log("Power Up active for shot");
            validPowerUpUse = false;


            // If fired at own board and ability is defensive
            if (alignment == "defense" && !players[rival].pgb.RequestTile(info))
            {
                Debug.Log("Power Up is defensive");
                //If tile is not already hit
                if (hitable == "ships" && !players[activePlayer].revealGrid[x, z] == true)
                {
                    Debug.Log("Power Up targetting own ships successful");
                    CallPowerUp(x, z, listPosition, info, rival, definitive);
                    validPowerUpUse = true;
                }
                //This is where code would be added if there were other friendly powerups
            }

            if (!players[rival].pgb.RequestTile(info))
            {
                //Necessary to prevent offensive ablilities hitting self
                isShooting = false;
                yield break;
            }

            //If fired at enemy board and ability is offensive
            if (alignment == "offense" && !(players[activePlayer] == players[rival]) && players[rival].placedShipList.Count != 0)
            {
                Debug.Log("Power Up is offensive");
                //If ability can target anywhere on board
                if (hitable == "anywhere")
                {
                    Debug.Log("Power Up targetting anywhere on enemy board successful");
                    CallPowerUp(x, z, listPosition, info, rival, definitive);
                    validPowerUpUse = true;
                }

                if (hitable == "notHit" && !players[rival].revealGrid[x, z] == true)
                {
                    Debug.Log("Power Up targetting non-hit enemy tiles successful");
                    CallPowerUp(x, z, listPosition, info, rival, definitive);
                    validPowerUpUse = true;
                }

                //Currently not puts 'hitable == "ships" in as no current abilities require it'

            }

            DisablePowerUp(listPosition);

            CheckWinCondition(rival);
            yield return new WaitForSeconds(1f);

            if (definitive && validPowerUpUse)
            {
                SwitchPlayer();
                PlayerShootingSwitch();
                PowerUpToggle.SetActive(false);
                PowerUpBar.SetActive(false);
            }
        }

        isShooting = false;
        yield break;
    }

    bool MoveToTile(Vector3 startPos, Vector3 aimPos, float speed, GameObject missile)
    {
        Timer += speed * Time.deltaTime;
        Vector3 nextPos = Vector3.Lerp(startPos, aimPos, Timer);
        nextPos.y = altitude * Mathf.Sin(Mathf.Clamp01(Timer) * Mathf.PI);
        missile.transform.LookAt(nextPos);
        return aimPos != (missile.transform.position = Vector3.Lerp(missile.transform.position, nextPos, Timer));
    }

    public void EnablePowerUp(int listPositionI, string alignmentI, string hitableI, bool definitiveI)
    {
        Debug.Log("GameManager.EnablePowerUp started");
        isPowerUpActive = true;
        listPosition = listPositionI;
        alignment = alignmentI;
        hitable = hitableI;
        definitive = definitiveI;
        Debug.Log("GameManager.EnablePowerUp ended");
    }

    public void CallPowerUp(int x, int z, int listPosition, TileInfo info, int rival, bool definitive)
    {
        Debug.Log("'CallPowerUp' called");

        object[] content = new object[] { x, z, listPosition, rival, definitive };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(OnPowerUpUsed, content, raiseEventOptions, SendOptions.SendReliable);

        Debug.Log("'PowerUps.DisablePower' called");
        PowerUpCanvas.GetComponent<PowerUps>().DisablePower(listPosition);
        PowerUpCanvas.GetComponent<PowerUps>().ActivatePowerUp(x, z, listPosition, rival);
    }


    public void CheckWinCondition(int rival)
    {
        if (players[rival].placedShipList.Count == 0)
        {
            Debug.Log("Victory code run");
            //print("You Win!!");
            //LOGIC
            //players[activePlayer].WinPanel.SetActive(true);
            if (PhotonNetwork.IsMasterClient && players[1].placedShipList.Count == 0)
            {
                PhotonNetwork.AutomaticallySyncScene = false;
                object[] content = new object[0];
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(OnVictory, content, raiseEventOptions, SendOptions.SendReliable);
                Debug.Log("Called N1");
                if (AdvModeCheck.GetComponent<AdvanceMC>().getUsername() != "")
                {
                    SendStats(true, 2);
                }
                SceneManager.LoadScene("Win.Scene");

            }
            else if (!PhotonNetwork.IsMasterClient && players[0].placedShipList.Count == 0)
            {
                PhotonNetwork.AutomaticallySyncScene = false;
                object[] content = new object[0];
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(OnVictory, content, raiseEventOptions, SendOptions.SendReliable);
                Debug.Log("Called N2");
                if (AdvModeCheck.GetComponent<AdvanceMC>().getUsername() != "")
                {
                    SendStats(true, 2);
                }
                SceneManager.LoadScene("Win.Scene");
                
                //PlayFabManager.SendLeaderboard(winScore);
            }


        }
        else if (players[rival].placedShipList.Count == 0 && players[rival].placedShipList.Count >= 0)
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            if (AdvModeCheck.GetComponent<AdvanceMC>().getUsername() != "")
            {
                SendStats(false, 0);
            }
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Defeat.Scene");
        }

    }

    public void PlayerShootingSwitch()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (activePlayer == 0)
                {
                    players[activePlayer].shootPanel.SetActive(true);
                    if (AdvModeCheck.GetComponent<AdvanceMC>().getAMC() == true)
                    {
                        PowerUpCanvas.GetComponent<PowerUps>().SetPurchaseButtons();
                        PowerUpToggle.SetActive(true);

                    }
                    
                }
            }
            else
            {
                if (activePlayer == 1)
                {
                    players[activePlayer].shootPanel.SetActive(true);
                    if (AdvModeCheck.GetComponent<AdvanceMC>().getAMC() == true)
                    {
                        PowerUpCanvas.GetComponent<PowerUps>().SetPurchaseButtons();
                        PowerUpToggle.SetActive(true);

                    }
                    
                }
            }
        }
        else
        {
            players[activePlayer].shootPanel.SetActive(true);
        }


        gameState = GameStates.IDLE;
    }

    public void DisablePowerUp(int listPosition)
    {
        isPowerUpActive = false;
    }

    public void shipTextUpdate(int index)
    {
        int shipsLeft = players[index].placedShipList.Count;
        if (PhotonNetwork.IsMasterClient)
        { 
            if (index == 0)
            {
                ownShipsP1.text = shipsLeft.ToString();
            }
            else if (index == 1)
            {
                enemyShipsP1.text = shipsLeft.ToString();
            }
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            if (index == 0)
            {
                enemyShipsP2.text = shipsLeft.ToString();
            }
            else if (index == 1)
            {
                ownShipsP2.text = shipsLeft.ToString();
            }
        }
    }


    #region Photon Raise Events

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    //=====================================================================

    //Testing Script for PlayerDisconnects case

    void OnApplicationQuit()
    {
        this.SendQuitEvent();
    }

    void SendQuitEvent()
    {
        // send event, add your code here
        SceneManager.LoadScene("PlayerDisconnected");
        PhotonNetwork.SendAllOutgoingCommands(); // send it right now
    }

    //=====================================================================


    //RUNS WHEN A EVENT IS RECIVED FROM THE NETWORK
    public void OnEvent(EventData photonEvent)
    {
        //Sets the information recieved to local variables to be manipulated
        byte eventCode = photonEvent.Code;
        object[] data = (object[])photonEvent.CustomData;

        if (eventCode == OnTileSelected)
        {
            //Converts information recieved from event into correct data types
            int x = (int)data[0];
            int z = (int)data[1];
            int playerIndex = (int)data[2];
            TileInfo info = players[playerIndex].pgb.TileInfoRequest(x, z);     //Gets TileInfo based on the coordinates sent through
            //Runs methods to check shot on recieving client, thus creating a hit marker on their board, no missile fires however
            this.CheckShot(x, z, info);
            players[activePlayer].enemyTurn.SetActive(false);

        }
        else if (eventCode == OnShipPlacementFinished)
        {
            Debug.Log("Event 'OnShipPlacementFinished' received");
            //If sent to 2nd player by 1st player
            if (activePlayer == 0)
            {
                PlaceSystemEvent.instance.SetPlayerField(players[activePlayer].pgb, players[activePlayer].playerType.ToString(), data);
                PlaceSystemEvent.instance.PlaceShip();
                Debug.Log("P1 ships placed based on event received");
                HideAllShips();
                instance.SelectReady();

            }
            //If sent to 1st player by 2nd player
            else
            {
                PlaceSystemEvent.instance.SetPlayerField(players[activePlayer].pgb, players[activePlayer].playerType.ToString(), data);
                PlaceSystemEvent.instance.PlaceShip();
                Debug.Log("P2 ships placed based on event received");
                HideAllShips();
                instance.SelectReady();
                if (AdvModeCheck.GetComponent<AdvanceMC>().getAMC() == true)
                {
                    PowerUpToggle.SetActive(true);
                }
                hudCanvasP1.SetActive(true);

            }

        }
        else if (eventCode == OnVictory)
        {
            Debug.Log("Event 'OnVictory' received");
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            if (AdvModeCheck.GetComponent<AdvanceMC>().getUsername() != "")
            {
                SendStats(false, 2 - (players[activePlayer].placedShipList.Count));
            }
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Defeat.Scene");

        }
        else if (eventCode == OnPowerUpUsed)
        {
            int x = (int)data[0];
            int z = (int)data[1];
            int listPosition = (int)data[2];
            int rival = (int)data[3];
            bool definitive = (bool)data[4];
            PowerUpCanvas.GetComponent<PowerUps>().ActivatePowerUp(x, z, listPosition, rival);
            if (definitive)
            {
                SwitchPlayer();
                PlayerShootingSwitch();
            }
        }
    }

    #endregion


    #region PlayFab Script

    public Text messageText;

    //======================================================

    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    //======================================================

    //Function to call for statistics from database to be gathered and set global variables based on recieved information
    public void SendStats(bool _won, int _shipsDestroyed)
    {
        won = _won;
        shipsDestroyed = _shipsDestroyed;
        GetStatistics();
    }

    //Function to update the statistics on the database based on what is currently in there and the results of the match
    public void UpdateStats()
    {
        if (won)
        {
            wins++;
            Debug.Log("Wins after addition" + wins);
            var requestWins = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "Matches Won",
                        Value = wins
                    }
                }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(requestWins, OnStatsUpdate, OnError);
        }
        else
        {
            losses++;
            var requestLosses = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "Matches Lost",
                        Value = losses
                    }
                }
            };
            PlayFabClientAPI.UpdatePlayerStatistics(requestLosses, OnStatsUpdate, OnError);
        }

        shipsSunk += shipsDestroyed;
        var requestShipsSunk = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "Ships Sunk",
                        Value = shipsSunk
                    }
                }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(requestShipsSunk, OnStatsUpdate, OnError);
    }

    //Debug to ensure that the statistics were uploaded sucessfully
    void OnStatsUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Stats Sent");
    }

    //Gets statistics from the database
    void GetStatistics()
    {
        //GetPlayerStatistics requires a list of strings if you want 
        //to specify which statistics you want
        List<string> statNames = new List<string>() { "Matches Won", "Matches Lost", "Ships Sunk" };

        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest { StatisticNames = statNames },
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    //Processes statistics recived and assigns them to variables
    public void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            if (eachStat.StatisticName == "Matches Won")
            {
                wins = eachStat.Value;
            }
            else if (eachStat.StatisticName == "Matches Lost")
            {
                losses = eachStat.Value;
            }
            else if (eachStat.StatisticName == "Ships Sunk")
            {
                shipsSunk = eachStat.Value;
            }
        }
        UpdateStats();
    }

    #endregion


}