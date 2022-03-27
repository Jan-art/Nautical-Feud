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
}
