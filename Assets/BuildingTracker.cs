using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingTracker : MonoBehaviour
{

    List<ToolShop> toolshops = new List<ToolShop>();
    List<Farm> farms = new List<Farm>();

    // Start is called before the first frame update
    void Start()
    {
        toolshops.AddRange(GameObject.FindObjectsOfType<ToolShop>());
        farms.AddRange(GameObject.FindObjectsOfType<Farm>());
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
