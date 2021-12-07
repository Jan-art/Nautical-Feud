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

    }

    override public void PlaceShip()
    {
        Debug.Log("Now running 'PlaceShip'"+locations.Length);
        for (int i = 0; i < (locations.Length)/3; i += 3)
        {
            Debug.Log("Looping through locations in 'PlaceShip'");
            if (locations[i].Equals("CORVETTE"))
            {
                Debug.Log("Placing Corvette now");
                //TileInfo info = pgb.TileInfoRequest((int)locations[i+1],(int)locations[i+2];
                GameObject temp = pgb.TileRequest((int)locations[i + 1], (int)locations[i + 2]);
                Debug.Log(pgb.TileRequest((int)locations[i + 1], (int)locations[i + 2]).GetComponent<Transform>().position);
                Ray ray = Camera.main.ScreenPointToRay(temp.GetComponent<Transform>().position);
                Debug.Log(ray);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, temp.layer))
                {
                    hitPoint = hit.point;
                    Debug.Log("if statement in 'PlaceShip' has run" + hitPoint);
                }
                //Maybe functional calculation for the position of the ship
                Vector3 pos = temp.GetComponent<Transform>().position;
                //Vector3 pos = new Vector3(Mathf.Round(hitPoint.x), 0, Mathf.Round(hitPoint.z));
                Quaternion rot = fleetList[currentShip].shipGhost.transform.rotation;
                GameObject newShip = Instantiate(fleetList[4].shipPrefab, pos, rot);
            }
            else
            {
                Debug.Log("Else run in 'PlaceShip' function");
                // Code for other ship types as the rotation of them needs to be considered as it can be different
            }

            CheckIfAllPlaced();
        }
        /* KEEPING THIS HERE FOR NOW TO HELP WITH IMPLEMENTING NEW VERSION
        Vector3 pos = new Vector3(Mathf.Round(hitPoint.x), 0, Mathf.Round(hitPoint.z));
        Quaternion rot = fleetList[currentShip].shipGhost.transform.rotation;
        GameObject newShip = Instantiate(fleetList[currentShip].shipPrefab, pos, rot);

        //UPDATE GRID MENU

        GameManager.instance.UpdateGrid(fleetList[currentShip].shipGhost.transform, newShip.GetComponent<ShipBehaviour>(), newShip);

        fleetList[currentShip].placedAmount++;

        //DEACTIVATE ISPLACING()
        isPlacing = false;

        //DEACTIVATE ALL GHOST MODELS
        ActivateShipGhost(-1);

        //CHECK IF ALL SHIPS ARE PLACED
        CheckIfAllPlaced();

        //UPDATE TEXT COUNT
        UpdateAmountText();
        */


    }
    
    override public void SetPlayerField(PhysicalGameBoard _pgb, string playerType, object[] _locations)
    {
        pgb = _pgb;
        locations = _locations;

        ClearAllShips();
    }

    override public void SetPlayerField(PhysicalGameBoard _pgb, string playerType) { }

    override protected bool CheckIfAllPlaced() { return true; } //ALL SHIPS

    override public void Update() { }
}
