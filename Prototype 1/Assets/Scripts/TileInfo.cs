using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public int xPos;
    public int zPos;

    bool hit;

    public SpriteRenderer sprite;
    public Sprite[] tileTops;  // 0 = FRAME || 1 = CROSSHAIR || 2 = WATER || 3 = HIT
                               
    public void ActivateTop(int index) {
        sprite.sprite = tileTops[index];

    }

    public void SetTilePos(int _xpos, int _zPos)
    {
        xPos = _xpos;
        zPos = _zPos;
    }

    void OnMouseOver()
    {
        ActivateTop(1);

    }

    void OnMouseExit()
    {
        ActivateTop(0);

    }

}
