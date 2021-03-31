using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class IntroSceneController : MonoBehaviour
{
    public Text Slide1Text1 = null;
    public Image Slide1Picture = null;

    public Text Slide2Text1 = null;
    public Text Slide2Text2 = null;
    public Image Slide2Picture = null;
    public Image Slide2Picture2 = null;

    public Text Slide3Text1 = null;
    public Text Slide3Text2 = null;
    public Image Slide3Picture = null;


    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Intro_Theme");  
        PlayFabManager.thePlayFabManager.Login("ccb");
        Slide1Text1.enabled = false;
        Slide1Picture.enabled = false;

        Slide2Text1.gameObject.SetActive(false);
        Slide2Text2.gameObject.SetActive(false);
        Slide2Picture.gameObject.SetActive(false);
        Slide2Picture2.gameObject.SetActive(false);

        Slide3Text1.gameObject.SetActive(false);
        Slide3Text2.gameObject.SetActive(false);
        Slide3Picture.gameObject.SetActive(false);
        StartCoroutine(SlideShow());
    }

    IEnumerator SlideShow()
    {
        Slide1Text1.enabled = true;
        Slide1Picture.enabled = true;
        yield return new WaitForSeconds(8);
        Slide2Text1.gameObject.SetActive(true);
        Slide2Picture.gameObject.SetActive(true);
        yield return new WaitForSeconds(7);
        Slide2Picture2.gameObject.SetActive(true);
        Slide2Text2.gameObject.SetActive(true);
        yield return new WaitForSeconds(8);
        Slide3Picture.gameObject.SetActive(true);
        Slide3Text1.gameObject.SetActive(true);
        yield return new WaitForSeconds(7);
        Slide3Text2.gameObject.SetActive(true);
        yield return new WaitForSeconds(8);
        FindObjectOfType<AudioManager>().Stop("Intro_Theme"); 
        SceneManager.LoadScene("TitleScreen");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            FindObjectOfType<AudioManager>().Stop("Intro_Theme"); 
            SceneManager.LoadScene("TitleScreen");
        }
    }
}
