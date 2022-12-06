using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCameras : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject sevCamera;
    public GameObject sev;

    private Vector3 buffer = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        // Set the first-person camera's parent as the SEV
        sevCamera.transform.parent = sev.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the user has pressed the C key
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Toggle the active camera
            mainCamera.SetActive(!mainCamera.activeSelf);
            sevCamera.SetActive(!sevCamera.activeSelf);
        }
    }
}
