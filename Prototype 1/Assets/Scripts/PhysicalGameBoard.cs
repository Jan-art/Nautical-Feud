using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalGameBoard : MonoBehaviour
{
    //Creates all tiles

    public bool fill; 

    //Connect to Tile prefab

    public GameObject tilePrefab; 


    //Initializing List storing all our tile prefabs
    List<GameObject> tileList = new List<GameObject>();

    //Create board
    void OnDrawGizmos() 
    {
        //PROTECT BOARD
        if(tilePrefab != null && fill)
        {
            //DELETE EXISTING TILES
            for (int i = 0; i < tileList.Count; i++) 
            {
                DestroyImmediate(tileList[i]);
            }
            tileList.Clear();

            //TILE CREATION
            for(int x = 0; x < 10; x++)
            {
                for (int k = 0; k < 10; k++)
                {

                    Vector3 pos = new Vector3(transform.position.x + x, 0, transform.position.z + k);
                    GameObject t = Instantiate(tilePrefab, pos, Quaternion.identity, transform);

                    t.GetComponent<TileInfo>().SetTilePos(x, k);
                    tileList.Add(t);
                }
            }
        }
    }

}

