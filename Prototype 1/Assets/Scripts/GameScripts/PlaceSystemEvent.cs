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
        Debug.Log("Now running 'PlaceShip'" + locations.Length);
        //Defines position and rotation variables
        int x = 0;
        int rotatedX = 0;
        int z = 0;
        int rotatedZ = 0;
        string shipRotation;

        GameObject newShip = null;

        //Defines booleans to stop ships being placed multiple times if their length is greater than 1
        bool battleshipPlaced = false;
        bool carrierPlaced = false;
        bool submarinePlaced = false;
        bool cruiserPlaced = false;

        GameObject temp;
        Ray ray;
        Vector3 pos;
        Quaternion rot = new Quaternion(0, 0, 0, 0);

        //Loops through all recieved information to decide what to do with it
        for (int i = 0; i < (locations.Length); i += 4)
        {
            //Re-intialises variables that will change each loop
            x = (int)locations[i + 1];
            rotatedX = x;
            z = (int)locations[i + 2];
            rotatedZ = z;
            shipRotation = (string)locations[i + 3];

            //Changes rotatedX or rotatedZ depending on if the ship is rotated in a way that requires it to be repositioned
            if (shipRotation.Equals("right"))
            {
                if (locations[i].Equals("CARRIER") && carrierPlaced == false)
                {
                    rotatedX += 4;
                }
                else if (locations[i].Equals("BATTLESHIP") && battleshipPlaced == false)
                {
                    rotatedX += 3;
                }
                else if (locations[i].Equals("SUBMARINE") && submarinePlaced == false)
                {
                    rotatedX += 2;
                }
                else if (locations[i].Equals("CRUISER") && cruiserPlaced == false)
                {
                    rotatedX += 1;
                }

            }
            else if (shipRotation.Equals("up"))
            {
                rot = new Quaternion(0, 0, 0, 0);
                if (locations[i].Equals("CARRIER") && carrierPlaced == false)
                {
                    rotatedZ += 4;
                }
                else if (locations[i].Equals("BATTLESHIP") && battleshipPlaced == false)
                {
                    rotatedZ += 3;
                }
                else if (locations[i].Equals("SUBMARINE") && submarinePlaced == false)
                {
                    rotatedZ += 2;
                }
                else if (locations[i].Equals("CRUISER") && cruiserPlaced == false)
                {
                    rotatedZ += 1;
                }
            }

            //Sets the position the ship will be placed at using rotatedX and rotatedZ
            temp = pgb.TileRequest(rotatedX, rotatedZ);
            ray = Camera.main.ScreenPointToRay(temp.GetComponent<Transform>().position);
            pos = temp.GetComponent<Transform>().position;

            Debug.Log("Ship Type in PlaceShip(): " + locations[i]);

            //=============================================================================================================
            //Fleetlist locations will need to be changed for the full version with all ships
            //=============================================================================================================

            //if statement for deciding which to ship to instantiate. 
            if (locations[i].Equals("CORVETTE"))
            {
                newShip = Instantiate(fleetList[4].shipPrefab, pos, rot);
                GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
            }
            else if (locations[i].Equals("CARRIER") && carrierPlaced == false)
            {
                newShip = Instantiate(fleetList[0].shipPrefab, pos, rot);
                carrierPlaced = true;
                GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
            }
            else if (locations[i].Equals("BATTLESHIP") && battleshipPlaced == false)
            {
                newShip = Instantiate(fleetList[1].shipPrefab, pos, rot);
                battleshipPlaced = true;
                GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
            }
            else if (locations[i].Equals("SUBMARINE") && submarinePlaced == false)
            {
                newShip = Instantiate(fleetList[2].shipPrefab, pos, rot);
                submarinePlaced = true;
                GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
            }
            else if (locations[i].Equals("CRUISER") && cruiserPlaced == false)
            {
                newShip = Instantiate(fleetList[3].shipPrefab, pos, rot);
                cruiserPlaced = true;
                GameManager.instance.UpdateGrid(newShip.GetComponent<Transform>(), newShip.GetComponent<ShipBehaviour>(), newShip, x, z, shipRotation);
            }

            //if statement to rotate the placed ships so they display correctly
            if (shipRotation.Equals("left"))
            {
                Debug.Log("shipRotation left in if statement");
                newShip.transform.localEulerAngles = new Vector3(0, 270f, 0);
            }
            else if (shipRotation.Equals("right"))
            {
                Debug.Log("shipRotation right in if statement");
                newShip.transform.localEulerAngles = new Vector3(0, 90f, 0);
            }
            else if (shipRotation.Equals("down"))
            {
                Debug.Log("shipRotation down in if statement");
                newShip.transform.localEulerAngles = new Vector3(0, 180f, 0);
            }
            else if (shipRotation.Equals("up"))
            {
                Debug.Log("shipRotation up in if statement");
                newShip.transform.localEulerAngles = new Vector3(0, 0f, 0);
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
