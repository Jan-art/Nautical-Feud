using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceSystemEvent : PlaceSystem
{
    public static PlaceSystemEvent instance;
    public object[] locations;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateAmountText();

        ActivateShipGhost(-1);  //-1?
    }

    override public void ClearAllShips()
    {
        GameManager.instance.RemoveAllShipsFromList();
        foreach (var ship in fleetList)
        {
            ship.placedAmount = 0;
        }
        UpdateAmountText();

    }

    //USED TO PLACE SHIPS BASED ON INFORMATION RECIEVED FROM THE OTHER PLAYER
    override public void PlaceShip()
    {
        //Defines variables that will be used to place ships
        Debug.Log("Now running 'PlaceShip'"+locations.Length);
        int x = 0;
        int z = 0;
        string shipRotation;
        GameObject newShip = null;
        bool battleshipPlaced = false;
        bool carrierPlaced = false;
        bool submarinePlaced = false;
        bool cruiserPlaced = false;
        GameObject temp;
        Ray ray;
        Vector3 pos;
        Quaternion rot;
        rot = new Quaternion(0, 180, 0, 0);

        for (int i = 0; i < (locations.Length); i += 4)
        {
            //Re-intialises variables that will change each loop
            x = (int)locations[i + 1];
            z = (int)locations[i + 2];
            shipRotation = (string)locations[i + 3];
            temp = pgb.TileRequest(x, z);
            ray = Camera.main.ScreenPointToRay(temp.GetComponent<Transform>().position);
            pos = temp.GetComponent<Transform>().position;
            //May not be necessary to use ray
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, temp.layer))
            {
                hitPoint = hit.point;
            }
            Debug.Log("Ship Type in PlaceShip(): " + locations[i]);
            //If statement for whether or not it is a corvette being placed. Longer ships need code for deciding their rotation and to
            //prevent multiple copies being placed
            if (locations[i].Equals("CORVETTE"))
            {
                newShip = Instantiate(fleetList[4].shipPrefab, pos, rot);
                GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
            }
            else
            {
                //Each one instantiates a ship, flips a flag to prevent a second being placed and calls updateGrid to register the ship with it's tiles
                if (locations[i].Equals("CARRIER") && carrierPlaced == false){
                    newShip = Instantiate(fleetList[0].shipPrefab, pos, rot);
                    carrierPlaced = true;
                    GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
                }
                else if (locations[i].Equals("BATTLESHIP") && battleshipPlaced == false)
                {
                    newShip = Instantiate(fleetList[0].shipPrefab, pos, rot);
                    battleshipPlaced = true;
                    GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
                }
                else if (locations[i].Equals("SUBMARINE") && submarinePlaced == false)
                {
                    newShip = Instantiate(fleetList[1].shipPrefab, pos, rot);
                    submarinePlaced = true;
                    GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
                }
                else if (locations[i].Equals("CRUISER") && cruiserPlaced == false)
                {
                    newShip = Instantiate(fleetList[2].shipPrefab, pos, rot);
                    cruiserPlaced = true;
                    GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
                }
                if (shipRotation.Equals("right") || shipRotation.Equals("left"))
                {
                    newShip.transform.localEulerAngles = new Vector3(0, 270f, 0);
                }
            }
            
            CheckIfAllPlaced();
        }
    }
    
    override public void SetPlayerField(PhysicalGameBoard _pgb, string playerType, object[] _locations)
    {
        pgb = _pgb;
        locations = _locations;

        ClearAllShips();
    }

    override protected bool CheckIfAllPlaced() { return true; } //ALL SHIPS

    //NECESSARY TO PREVENT ERRORS WITH NOT HAVING IMPLEMENTED ALL METHODS
    override public void SetPlayerField(PhysicalGameBoard _pgb, string playerType) { }
    override public void Update() { }
}
