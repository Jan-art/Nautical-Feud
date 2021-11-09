using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceSystem : MonoBehaviour
{
    public bool isPlacing; //PLACE MODE ON / OFF
    bool canPlace; // FREE TO PLACE

    PhysicalGameBoard pgb;
    public LayerMask layerToCheck;  //

    [System.Serializable]
    public class ShipsToPlace      //
    {
        public GameObject shipGhost;   //
        public GameObject shipPrefab;  //
        public int amountToPlace = 1; //
        [HideInInspector]public int placedAmount = 0;  //

    }

    public List<ShipsToPlace> fleetList = new List<ShipsToPlace>(); //Set of ghost ships

    int currentShip = 0 ;

    RaycastHit hit;
    Vector3 hitPoint;



    void Start()
    {
        ActivateShipGhost(-1);  //-1?
        //ActivateShipGhost(currentShip);
    }

   
    void Update()
    {
        if (isPlacing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit,Mathf.Infinity,layerToCheck))
            {
                //If tile hit is not the opponent's tile.

                //RETURN

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

    void ActivateShipGhost(int index)
    {
        if(index!= -1)
        {
            if(fleetList[index].shipGhost.activeInHierarchy)
            {
                return;
            }
        }

        //DEACTIVATE ALL GHOST SHIPS
        for (int i = 0; i < fleetList.Count; i++)
        {
            fleetList[i].shipGhost.SetActive(false);
        }

        if(index == -1)
        {
            return;
        }
        //ACTIVATE SELECTED GHOST SHIP

        fleetList[index].shipGhost.SetActive(true);
    }

    void PlaceGhost()
    {
        if (isPlacing)
        {
            canPlace = CheckIfOccupied();
            fleetList[currentShip].shipGhost.transform.position = new Vector3(Mathf.Round(hitPoint.x), 0, Mathf.Round(hitPoint.z));

        }
        else
        {
            //Deactivate all ghost models
            ActivateShipGhost(-1);
        }
    }

    void RotateShipGhost()
    {
        fleetList[currentShip].shipGhost.transform.localEulerAngles += new Vector3(0, 90f, 0);
    }

    bool CheckIfOccupied()
    {
            foreach(Transform child in fleetList[currentShip].shipGhost.transform)
            {
                GhostBehaviour ghost = child.GetComponent<GhostBehaviour>();
                if (!ghost.OverTile())
                {
                    child.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 125);
                    return false;
                }
                child.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 100);
            }

            return true;
    }
    
    void PlaceShip()
    {
        Vector3 pos = new Vector3(Mathf.Round(hitPoint.x), 0, Mathf.Round(hitPoint.z));
        Quaternion rot = fleetList[currentShip].shipGhost.transform.rotation;
        GameObject newShip = Instantiate(fleetList[currentShip].shipPrefab, pos, rot );

        //UPDATE GRID MENU
        
        //GameManager.instance.UpdateGrid(fleetList[currentShip].shipGhost.transform, newShip.GetComponent<ShipBehaviour>(), newShip);

        fleetList[currentShip].placedAmount++;

        //DEACTIVATE ISPLACING()
        isPlacing = false;
        //DEACTIVATE ALL GHOST MODELS
        ActivateShipGhost(-1);
        //CHECK IF ALL SHIPS ARE PLACED
      
    }

    public void PlaceShipBtn(int index) //Menu Buttons
    {
        if(CheckIfAllPlaced(index))
        {
            print("ALL AVAILABLE SHIPS HAVE BEEN PLACED ALREADY !!!");
            return;
        }
        //Activate Ghost 
        currentShip = index;
        ActivateShipGhost(currentShip);
        isPlacing = true;
    }

    bool CheckIfAllPlaced(int index) 
        {
            return fleetList[index].placedAmount == fleetList[index].amountToPlace;
        }
    
}





