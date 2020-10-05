using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Master : MonoBehaviour
{

    int progress;
    const int progressNeeded = 200;

    [SerializeField]
    GameObject MinionPrefab;
    const float minionCooldown = 5;
    float timeSinceLastMinion= 10000;
    [SerializeField]
    Button minionButton;
    [SerializeField]
    Slider minionSlider;

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
        timeSinceLastMinion += Time.deltaTime;
        minionSlider.value = timeSinceLastMinion / minionCooldown;
        minionButton.interactable = Water.Level < transform.position.y;
    }

    public void MakeProgress()
    {
        progress++;
        Debug.Log("Progress in digging out Master is now " + progress);
        if (progress >= progressNeeded)
        {
            Debug.Log("You Won!!!"); // Make victory screen here
            UnityEngine.SceneManagement.SceneManager.LoadScene("end");
        }
    }

    public string GetProgressUpdate() => "" + progress + "/" + progressNeeded;
    public string GetDirtUpdate() => "" + dirt.ToString("0.#") + "/" + maxDirt;


    public void SpawnMinion()
    {
        if (timeSinceLastMinion < minionCooldown)
            return;
        GameObject go = Instantiate(MinionPrefab);
        go.transform.position = transform.position + new Vector3(-1, 0, 1);
        timeSinceLastMinion = 0;
    }
}
