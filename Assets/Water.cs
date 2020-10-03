using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{

    const float minLevel = 5.0f;
    const float maxLevel = 15.0f;
    const float Period = 60.0f;
    float startTime;
    Vector3 center;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time + Period / 4;
        center = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.time - startTime;
        float D = maxLevel - minLevel;
        float level = Mathf.Sin(elapsedTime * Mathf.PI * 2 / Period) * .5f * D + minLevel * 2;
        transform.position = new Vector3(center.x, level, center.z);
    }
}
