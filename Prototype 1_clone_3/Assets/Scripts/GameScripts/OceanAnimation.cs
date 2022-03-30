using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanAnimation : MonoBehaviour
{
    public float speed = 0.5f;
    float offset;
    public bool X = false;
    public bool Y = false;

    Material material;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset = Time.time * speed % 1;
        if (X && Y)
        {
            material.mainTextureOffset = new Vector2(offset, offset);
        }
        else if (X)
        {
            material.mainTextureOffset = new Vector2(offset, 0);
        }
        else if (Y)
        {
            material.mainTextureOffset = new Vector2(0, offset);

        }
    }
}
