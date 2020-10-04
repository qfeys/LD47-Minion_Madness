using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public static float Level { get => level; }

    const float minLevel = 4.0f;
    const float maxLevel = 8.0f;
    const float Period = 60.0f;
    float startTime;
    static float level;
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
        float omeg = elapsedTime * Mathf.PI * 2 / Period;
        level = (
            Mathf.Sin(omeg) +
            .2f * Mathf.Sin(omeg * 2) +
            .3f * Mathf.Sin(omeg * 3) +
            .1f * Mathf.Sin(omeg * 4)
            ) * .5f * D + (minLevel + maxLevel) / 2;
        transform.position = new Vector3(center.x, level, center.z);
    }
}
