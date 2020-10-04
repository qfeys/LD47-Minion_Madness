using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusCanvas : MonoBehaviour
{

    public Text progress;
    public Text progressText;
    public Text dirt;
    public Text dirtWarning;

    Master master;
    ToolShop toolShop;
    IronPile ironPile;
    BuildSite buildSite;
    Farm farm;

    enum Typ { master, toolshop, ironpile, buildsite, farm}
    Typ typ;

    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = transform.parent.gameObject;
        if (parent.GetComponent<Master>() != null) { typ = Typ.master; master = parent.GetComponent<Master>(); }
        else if (parent.GetComponent<ToolShop>() != null) { typ = Typ.toolshop; toolShop = parent.GetComponent<ToolShop>(); }
        else if (parent.GetComponent<IronPile>() != null) { typ = Typ.ironpile; ironPile = parent.GetComponent<IronPile>(); }
        else if (parent.GetComponent<BuildSite>() != null) { typ = Typ.buildsite; buildSite = parent.GetComponent<BuildSite>(); }
        else if (parent.GetComponent<Farm>() != null) { typ = Typ.farm; farm = parent.GetComponent<Farm>(); }
        else throw new System.NotImplementedException("typ not available yet for go: " + parent);
    }

    // Update is called once per frame
    void Update()
    {
        switch (typ)
        {
            case Typ.master:
                progress.text = master.GetProgressUpdate();
                progressText.text = "Dig out Master";
                dirt.text = master.GetDirtUpdate();
                dirt.gameObject.SetActive(master.HasDirt);
                dirtWarning.gameObject.SetActive(master.HasDirt);
                break;
            case Typ.toolshop:
                progress.text = toolShop.GetProgressUpdate();
                progressText.text = "Iron available";
                dirt.text = toolShop.GetDirtUpdate();
                dirt.gameObject.SetActive(toolShop.HasDirt);
                dirtWarning.gameObject.SetActive(toolShop.HasDirt);
                break;
            case Typ.ironpile:
                progress.text = ironPile.GetProgressUpdate();
                progressText.text = "Iron in the ground";
                dirt.text = ironPile.GetDirtUpdate();
                dirt.gameObject.SetActive(ironPile.HasDirt);
                dirtWarning.gameObject.SetActive(ironPile.HasDirt);
                break;
            case Typ.buildsite:
                progress.text = buildSite.GetProgressUpdate();
                progressText.text = "Before completion";
                dirt.text = buildSite.GetDirtUpdate();
                dirt.gameObject.SetActive(buildSite.HasDirt);
                dirtWarning.gameObject.SetActive(buildSite.HasDirt);
                break;
            case Typ.farm:
                progress.text = farm.GetProgressUpdate();
                progressText.text = "Food available";
                dirt.text = farm.GetDirtUpdate();
                dirt.gameObject.SetActive(farm.HasDirt);
                dirtWarning.gameObject.SetActive(farm.HasDirt);
                break;
            default:
                break;
        }
    }
}
