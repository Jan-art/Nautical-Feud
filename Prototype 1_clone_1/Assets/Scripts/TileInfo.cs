using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public int xPos;
    //public xPos => XPos;
    public int zPos;
    //public zPos => ZPos;

    bool hit;

    public SpriteRenderer sprite;
    public Sprite[] tileTops;  // 0 = FRAME || 1 = CROSSHAIR || 2 = WATER || 3 = HIT
                               
    public void ActivateTop(int index, bool _hit) 
    {
        sprite.sprite = tileTops[index];

        //COLOUR TILE

        hit = _hit;

    }

    public void SetTilePos(int _xpos, int _zPos)
    {
        xPos = _xpos;
        zPos = _zPos;
    }

    /*
    public int GetTilexPos()
    {
        return xPos;
    }

    public int GetTilezPos()
    {
        return zPos;
    }
    */

    void OnMouseOver()
    {
        if(GameManager.instance.gameState == GameManager.GameStates.KILL) 
        {
            if (!hit)
            {
                ActivateTop(1,false);
            }
            if (Input.GetMouseButtonDown(0))
            {
                //IDENTIFY CORDINATE
                GameManager.instance.CheckShot(xPos, zPos, this);
            }
        }
       

    }

    void OnMouseExit()
    {
        if(!hit)
        {
            ActivateTop(0,false);
        }
    }

}
