using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    const int amountOfWorkplaces = 6;
    const float DistanceToWorkplaces = 4;
    int currentworkspot;

    int foodStock;

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
        if(Water.Level > transform.position.y)
        {
            dirt += Time.deltaTime;
            if (dirt > maxDirt) dirt = maxDirt;
        }
    }

    internal Vector2 GetWorkPlace()
    {
        float x = transform.position.x;
        float y = transform.position.z;
        float angle = 360 / amountOfWorkplaces * currentworkspot * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * DistanceToWorkplaces + new Vector2(x, y);
    }

    internal void FoodProduced()
    {
        foodStock++;
    }
}
