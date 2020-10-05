using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingTracker : MonoBehaviour
{
    public static BuildingTracker instance;

    List<ToolShop> toolshops = new List<ToolShop>();
    List<Farm> farms = new List<Farm>();

    private void Awake()
    {
        if (instance != null) throw new System.Exception("multiple instances");
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        toolshops.AddRange(GameObject.FindObjectsOfType<ToolShop>());
        farms.AddRange(GameObject.FindObjectsOfType<Farm>());
    }

    internal void NewBuilding(GameObject go)
    {
        if (go.GetComponent<ToolShop>() != null) toolshops.Add(go.GetComponent<ToolShop>());
        else if (go.GetComponent<Farm>() != null) farms.Add(go.GetComponent<Farm>());
        else throw new Exception("Invalid building");
    }

    public GameObject FindClosestToolshop(Vector3 pos)
    {
        GameObject closest = null;
        float closestDist = float.MaxValue;
        foreach (var ts in toolshops)
        {
            float dist = Vector3.Distance(pos, ts.transform.position);
            if (dist < closestDist)
            {
                closest = ts.gameObject;
                closestDist = dist;
            }
        }
        return closest;
    }

    public GameObject FindClosestFarm(Vector3 pos)
    {
        GameObject closest = null;
        float closestDist = float.MaxValue;
        foreach (var farm in farms)
        {
            if (farm.HasFood == false) continue;
            float dist = Vector3.Distance(pos, farm.transform.position);
            if (dist < closestDist)
            {
                closest = farm.gameObject;
                closestDist = dist;
            }
        }
        return closest;
    }
}
