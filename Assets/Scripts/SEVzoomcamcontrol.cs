using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEVzoomcamcontrol : MonoBehaviour
{
    public static string zoomActive = "n";
    public AudioSource source;
    public AudioClip clip;
    int counter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ButtonClicked()
    {
        counter++;
        if (counter == 1)
        {
            zoomActive = "y";
            source.PlayOneShot(clip);
        }
        else if (counter % 2 == 1)
        {
            zoomActive = "y";
        }
        else
        {
            zoomActive = "n";
        }
    }
    
    
}
