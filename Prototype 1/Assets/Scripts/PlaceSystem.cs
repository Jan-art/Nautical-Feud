using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceSystem : MonoBehaviour
{
    public static PlaceSystem instance;

    public bool isPlacing; //PLACE MODE ON / OFF
    bool canPlace; // FREE TO PLACE

    PhysicalGameBoard pgb;
    public LayerMask layerToCheck;  //

    [System.Serializable]
    public class ShipsToPlace      //
    {
        public GameObject shipGhost;   //
        public GameObject shipPrefab;  //
        public int amountToPlace = 1;  //
        public Text amountText;
        [HideInInspector]public int placedAmount = 0;   //


    }

    public List<ShipsToPlace> fleetList = new List<ShipsToPlace>(); //Set of ghost ships

    public Button readyBtn;

    int currentShip = 0 ;

    RaycastHit hit;
    Vector3 hitPoint;

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

    public void SetPlayerField(PhysicalGameBoard _pgb, string playerType)
    {
        pgb = _pgb;
        readyBtn.interactable = false;

        ClearAllShips();

       /* if(playerType == "/")
        {
            
        }
       */

    }
   
    void Update()
    {
        if (isPlacing)
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

    void ActivateShipGhost(int index)
    {
        if(index != -1)
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
    
    bool CheckIfAllPlaced() //ALL SHIPS
    {
        foreach (var ship in fleetList)  //Change to for loop. 
        {
            if(ship.placedAmount != ship.amountToPlace)
            {
                return false;
            }
        }  

        readyBtn.interactable = true;
        return true;
    }
    void UpdateAmountText()
    {
        for (int i = 0; i < fleetList.Count; i++)
        {
            fleetList[i].amountText.text = (fleetList[i].amountToPlace - fleetList[i].placedAmount).ToString();
        }
    }

    public void ClearAllShips()
    {
        GameManager.instance.RemoveAllShips();
        foreach (var ship in fleetList)
        {
            ship.placedAmount = 0;
        }
        UpdateAmountText();
        //DISABLE READY BUTTON
    }
}





