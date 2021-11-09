using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBehaviour : MonoBehaviour
{
    public LayerMask layerToCheck;
    RaycastHit hit;
    TileInfo info;

    PhysicalGameBoard pgb;

    public void SetPlayfield(PhysicalGameBoard _pgb)
    {
        pgb = _pgb;
    }

    public bool OverTile()
    {
        info = GetTileInfo();

        if(info != null) 
        {
            return true;
        }
        info = null;
        return false;
    }
    
    public TileInfo GetTileInfo()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        if(Physics.Raycast(ray,out hit, 1f, layerToCheck))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            return hit.collider.GetComponent<TileInfo>();
        }
        
        return null;
    }
}

