using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Master : MonoBehaviour
{

    int progress;
    const int progressNeeded = 500;

    float dirt = 4;
    const float maxDirt = 4;
    public bool HasDirt { get => dirt > 0; }
    public void RemoveDirt(float work) { dirt -= work; }

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
        if (Water.Level > transform.position.y)
        {
            dirt += Time.deltaTime;
            if (dirt > maxDirt) dirt = maxDirt;
        }
    }

    public void MakeProgress()
    {
        progress++;
        Debug.Log("Progress in digging out Master is now " + progress);
        if (progress >= progressNeeded)
            Debug.Log("You Won!!!"); // Make victory screen here
    }

    public string GetProgressUpdate() => "" + progress + "/" + progressNeeded;
    public string GetDirtUpdate() => "" + dirt + "/" + maxDirt;
}
