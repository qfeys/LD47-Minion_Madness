using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public static float Level { get => level; }

    const float minLevel = 1.5f;
    const float maxLevel = 8.5f;
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
        // On desmos:
        // \left(\frac{\left(\sin\left(o\right)+1\right)^{2}}{2}-.7+.4\cdot\sin\left(o\cdot2\right)+.3\cdot\sin\left(o\cdot3\right)+.2\cdot\sin\left(o\cdot4\right)\right)\cdot.35\cdot D+\frac{\left(l+L\right)}{2}
        level = (
            Mathf.Pow(Mathf.Sin(omeg) + 1, 2) / 2 - .7f +
            .4f * Mathf.Sin(omeg * 2) +
            .3f * Mathf.Sin(omeg * 3) +
            .2f * Mathf.Sin(omeg * 4)
            ) * .35f * D + (minLevel + maxLevel) / 2;
        transform.position = new Vector3(center.x, level, center.z);
    }
}
