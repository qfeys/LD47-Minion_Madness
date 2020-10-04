using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MouseManager : MonoBehaviour
{
    Minion selection;

    [SerializeField]
    GameObject toolshopGhost;
    [SerializeField]
    GameObject farmGhost;
    [SerializeField]
    GameObject toolshopFoudationPrefab;
    [SerializeField]
    GameObject farmFoundationPrefab;
    

    enum State { standard, building}
    State state;
    public enum Building { toolshop, farm}
    Building currentBuilding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.standard:
                StandardUpdate();
                break;
            case State.building:
                BuildingUpdate();
                break;
        }
    }

    private void StandardUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.gameObject;
                Minion minion = go.GetComponent<Minion>();
                if (minion != null)
                {
                    selection = minion;
                    Debug.Log("minion is selected.");
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.transform.parent.gameObject;
                Debug.Log("setting target as " + go);
                selection.SetTarget(go);
            }
        }
    }

    private void BuildingUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 cursorPos;
        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask(new string[] { "Terrain" })))
        {
            Assert.IsTrue(hit.collider.GetComponent<Terrain>() != null);
            cursorPos = hit.point;
        }
        else
        {
            Debug.LogWarning("Ray did not hit terrain.");
            return;
        }

        GameObject activeBuiding = null;
        switch (currentBuilding)
        {
            case Building.toolshop: activeBuiding = toolshopGhost; break;
            case Building.farm: activeBuiding = farmGhost; break;
        }
        activeBuiding.transform.position = cursorPos;

        if (Input.GetMouseButtonUp(0) && MouseInputUIBlocker.BlockedByUI == false)
        {
            GameObject relevantPrefab = null;
            switch (currentBuilding)
            {
                case Building.toolshop: relevantPrefab = toolshopFoudationPrefab; break;
                case Building.farm: relevantPrefab = farmFoundationPrefab; break;
            }
            GameObject.Instantiate(relevantPrefab, activeBuiding.transform.position, Quaternion.identity);
            activeBuiding.SetActive(false);
            state = State.standard;
        }
    }

    public void Build(string building)
    {
        Build((Building)Enum.Parse(typeof(Building), building));
    }

    public void Build(Building building)
    {
        state = State.building;
        currentBuilding = building;
        switch (currentBuilding)
        {
            case Building.toolshop: toolshopGhost.SetActive(true); break;
            case Building.farm: farmGhost.SetActive(true); break;
        }
    }
}
