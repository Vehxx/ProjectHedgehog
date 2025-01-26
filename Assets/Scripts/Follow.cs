using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Follow : MonoBehaviour
{
    public Transform followTransform;
    public BoxCollider2D mapBounds;

    private float xMin, xMax, yMin, yMax;
    private float camY, camX;
    private float camOrthsize;
    private float cameraRatio;
    private float height;
    private float width;
    private Camera mainCam;
    private Vector3 smoothPos;
    public float smoothSpeed = 0.5f;

    private void Start()
    {
        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;
        mainCam = GetComponent<Camera>();
        camOrthsize = mainCam.orthographicSize;
        cameraRatio = (xMax + camOrthsize) / 2.0f;
        height = mainCam.orthographicSize;
        width = height*2;

        camY = followTransform.position.y;
        camX = followTransform.position.x;

        if (camY < yMin + height)
        {
            camY = yMin + height;
        }
        if (camY > yMax - height)
        {
            camY = yMax - height;
        }
        if (camX < xMin + width)
        {
            camX = xMin + width;
        }
        if (camX > xMax - width)
        {
            camX = xMax - width;
        }


        //camX = followTransform.position.x;
        smoothPos = Vector3.Lerp(this.transform.position, new Vector3(camX, camY, this.transform.position.z), smoothSpeed);
        this.transform.position = smoothPos;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        camY = followTransform.position.y;
        camX = followTransform.position.x;

        if (camY < yMin + height) {
            camY = yMin + height;
        }
        if (camY > yMax - height) {
            camY = yMax - height;
        }
        if (camX < xMin + width) {
            camX = xMin + width;
        }
        if (camX > xMax - width) {
            camX = xMax - width;
        }


        //camX = followTransform.position.x;
        smoothPos = Vector3.Lerp(this.transform.position, new Vector3(camX, camY, this.transform.position.z), smoothSpeed);
        this.transform.position = smoothPos;



    }
}