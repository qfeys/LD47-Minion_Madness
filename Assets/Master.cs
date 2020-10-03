using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Master : MonoBehaviour
{

    int progress;
    const int progressNeeded = 500;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeProgress()
    {
        progress++;
        Debug.Log("Progress in digging out Master is now " + progress);
        if (progress >= progressNeeded)
            Debug.Log("You Won!!!"); // Make victory screen here
    }
}
