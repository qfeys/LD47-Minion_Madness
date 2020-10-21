using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionCanvas : MonoBehaviour
{

    public Text hunger;
    public Text tool;
    public Text ironBar;
    public Text drowning;
    public Text idle;
    public Text state;

    Minion minion;

    // Start is called before the first frame update
    void Start()
    {
        minion = GetComponentInParent<Minion>();
    }

    // Update is called once per frame
    void Update()
    {
        hunger.text = minion.GetHungerText();
        tool.text = minion.GetToolText();
        ironBar.gameObject.SetActive(minion.HasIronBar);
        drowning.gameObject.SetActive(minion.UnderwaterStatus != 0);
        drowning.text = minion.UnderwaterStatus == 1 ? "Wet Feet!" : "Adrift!";
        idle.gameObject.SetActive(minion.IsIdle());
        state.text = minion.GetStateText();

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
