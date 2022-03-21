using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{
 public int shipLength;
 int hitCount;
 public OccupationType type;

 void Start()
 {
     hitCount = shipLength;
 }

//Check if ship has sunk
 bool IsSunk()
 {
     return hitCount <= 0;
 }
 
 public bool IsHit()
 {
    return hitCount < shipLength && hitCount > 0;
 }

  public bool AbsorbDamage()
 {
     hitCount--;
     if(IsSunk())
     {
       //RE-ENABLE MESH RENDERER > REPORT TO GAME MANAGER
       GetComponent<MeshRenderer>().enabled = true;
       return true;
     }
        return false;
 }

}