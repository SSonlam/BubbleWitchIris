using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BubbleBulletLight : MonoBehaviour
{
    private Light2D myLight;
    private bool fading;
    private float fadingSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light2D>();
        myLight.intensity = 3f;
        fading = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            if  (myLight.intensity > 0.8f) 
            {
                myLight.intensity = Mathf.Lerp(myLight.intensity, 0.2f, Time.deltaTime*fadingSpeed*5f);
            }
            else
                myLight.intensity = Mathf.Lerp(myLight.intensity, 0.2f, Time.deltaTime*fadingSpeed);
        }   
        if  (myLight.intensity < 0.2f) 
        {
            fading = false;
        }  
    }
}
