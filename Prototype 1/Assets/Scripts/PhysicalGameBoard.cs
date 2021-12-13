using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhysicalGameBoard : MonoBehaviourPunCallbacks
{
    //Creates all tiles
    public bool create; 

    //Connect to Tile prefab
    public GameObject tilePrefab; 

    //Initializing List storing all our tile prefabs
    List<GameObject> tileList = new List<GameObject>();

    public List<TileInfo> tileInfoList = new List<TileInfo>();

    void Start()
    {
        tileList.Clear();
        tileInfoList.Clear();
        //FILL GameObj
        foreach (Transform t in transform)
        {
            if(t != transform)
            {
                tileList.Add(t.gameObject);
            }
        }
        //Fill Tile Info
        foreach (GameObject g in tileList)
        {
            tileInfoList.Add(g.GetComponent<TileInfo>());
        }
    }

    public bool RequestTile(TileInfo info)
    {
        return tileInfoList.Contains(info);
    }

    
    public TileInfo TileInfoRequest(int x, int z)
    {
        TileInfo info;
        /*if (x == 0)
        {
            info = tileInfoList[x + z];
        }
        else
        {
        */
        info = tileInfoList[(x * 10) + z];
        //}
        return info;
    }

    public GameObject TileRequest(int x, int z)
    {
        GameObject temp;
        temp = tileList[(x * 10) + z];
        return temp;
    }

    //Create board
    void OnDrawGizmos() 
    {
        //PROTECT BOARD
        if(tilePrefab != null && create)
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

