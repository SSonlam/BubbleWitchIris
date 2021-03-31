using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class TitleButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler 
// required interface when using the OnPointerEnter and OnPointerUp method.
{
    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<AudioManager>().Play("Menu_Hover");
        //Debug.Log(gameObject.name + " hovered.");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //Text newPlayButton = transform.FindChild("Text").GetComponent<Text>();
        if (gameObject.name == "newPlayButton")
        {
            //Debug.Log(gameObject.name + " clicked.");
            return;
            //Sound is played in the Play Button script (TitleButton.cs)         
        }
        else
        {    
            //Debug.Log(gameObject.name + " clicked."); 
            FindObjectOfType<AudioManager>().Play("Menu_Clicked");               
        }
    }

    /* If you wanna use any of these methods, please add the handlers at line 5
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("The cursor exited the selectable UI element. " + eventData);
    }
    public void OnBeginDrag(PointerEventData data)
    {
        Debug.Log("OnBeginDrag: " + data.position);
        data.pointerDrag = null;
    }
    public void OnDrag(PointerEventData data)
    {
        if (data.dragging)
        {
            timeCount += Time.deltaTime;
            if (timeCount > 1.0f)
            {
                Debug.Log("Dragging:" + data.position);
                timeCount = 0.0f;
            }
        }
    }
    public void OnEndDrag(PointerEventData data)
    {
        Debug.Log("OnEndDrag: " + data.position);
    }
    */
}