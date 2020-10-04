using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector2 screensize;

    float keyspeed = .5f;
    float mousespeed = .1f;

    float BorderSize = .08f;

    // Start is called before the first frame update
    void Start()
    {
        screensize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * keyspeed;
        float y = Input.GetAxis("Vertical") * keyspeed;

        Vector2 mousepos = Input.mousePosition;
        if (MouseInputUIBlocker.BlockedByUI == false)
        {
            if (mousepos.x < screensize.x * BorderSize) x -= mousespeed;
            if (mousepos.x > screensize.x * (1 - BorderSize)) x += mousespeed;
            if (mousepos.y < screensize.y * BorderSize) y -= mousespeed;
            if (mousepos.y > screensize.y * (1 - BorderSize)) y += mousespeed;
        }

        Vector3 pos = transform.position;
        pos += new Vector3(x, 0, y);
        transform.position = pos;
    }
}
