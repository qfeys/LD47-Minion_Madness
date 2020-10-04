using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSite : MonoBehaviour
{

    [SerializeField]
    GameObject prefabFinalBuilding;

    [SerializeField]
    int workToComplete;

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
        
    }

    internal void MakeProgress()
    {
        workToComplete--;
        if (workToComplete <= 0)
        {
            GameObject go = Instantiate(prefabFinalBuilding, gameObject.transform.position, gameObject.transform.rotation);
            BuildingTracker.instance.NewBuilding(go);
            Destroy(gameObject);
        }
    }
}
