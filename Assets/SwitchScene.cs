using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{


    public void Switch(string name)
    {
        SceneManager.LoadScene(name);
    }
}
