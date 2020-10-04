using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEditorInternal;
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

    [SerializeField]
    GameObject dummy;
    [SerializeField]
    GameObject selectionRing;


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

        if (state == State.standard && selection != null)
        {
            selectionRing.SetActive(true);
            selectionRing.transform.position = selection.transform.position;
        }
        else
            selectionRing.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Escape))
            selection = null;
    }

    private void StandardUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Minion minion = hit.collider.gameObject.GetComponentInParent<Minion>();
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
            if (selection != null && Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<Terrain>())
                {
                    GameObject go = Instantiate(dummy, hit.point, Quaternion.identity);
                    selection.SetTarget(go);
                }
                else
                {
                    GameObject go = hit.collider.transform.parent.gameObject;
                    Debug.Log("setting target as " + go);
                    selection.SetTarget(go);
                }
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
