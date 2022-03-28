using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GameManager instance;


    //Proton Event Codes used to identify what reaction needs to be taken to the information recieved
    public const byte OnTileSelected = 1;
    public const byte OnShipPlacementFinished = 2;
    public const byte OnVictory = 3;


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

        // [Space]
        // public GameObject WinPanel;
        // public GameObject LossPanel;



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

    //MISSILE
    public GameObject missilePrefab;
    float altitude = 3f;
    float Timer;

    //ADD SPEED float

    bool isShooting;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {

        //HIDE PANELS
        HideAllPanels();

        //

        //players[0].WinPanel.SetActive(false);
        //players[1].WinPanel.SetActive(false);
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
                zTile ++;
            }
            else
            {
                xTile ++;
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
                    if(PhotonNetwork.IsConnected)
                    {
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            if(activePlayer == players[0].rival)
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

            //SWITCH Player 
            SwitchPlayer();
            
            //PROTON EVENT TO NOTIFIY OTHER PLAYER, ONLY USED BY PLAYER 1 (MASTER CLIENT)
            if (PhotonNetwork.IsMasterClient)
            {
                object[] content = new object[28];
                int index = 0;
                string rotated = "";
                string occupationType;
                bool flag;

                //Loops through all tiles and if they are occupied adds the information to a object array that will be sent to the other player
                for (int i = 0; i < 10; i++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        if (players[0].myGrid[i, k].IsOccupied())
                        {
                            //If a tile is occupied adds what occupies it, tile coordinates and its rotation to the locations list 
                            flag = false;
                            occupationType = players[0].myGrid[i, k].getOccupationString();
                            content[index] = occupationType;
                            index++;
                            content[index] = i;
                            index++;
                            content[index] = k;
                            index++;
                            /*If occupied by a corvette has the systemregister it as rotated down otherwise looks at surronding tiles to figure out
                              the ships rotation */
                            if (!occupationType.Equals("CORVETTE")){
                                if (i+1 < 10)
                                {
                                    if (occupationType.Equals(players[0].myGrid[i + 1, k].getOccupationString()))
                                    {
                                        rotated = "right";
                                        flag = true;
                                    }
                                }
                                if (i-1 > -1)
                                {
                                    if (occupationType.Equals(players[0].myGrid[i - 1, k].getOccupationString()))
                                    {
                                        rotated = "left";
                                        flag = true;
                                    }
                                }
                                if (k+1 < 10)
                                {
                                    if (occupationType.Equals(players[0].myGrid[i, k + 1].getOccupationString()))
                                    {
                                        rotated = "up";
                                        flag = true;
                                    }
                                }
                                if (flag == false)
                                {
                                    rotated = "down";
                                }
                            }
                            else
                            {
                                rotated = "down";
                            }
                            content[index] = rotated;
                            index++;
                            Debug.Log("Location added to list. Occupied by" + content[index-4] + content[index-3]+ content[index-2] + content[index-1]);
                        } 
                    }
                }
                //Sets the recivers (other player) and information (object array formed above) and then sends them using RaiseEvent
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(OnShipPlacementFinished, content, raiseEventOptions, SendOptions.SendReliable);
                Debug.Log("Event 'OnShipPlacementFinished' raised");
                StartCoroutine(MoveCamera(WarCamPos));
                placingCanvas.SetActive(false);

            }
            else
            {

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

            //SWITCH Player
            SwitchPlayer();

            //MOVE CAM
            StartCoroutine(MoveCamera(WarCamPos));

            //PROTON EVENT TO NOTIFIY OTHER PLAYER, ONLY USED BY PLAYER 2 (NOT MASTER CLIENT)
            if (!PhotonNetwork.IsMasterClient)
            {
                object[] content = new object[28]; //NEEDS TO BE CHANGED FOR ACTUAL CODE
                int index = 0;
                string rotated = "";
                string occupationType;
                bool flag;
                //Loops through all tiles and if they are occupied adds the information to a object array that will be sent to the other player
                for (int i = 0; i < 10; i++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        if (players[1].myGrid[i, k].IsOccupied())
                        {
                            //If a tile is occupied adds what occupies it, tile coordinates and its rotation to the locations list
                            flag = false;
                            occupationType = players[1].myGrid[i, k].getOccupationString();
                            content[index] = occupationType;
                            index++;
                            content[index] = i;
                            index++;
                            content[index] = k;
                            index++;
                            /*If occupied by a corvette has the systemregister it as rotated down otherwise looks at surronding tiles to figure out
                              the ships rotation */
                            if (!occupationType.Equals("CORVETTE"))
                            {
                                if (i + 1 < 10)
                                {
                                    if (occupationType.Equals(players[1].myGrid[i + 1, k].getOccupationString()))
                                    {
                                        rotated = "right";
                                        flag = true;
                                    }
                                }
                                if (i - 1 > -1)
                                {
                                    if (occupationType.Equals(players[1].myGrid[i - 1, k].getOccupationString()))
                                    {
                                        rotated = "left";
                                        flag = true;
                                    }
                                }
                                if (k + 1 < 10)
                                {
                                    if (occupationType.Equals(players[1].myGrid[i, k + 1].getOccupationString()))
                                    {
                                        rotated = "up";
                                        flag = true;
                                    }
                                }
                                if (flag == false)
                                {
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


                //UNHIDE P1 SHIPS
                //UnHideAllShips(); //Not needed anymore 

                //TURN_OFF PLACING CANVAS
                placingCanvas.SetActive(false);

                //Game Start
            }
            else
            {
                //ACTIVATE P1 KILL PANELS
                players[activePlayer].shootPanel.SetActive(true);
                //placingCanvas.SetActive(false);
            }
        }

        


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
        if (isShooting)
        {
            yield break;
        }
        isShooting = true;

        int rival = Rival();


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
        if (!(players[activePlayer]==players[rival]) && players[rival].placedShipList.Count != 0)
        {
            //Stores information about the tile hit and sends it to the other player so they can update their version
            object[] content = new object[] {x, z, rival};
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
            }

            //HIGHLIGHT TILE
            //ADD [EXPLOSION + SOUND HERE]
            info.ActivateTop(3, true);

        }
        else
        {   //HIGHLIGHT TILE
            //ADD [EXPLOSION + SOUND HERE]

            //NOT HIT
            info.ActivateTop(2, true);
        }

        //REVEAL TILE
        players[rival].revealGrid[x, z] = true;

        //====================================================================
        //CHECK WIN STATUS

        if (players[rival].placedShipList.Count == 0)
        {
            Debug.Log("Victory code run");
            //print("You Win!!");
            //LOGIC
            //players[activePlayer].WinPanel.SetActive(true);
            if (PhotonNetwork.IsMasterClient && players[1].placedShipList.Count == 0)
            {
                
                object[] content = new object[0];
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(OnVictory, content, raiseEventOptions, SendOptions.SendReliable);
                Debug.Log("Called N1");
                PhotonNetwork.AutomaticallySyncScene = false;
                SceneManager.LoadScene("Win.Scene");
                
                
            }
            else if (!PhotonNetwork.IsMasterClient && players[0].placedShipList.Count == 0)
            {
                
                object[] content = new object[0];
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(OnVictory, content, raiseEventOptions, SendOptions.SendReliable);
                Debug.Log("Called N2");
                PhotonNetwork.AutomaticallySyncScene = false;
                SceneManager.LoadScene("Win.Scene");
                
            }


            yield break;
        }
         else if(players[rival].placedShipList.Count == 0 && players[rival].placedShipList.Count >= 0)
         {
             PhotonNetwork.AutomaticallySyncScene = false;
             SceneManager.LoadScene("Defeat.Scene");
         }

        yield return new WaitForSeconds(1f);

        //======================================================================

        // Should be commented out for actual release
        //HIDE SHIPS
        //HideAllShips();
        // Should be commented out for actual release

        //SWITCH PLAYER
        SwitchPlayer();

        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (activePlayer == 0)
                {
                    players[activePlayer].shootPanel.SetActive(true);
                }
            }
            else
            {
                if (activePlayer == 1)
                {
                    players[activePlayer].shootPanel.SetActive(true);
                }
            }
        }
        else
        {
            players[activePlayer].shootPanel.SetActive(true);
        }

        //SET IDLE STATE
        gameState = GameStates.IDLE;

        isShooting = false;
    }

    bool MoveToTile(Vector3 startPos, Vector3 aimPos, float speed, GameObject missile)
    {
        Timer += speed * Time.deltaTime;
        Vector3 nextPos = Vector3.Lerp(startPos, aimPos, Timer);
        nextPos.y = altitude * Mathf.Sin(Mathf.Clamp01(Timer) * Mathf.PI);
        missile.transform.LookAt(nextPos);

        return aimPos != (missile.transform.position = Vector3.Lerp(missile.transform.position, nextPos, Timer));
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

            }
            
        }
        else if (eventCode == OnVictory)
        {
            Debug.Log("Event 'OnVictory' received");
            PhotonNetwork.AutomaticallySyncScene = false;
            SceneManager.LoadScene("Defeat.Scene");

        }
    }
    
    #endregion
    

}


