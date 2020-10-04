using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolShop : MonoBehaviour
{

    int ironSupply;

    float dirt = 0;
    const float maxDirt = 4;
    public bool HasDirt { get => dirt > 0; }
    public void RemoveDirt(float work) { dirt -= work; }

    // Start is called before the first frame update
    void Start()
    {
        // Set height
        var pos = transform.position;
        pos.y = GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(pos);
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Water.Level > transform.position.y)
        {
            dirt += Time.deltaTime;
            if (dirt > maxDirt) dirt = maxDirt;
        }
    }

    internal void AddIron()
    {
        ironSupply++;
    }

    internal bool GrabIron()
    {
        if (ironSupply <= 0) return false;
        ironSupply--;
        return true;
    }
}
