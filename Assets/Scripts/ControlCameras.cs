using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCameras : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject sevCamera;
    public GameObject splitCameraOne;
    public GameObject splitCameraTwo;
    public GameObject sev;

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
            if (splitCameraOne.activeSelf)
            {
                // Activate the SEV's first-person camera; turn off the split-view cameras
                sevCamera.SetActive(true);
                splitCameraOne.SetActive(false);
                splitCameraTwo.SetActive(false);
            }
            else if (sevCamera.activeSelf)
            {
                // Activate the main camera; turn off sevCamera
                mainCamera.SetActive(true);
                sevCamera.SetActive(false);
            }
            else if (mainCamera.activeSelf)
            {
                // Activate the split-view cameras; turn off the main camera
                splitCameraOne.SetActive(true);
                splitCameraTwo.SetActive(true);
                mainCamera.SetActive(false);
            }
        }
    }
}
