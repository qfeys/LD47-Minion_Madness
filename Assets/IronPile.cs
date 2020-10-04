using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronPile : MonoBehaviour
{
    [SerializeField]
    int totalIron = 16;

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

    public void TakeIron()
    {
        totalIron--;
        if(totalIron<= 0)
        {
            Debug.Log("Iron pile depleted");
            Destroy(gameObject);
        }    
    }

    public string GetProgressUpdate() => "" + totalIron;
    public string GetDirtUpdate() => "" + dirt.ToString("0.#") + "/" + maxDirt;
}
