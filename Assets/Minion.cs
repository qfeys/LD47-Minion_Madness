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
    GameObject interruptedTarget;

    int underwaterStatus = 0; // 0, 1 or 2
    float underwaterModifyer { get => (underwaterStatus == 0 ? 1 : underwaterStatus == 1 ? 0.5f : 0); }
    public int UnderwaterStatus { get => underwaterStatus; }

    float hunger = 1;
    float tool = 0;
    bool hasTool = false;

    bool hasIronBar = false;
    public bool HasIronBar { get => hasIronBar; }

    internal string GetHungerText() => "Hunger: " + (hunger > 0 ? (hunger * 100).ToString("00") + "%" : "Is hungry");

    internal string GetToolText() => hasTool ? "Tool: " + (tool * 100).ToString("00") + "%" : "No tool";

    float WorkSpeed { get => .5f * (hunger <= 0 ? .5f : 1) * (tool <= 0 ? 1 : 4) * underwaterModifyer; }
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
        // hunger
        hunger -= Time.deltaTime / HungerRate;
        if (hunger <= 0) FindFood();

        // Is underwater
        Vector3 pos = transform.position;
        float groundheight = terrain.SampleHeight(pos);
        if (groundheight < Water.Level)
            if (groundheight + 1 < Water.Level)
                underwaterStatus = 2;
            else
                underwaterStatus = 1;
        else
            underwaterStatus = 0;

        // Drift
        if (underwaterStatus == 2)
        {
            pos.z -= Time.deltaTime;
            transform.position = pos;
        }
        if (pos.z < 1)
            DestroyFully(gameObject);

        // Set height
        pos.y = underwaterStatus == 2 ? Water.Level - 1 : groundheight;
        transform.position = pos;
    }

    internal void SetTarget(GameObject go)
    {
        if (go.GetComponent<Terrain>() != null) return;
        if (go.GetComponent<Minion>() != null) return;
        if (go.GetComponent<Water>() != null) return;
        target = go;
        state = State.walking;
    }

    private void IdleState()
    {
        // idle walking?
        // play idle animation
    }

    private void WalkingState()
    {
        if(target == null)
        {
            state = State.idle;
            return;
        }
        if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
        {
            Vector2 dir = (ToFlat(target.transform.position) - ToFlat(transform.position)).normalized;
            MoveToDir(dir);
        }
        else
        {
            if (target.GetComponent<Master>() != null) state = State.dig_master;
            else if (target.GetComponent<IronPile>() != null) state = State.dig_iron;
            else if (target.GetComponent<Farm>() != null) state = State.farm;
            else if (target.GetComponent<BuildSite>() != null) state = State.build;
            else if (target.GetComponent<Food>() != null) state = State.eat;
            else if (target.GetComponent<ToolShop>() != null) state = State.make_tool;
            else if (target.GetComponent<Dummy>() != null) state = State.idle;
            else throw new Exception("invalid target: " + target);
        }
    }

    private void DigMasterState()
    {
        Master master = target.GetComponent<Master>();
        Assert.IsTrue(master != null, "This is the assert of the master");
        if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
        {
            // Lost contact
            state = State.walking;
            return;
        }
        if (master.HasDirt)
        {
            master.RemoveDirt(WorkSpeed * Time.deltaTime);
            return;
        }
        currentJobProgress += WorkSpeed * Time.deltaTime;
        if(currentJobProgress >= 1)
        {
            currentJobProgress--;
            master.MakeProgress();
        }
    }


    GameObject activeToolshop;
    private void DigIronState()
    {
        IronPile ironPile = target.GetComponent<IronPile>();
        Assert.IsTrue(ironPile != null, "This is the assert of the iron pile");
        if (hasIronBar) // in this case, bring the bar to a toolshop
        {
            if (Vector2.Distance(ToFlat(transform.position), ToFlat(activeToolshop.transform.position)) > 2)
            {
                Vector2 dir = (ToFlat(activeToolshop.transform.position) - ToFlat(transform.position)).normalized;
                MoveToDir(dir);
            }
            else
            {
                activeToolshop.GetComponent<ToolShop>().AddIron();
                activeToolshop = null;
                hasIronBar = false;
            }
        }
        else    // case of just mining
        {
            if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
            {
                // Lost contact
                state = State.walking;
                return;
            }
            if (ironPile.HasDirt)
            {
                ironPile.RemoveDirt(WorkSpeed * Time.deltaTime);
                return;
            }
            currentJobProgress += WorkSpeed * Time.deltaTime;
            if (currentJobProgress >= TimeToMineIron)
            {
                currentJobProgress = 0;
                hasIronBar = true;
                ironPile.TakeIron();
                Debug.Log("Iron taken");
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


    Vector2 currentFarminPosition;
    float timeOfFarmingOrder = -10;
    private void FarmState()
    {
        Farm farm = target.GetComponent<Farm>();
        Assert.IsTrue(farm != null, "This is the assert of the farm");
        if(Time.time - timeOfFarmingOrder > 10) // More then 10 sec since last order, so ask for a new one
        {
            currentFarminPosition = farm.GetWorkPlace();
            timeOfFarmingOrder = Time.time;
        }

        if (Vector2.Distance(ToFlat(transform.position), currentFarminPosition) > .5)
        {
            Vector2 dir = (currentFarminPosition - ToFlat(transform.position)).normalized;
            MoveToDir(dir);
        }
        if (Vector2.Distance(ToFlat(transform.position), currentFarminPosition) > 2)
        {
            if (farm.HasDirt)
            {
                farm.RemoveDirt(WorkSpeed * Time.deltaTime);
                return;
            }
            currentJobProgress += WorkSpeed * Time.deltaTime;
            if (currentJobProgress >= TimeToFarm)
            {
                currentJobProgress -= TimeToFarm;
                farm.FoodProduced();
                Debug.Log("Food produced");
            }
        }
    }

    private void BuildState()
    {
        if(target == null || target.GetComponent<BuildSite>() == null)
        {
            state = State.idle;
            return;
        }

        if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
        {
            // Lost contact
            state = State.walking;
            return;
        }
        currentJobProgress += WorkSpeed * Time.deltaTime;
        if (currentJobProgress >= 1)
        {
            currentJobProgress--;
            target.GetComponent<BuildSite>().MakeProgress();
            Debug.Log("Build progresses 1");
        }
    }

    private void EatState()
    {
        if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
        {
            Vector2 dir = (ToFlat(target.transform.position) - ToFlat(transform.position)).normalized;
            MoveToDir(dir);
            currentJobProgress = 0;
        }
        else
        {
            currentJobProgress += Time.deltaTime;
            if(currentJobProgress > TimeToEat)
            {
                hunger = 1;
                target = interruptedTarget;
                interruptedTarget = null;
                state = State.walking;
                Debug.Log("Minion just ate something");
            }
        }
    }

    private void MakeToolState()
    {
        ToolShop toolShop = target.GetComponent<ToolShop>();
        Assert.IsTrue(toolShop != null, "This is the assert of the toolshop");
        if (hasTool &&  tool == 1)
        {
            state = State.idle;
            return;
        }
        if (Vector2.Distance(ToFlat(transform.position), ToFlat(target.transform.position)) > 2)
        {
            // Lost contact, go back
            state = State.walking;
            return;
        }
        if(hasIronBar == false) // no bar, so take one
        {
            if (toolShop.GrabIron())
            {
                hasIronBar = true;
            }
            else
            {
                state = State.idle;
                return;
            }
        }
        if (toolShop.HasDirt)
        {
            toolShop.RemoveDirt(WorkSpeed * Time.deltaTime);
            return;
        }
        currentJobProgress += WorkSpeed * Time.deltaTime;
        float timeRequirement = hasTool ? TimeToFixTool : TimeToMakeTool;
        if (currentJobProgress >= timeRequirement)
        {
            currentJobProgress = 0;
            hasIronBar = false;
            hasTool = true;
            tool = 1;
            Debug.Log("Tool made");
            if (interruptedTarget != null)
            {
                target = interruptedTarget;
                interruptedTarget = null;
                state = State.walking;
            }
            else
                state = State.idle;
        }
    }

    private bool FindNearestToolshop(out GameObject toolshop)
    {
        toolshop = BuildingTracker.instance.FindClosestToolshop(transform.position);
        if (toolshop == null) return false;
        return true;
    }

    private void FindFood()
    {
        if (state == State.eat) return;
        Assert.IsTrue(BuildingTracker.instance != null);
        GameObject farm = BuildingTracker.instance.FindClosestFarm(transform.position);
        if (farm == null) return;
        if (interruptedTarget == null)
            interruptedTarget = target;
        target = farm;
        state = State.eat;
    }

    private void MoveToDir(Vector2 dir)
    {
        transform.position += ToFull(dir * speed * Time.deltaTime * underwaterModifyer);
        transform.rotation = Quaternion.Euler(0, -Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90, 0);
    }

    private void PushToDir(Vector2 dir)
    {
        transform.position += ToFull(dir);
    }

    static void DestroyFully(GameObject go)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            DestroyFully(go.transform.GetChild(i).gameObject);
        }
        Destroy(go);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>() != null)
            return;
        var dir = transform.position - collision.transform.position;
        PushToDir(ToFlat(dir).normalized * .5f);
        Debug.Log("collision. Moving to: " + dir + " / " + ToFlat(dir).normalized);
    }

    Vector2 ToFlat(Vector3 vector3) => new Vector2(vector3.x, vector3.z);
    Vector3 ToFull(Vector2 vector2) => new Vector3(vector2.x, 0, vector2.y);

    const float TimeToMineIron = 6f;
    const float TimeToMakeTool = 10f;
    const float TimeToFixTool = 4f;
    const float TimeToFarm = 4f;
    const float TimeToEat = 2f;
    const float HungerRate = 28f; // seconds until minion becomes hungry
}
