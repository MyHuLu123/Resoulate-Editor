using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixOnCamera : MonoBehaviour
{
    public bool fixX;
    public bool fixY;
    public Camera targetCamera;
    void Start()
    {
        if(targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }
    void Update()
    {
        float changeX = transform.position.x, changeY=transform.position.y;
        if (fixX)
        {
            changeX = targetCamera.transform.position.x;
        }
        if (fixY)
        {
            changeY = targetCamera.transform.position.y;
        }
        transform.position = new Vector3(changeX,changeY,transform.position.z);
    }
}
