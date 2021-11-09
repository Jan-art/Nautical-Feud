using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
  
  
}

