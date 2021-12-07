using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum OccupationType
{
   EMPTY,
   CARRIER,
   BATTLESHIP,
   SUBMARINE,
   CRUISER,
   CORVETTE
   
}

public class Tile 
{
  public OccupationType type;
  public ShipBehaviour placedShip;

  //CONSTRUCTOR //
  public Tile(OccupationType _type, ShipBehaviour _placedShip)
  {
      type = _type;
      placedShip = _placedShip;
  }

  public bool IsOccupied()
  {
      return type == OccupationType.CARRIER ||
          type == OccupationType.BATTLESHIP ||
          type == OccupationType.SUBMARINE ||
          type == OccupationType.CRUISER ||
          type == OccupationType.CORVETTE;
  }

  public OccupationType getOccupation()
    {
        return type;
    }

    // Assumes getOccupation() has been called to make sure it isn't empty
  public string getOccupationString()
    {
        if (type == OccupationType.CORVETTE)
        {
            return "CORVETTE";
        }
        else if (type == OccupationType.CARRIER)
        {
            return "CARRIER";
        }
        else if (type == OccupationType.BATTLESHIP)
        {
            return "BATTLESHIP";
        }
        else if (type == OccupationType.CRUISER)
        {
            return "CRUISER";
        } 
        else
        {
            return "SUBMARINE";
        }
    }
  
}

