using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class UnitLight : MonoBehaviour
{
    private Light2D myLight;

    
    public float eachFadeTime;
    public float fadeWaitTime;
    // Start is called before the first frame update
    void Start()
    {
        eachFadeTime = Random.Range(2f,4f);
        fadeWaitTime = Random.Range(4f,6f);
        myLight = GetComponent<Light2D>();
        myLight.pointLightOuterRadius = Random.Range(2f,3f);
        StartCoroutine(fadeInAndOutRepeat(myLight, eachFadeTime, fadeWaitTime));
    }
        //Fade in and out forever
    IEnumerator fadeInAndOutRepeat(Light2D lightToFade, float duration, float waitTime)
    {
        WaitForSeconds waitForXSec = new WaitForSeconds(waitTime);

        while (true)
        {
            //Fade out
            yield return fadeInAndOut(lightToFade, false, duration);

            //Wait
            yield return waitForXSec;

            //Fade-in 
            yield return fadeInAndOut(lightToFade, true, duration);
        }
    }

    IEnumerator fadeInAndOut(Light2D lightToFade, bool fadeIn, float duration)
    {
        float minLuminosity = Random.Range(1.1f,1.2f); // min intensity
        float maxLuminosity = Random.Range(1.3f,1.4f); // max intensity

        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float a, b;

        if (fadeIn)
        {
            a = minLuminosity;
            b = maxLuminosity;
        }
        else
        {
            a = maxLuminosity;
            b = minLuminosity;
        }

        float currentIntensity = lightToFade.intensity;

        while (counter < duration)
        {
            counter += Time.deltaTime;

            lightToFade.intensity = Mathf.Lerp(a, b, counter / duration);

            yield return null;
        }
    }


}
