using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Minion : MonoBehaviour
{
    public Terrain terrain;

    public float speed = 4;

    Vector2 velocity = Vector2.zero;

    Vector2 target;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        target = ToFlat(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(ToFlat(transform.position), target) > 2)
        {
            Vector2 dir = (target - ToFlat(transform.position)).normalized;
            transform.position += ToFull(dir * speed * Time.deltaTime);
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
        target = ToFlat(go.transform.position);
    }

    Vector2 ToFlat(Vector3 vector3) => new Vector2(vector3.x, vector3.z);
    Vector3 ToFull(Vector2 vector2) => new Vector3(vector2.x, 0, vector2.y);
}
