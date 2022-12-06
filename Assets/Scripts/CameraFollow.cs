using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (SEVzoomcamcontrol.zoomActive == "y")
        {
            GetComponent<Camera>().enabled = true;
        }
        else
        {
            GetComponent<Camera>().enabled = false;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
    
    
}
