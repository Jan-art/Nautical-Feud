using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class PlaceSystem : MonoBehaviour
{
    protected bool isPlacing; //PLACE MODE ON / OFF
    protected bool canPlace; // FREE TO PLACE

    protected PhysicalGameBoard pgb;
    public LayerMask layerToCheck;  //

    [System.Serializable]public class ShipsToPlace      //
    {
        public GameObject shipGhost;   //
        public GameObject shipPrefab;  //
        public int amountToPlace = 1;  //
        public Text amountText;
        [HideInInspector]public int placedAmount = 0;   //


    }

    public List<ShipsToPlace> fleetList = new List<ShipsToPlace>(); //Set of ghost ships

    protected int currentShip ;

    protected RaycastHit hit;
    protected Vector3 hitPoint;

    void Awake()
    {
    }


    void Start()
    {
    }

    abstract public void SetPlayerField(PhysicalGameBoard _pgb, string playerType);

    abstract public void SetPlayerField(PhysicalGameBoard _pgb, string playerType, object[] _locations);

    abstract public void Update(); 

    protected void ActivateShipGhost(int index)
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

    protected void PlaceGhost()
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

    protected void RotateShipGhost()
    {
        fleetList[currentShip].shipGhost.transform.localEulerAngles += new Vector3(0, 90f, 0);
    }

    protected bool CheckIfOccupied()
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

    abstract public void PlaceShip();

    public void PlaceShipBtn(int index) //Menu Buttons
    {
        if(CheckIfAllPlaced(index))
        {
            //print("ALL AVAILABLE SHIPS HAVE BEEN PLACED ALREADY !!!");
            return;
        }
        //Activate Ghost 
        currentShip = index;
        ActivateShipGhost(currentShip);
        isPlacing = true;
    }

    protected bool CheckIfAllPlaced(int index) 
        {
            return fleetList[index].placedAmount == fleetList[index].amountToPlace;
        }

    abstract protected bool CheckIfAllPlaced(); //ALL SHIPS


    protected void UpdateAmountText()
    {
        for (int i = 0; i < fleetList.Count; i++)
        {
            fleetList[i].amountText.text = (fleetList[i].amountToPlace - fleetList[i].placedAmount).ToString();
        }
    }

    abstract public void ClearAllShips();
}





