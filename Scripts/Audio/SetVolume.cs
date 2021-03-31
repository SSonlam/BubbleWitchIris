using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volumeSlider;
    public InputField volumeInput;

     void Start()
    {
        float volumeLevel;
        mixer.GetFloat("MusicVol", out volumeLevel);
        SetLevel(Mathf.Pow(10f, volumeLevel / 20));
        Debug.Log("The Volume Level Is" + volumeLevel);
    }

    public void SetLevel(float sliderValue)
	{
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        volumeInput.text = ((int)(sliderValue * 100)).ToString();
	}

    public void setVolumeInput()
    { 
        if(volumeInput.text != "")
        {
            int vInput = int.Parse(volumeInput.text);
            Debug.Log("The value of vInput is " + vInput);

            if (vInput > 100)
            {
                mixer.SetFloat("MusicVol", Mathf.Log10(100 / 100) * 20);
                volumeSlider.value = 1;
                volumeInput.text = 100.ToString();
            }
            else if (vInput <= 0)
            {
                mixer.SetFloat("MusicVol", Mathf.Log10(0.01f) * 20);
                volumeSlider.value = 0;
                volumeInput.text = 0.ToString();
            }
            else
            {
                mixer.SetFloat("MusicVol", Mathf.Log10(vInput / 100) * 20);
                float fInput = vInput/100f;
                volumeSlider.value = (fInput);
            }
        }
    }

    


}
