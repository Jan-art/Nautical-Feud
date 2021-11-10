using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager instance;

  void Awake()
  {
        instance = this;
  }
  

  [System.Serializable]public class Player
  {
     public enum PlayerType //In future versions of the game, can add NPC players that are controlled by AI. 
     {
         HUMAN
     }

     public PlayerType playerType;
     public Tile[,] myGrid = new Tile[10,10];
     public bool[,] revealGrid = new bool[10,10];
     public PhysicalGameBoard pgb;
     public LayerMask layerToPlaceOn;

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
     }
     
     public List<GameObject> placedShipList = new List<GameObject>();
  }

  int activePlayer; //Track current Turn
  public Player[] players = new Player[2];

  void Start()
    {
        PlaceSystem.instance.SetPlayerField(players[activePlayer].pgb, players[activePlayer].playerType.ToString());
    }

  void AddShipToList(GameObject placedShip)
  {
      players[activePlayer].placedShipList.Add(placedShip);
  }

  public void UpdateGrid(Transform shipTransform, ShipBehaviour ship, GameObject placedShip)
  {
      foreach(Transform child in shipTransform)
      {
          TileInfo tInfo = child.GetComponent<GhostBehaviour>().GetTileInfo();
          players[activePlayer].myGrid[tInfo.xPos, tInfo.zPos] = new Tile(ship.type, ship);
      }

      AddShipToList(placedShip);
      DebugGrid();
  }

    public bool CheckIfOccupied(int xPos, int zPos)
    {
        return players[activePlayer].myGrid[xPos, zPos].IsOccupied();
    }

  public void DebugGrid()
  {
      string s = "";
      //Separator
      int sep = 0; 
      for (int x = 0; x < 10; x++)
      {
          s += "|";
          for (int y = 0; y < 10; y++)
          {
              string t = "0"; //Occupation Type
              if(players[activePlayer].myGrid[x, y].type == OccupationType.CARRIER)
              {
                  t = "C";
              }
               if(players[activePlayer].myGrid[x, y].type == OccupationType.BATTLESHIP)
              {
                  t = "B";
              }
               if(players[activePlayer].myGrid[x, y].type == OccupationType.SUBMARINE)
              {
                  t = "S";
              }
               if(players[activePlayer].myGrid[x, y].type == OccupationType.CRUISER)
              {
                  t = "U";
              }
               if(players[activePlayer].myGrid[x, y].type == OccupationType.CORVETTE)
              {
                  t = "R";
              }

              s += t;
              sep = y % 10;
              if(sep == 9)
              {
                  s += "|";
              }

          }

          s += "\n";
          
      }
      print(s);
  }
  
    public void RemoveAllShips()
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

}


