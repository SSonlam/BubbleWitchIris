using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CaptureLight : MonoBehaviour
{
    private Light2D myLight;
    private bool fading;
    private float fadingSpeed = 8f;
    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light2D>();
        myLight.intensity = 1.5f;
        myLight.pointLightOuterRadius = 5f;
        fading = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (fading)
        {
            myLight.intensity = Mathf.Lerp(myLight.intensity, 0.2f, Time.deltaTime * fadingSpeed);
            myLight.pointLightOuterRadius  = Mathf.Lerp(myLight.pointLightOuterRadius , 2, Time.deltaTime*fadingSpeed);
        }   
        if  (myLight.intensity < 0.3f) 
        {
            fading = false;
        }  
    }
}
