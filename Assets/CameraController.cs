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

        // Check bounds
        if (pos.x < 15) pos.x = 15;
        if (pos.x > 65) pos.x = 65;
        if (pos.z < -2) pos.z = -2;
        if (pos.z > 50) pos.z = 50;

        transform.position = pos;


        // Zoom
        Vector2 zoomReq = Input.mouseScrollDelta;
        Camera.main.fieldOfView -= zoomReq.y * 2;
        if (Camera.main.fieldOfView > 60) Camera.main.fieldOfView = 60;
        if (Camera.main.fieldOfView < 20) Camera.main.fieldOfView = 20;
    }
}
