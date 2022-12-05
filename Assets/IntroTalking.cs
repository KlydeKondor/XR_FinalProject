using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroTalking : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ButtonClicked()
    {
        source.PlayOneShot(clip);
    }
    
}
