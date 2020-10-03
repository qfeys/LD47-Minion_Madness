using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class Minion : MonoBehaviour
{
    public Terrain terrain;

    public float speed = 4;

    Vector2 velocity = Vector2.zero;

    GameObject target;

    float hunger = 1;
    float tool = 0;
    bool hasTool = false;

    bool hasIronBar = false;

    float WorkSpeed { get => .5f * (hunger == 0 ? .5f : 1) * (tool == 0 ? 1 : 4); }
    float currentJobProgress;

    enum State { idle, walking, dig_master, dig_iron, farm, build, eat, make_tool}
    State state;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        target = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.idle:
                IdleState();
                break;
            case State.walking:
                WalkingState();
                break;
            case State.dig_master:
                DigMasterState();
                break;
            case State.dig_iron:
                DigIronState();
                break;
            case State.farm:
                FarmState();
                break;
            case State.build:
                BuildState();
                break;
            case State.eat:
                EatState();
                break;
            case State.make_tool:
                MakeToolState();
                break;
            default:
                break;
        }

        // Set height
        var pos = transform.position;
        pos.y = terrain.SampleHeight(pos) + 1;
        transform.position = pos;
    }

    internal void SetTarget(GameObject go)
    {
        if (go.GetComponent<Terrain>() != null) return;
        if (go.GetComponent<Minion>() != null) return;
        target = go;
        state = State.walking;
    }

    private void IdleState()
    {
        // play idle animation
    }

    private void WalkingState()
    {
        if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
        {
            Vector2 dir = (ToFlat(target.transform.position) - ToFlat(transform.position)).normalized;
            transform.position += ToFull(dir * speed * Time.deltaTime);
        }
        else
        {
            if (target.GetComponent<Master>() != null) state = State.dig_master;
            else if (target.GetComponent<IronPile>() != null) state = State.dig_iron;
            else if (target.GetComponent<Farm>() != null) state = State.farm;
            else if (target.GetComponent<BuildSite>() != null) state = State.build;
            else if (target.GetComponent<Food>() != null) state = State.eat;
            else if (target.GetComponent<ToolShop>() != null) state = State.make_tool;
            else throw new Exception("invalid target: " + target);
        }
    }

    private void DigMasterState()
    {
        Assert.IsTrue(target.GetComponent<Master>() != null, "This is the assert of the master");
        if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
        {
            // Lost contact
            state = State.walking;
            return;
        }
        currentJobProgress += WorkSpeed * Time.deltaTime;
        if(currentJobProgress >= 1)
        {
            currentJobProgress--;
            target.GetComponent<Master>().MakeProgress();
        }
    }


    GameObject activeToolshop;
    private void DigIronState()
    {
        Assert.IsTrue(target.GetComponent<IronPile>() != null, "This is the assert of the iron pile");
        if (hasIronBar)
        {
            if (Vector2.Distance(ToFlat(transform.position), ToFlat(activeToolshop.transform.position)) > 2)
            {
                Vector2 dir = (ToFlat(activeToolshop.transform.position) - ToFlat(transform.position)).normalized;
                transform.position += ToFull(dir * speed * Time.deltaTime);
            }
            else
            {
                activeToolshop.GetComponent<ToolShop>().AddIron();
                activeToolshop = null;
                hasIronBar = false;
            }
        }
        else
        {
            if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
            {
                // Lost contact
                state = State.walking;
                return;
            }
            currentJobProgress += WorkSpeed * Time.deltaTime;
            if (currentJobProgress >= TimeToMineIron)
            {
                currentJobProgress = 0;
                hasIronBar = true;
                GameObject toolshop;
                if (FindNearestToolshop(out toolshop))
                {
                    activeToolshop = toolshop;
                }
                else
                    state = State.idle;
            }
        }
    }

    private void FarmState()
    {
        throw new NotImplementedException();
    }

    private void BuildState()
    {
        throw new NotImplementedException();
    }

    private void EatState()
    {
        throw new NotImplementedException();
    }

    private void MakeToolState()
    {
        throw new NotImplementedException();
    }

    private bool FindNearestToolshop(out GameObject toolshop)
    {
        toolshop = BuildingTracker.instance.FindClosestToolshop(transform.position);
        if (toolshop == null) return false;
        return true;
    }

    Vector2 ToFlat(Vector3 vector3) => new Vector2(vector3.x, vector3.z);
    Vector3 ToFull(Vector2 vector2) => new Vector3(vector2.x, 0, vector2.y);

    const float TimeToMineIron = 6f;
}
