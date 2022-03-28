using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceSystemManual : PlaceSystem
{
    public static PlaceSystemManual instance;
    public Button readyBtn;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        readyBtn.interactable = false;

        UpdateAmountText();


        ActivateShipGhost(-1);  //-1?
        //ActivateShipGhost(currentShip);
    }

    override public void ClearAllShips()
    {
        GameManager.instance.RemoveAllShipsFromList();
        foreach (var ship in fleetList)
        {
            ship.placedAmount = 0;
        }
        UpdateAmountText();

        readyBtn.interactable = false;

    }

    override public void PlaceShip()
    {
        Vector3 pos = new Vector3(Mathf.Round(hitPoint.x), 0, Mathf.Round(hitPoint.z));
        Quaternion rot = fleetList[currentShip].shipGhost.transform.rotation;
        GameObject newShip = Instantiate(fleetList[currentShip].shipPrefab, pos, rot );

        //UPDATE GRID MENU
        
        GameManager.instance.UpdateGrid(fleetList[currentShip].shipGhost.transform,newShip.GetComponent<ShipBehaviour>(), newShip);

        fleetList[currentShip].placedAmount++;

        //DEACTIVATE ISPLACING()
        isPlacing = false;

        //DEACTIVATE ALL GHOST MODELS
        ActivateShipGhost(-1);

        //CHECK IF ALL SHIPS ARE PLACED
        CheckIfAllPlaced();

        //UPDATE TEXT COUNT
        UpdateAmountText();

      
    }

    override public void SetPlayerField(PhysicalGameBoard _pgb, string playerType)
    {
        pgb = _pgb;
        readyBtn.interactable = false;

        ClearAllShips();

    }

    override public void SetPlayerField(PhysicalGameBoard _pgb, string playerType, object[] _locations) { }

    override protected bool CheckIfAllPlaced() //ALL SHIPS
    {
        foreach (var ship in fleetList)  //Change to for loop. 
        {
            if (ship.placedAmount != ship.amountToPlace)
            {
                return false;
            }
        }

        readyBtn.interactable = true;
        return true;
    }

    override public void Update(){
        if(isPlacing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerToCheck)) 
            {
                //If tile hit is not the opponent's tile.
                if (!pgb.RequestTile(hit.collider.GetComponent<TileInfo>()))
                {
                    return;
                }

                hitPoint = hit.point;
            }

        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            //PLACE SHIP
            PlaceShip();

        }

        if (Input.GetMouseButtonDown(1))
        {
            //ROTATE SHIP
            RotateShipGhost();
        }

        //Place Ghost
        PlaceGhost();
        }
    }

    //================================================= PART OF AUTOMATIC PLACEMENT - START

    bool CheckIfOccupied(Transform tr)
    {
        foreach (Transform child in tr.transform)
        {
            GhostBehaviour ghost = child.GetComponent<GhostBehaviour>();
            if (!ghost.OverTile())
            {
                return false;
            }
        }
        return true;
    }

    //================================================= PART OF AUTOMATIC PLACEMENT - END

    //====================================================================
    //                       AutomaticPlacing - Start          
    //====================================================================

    public void AutoPlaceShips()
    {
        ClearAllShips(); //Clear all ships first

        bool posFound = false;


        //Loop through all Ships
        for (int i = 0; i < fleetList.Count; i++)
        {
            //Loop through ship amount
            for (int j = 0; j < fleetList[i].amountToPlace; j++)
            {
                if (fleetList[i].amountToPlace <= 0)
                {
                    print("Error - No Ships To Place");
                    return;
                }
                posFound = false;

                //While Found
                while (!posFound)
                {
                    currentShip = i;

                    int xPos = Random.Range(0, 10);
                    int zPos = Random.Range(0, 10);

                    //Create Ghost
                    GameObject tempGhost = Instantiate(fleetList[currentShip].shipGhost);
                    tempGhost.SetActive(true);
                    Debug.Log("TempGhostActivated");

                    //Set Ghost Playfield

                    //Set Ghost POS

                    tempGhost.transform.position = new Vector3(pgb.transform.position.x + xPos, 0,
                                                               pgb.transform.position.z + zPos);

                    Vector3[] rot = { Vector3.zero, new Vector3(0, 90, 0), new Vector3(0, 180, 0), new Vector3(0, 270, 0) };

                    //Check For random rotation

                    for (int r = 0; r < rot.Length; r++)
                    {
                        List<int> indexList = new List<int> { 0, 1, 2, 3 };
                        int rr = indexList[Random.Range(0, indexList.Count)]; //Randomise Index

                        tempGhost.transform.rotation = Quaternion.Euler(rot[rr]);
                        if (CheckIfOccupied(tempGhost.transform))
                        {
                            PlaceAutoShip(tempGhost);
                            posFound = true;
                        }
                        else
                        {
                            Destroy(tempGhost);
                            indexList.Remove(r);
                        }
                    }
                }
            }
            //SelectReady.interactable = true;
            CheckIfAllPlaced();
            //UPDATED TEXT AMOUNT

        }

        void PlaceAutoShip(GameObject temp)
        {
            GameObject newShip = Instantiate(fleetList[currentShip].shipPrefab, temp.transform.position, temp.transform.rotation);
            GameManager.instance.UpdateGrid(temp.transform, newShip.GetComponent<ShipBehaviour>(), newShip);
            fleetList[currentShip].placedAmount++;

            Destroy(temp);
            UpdateAmountText();
        }

    }



    //====================================================================
    //                       AutomaticPlacing - End         
    //====================================================================
}

