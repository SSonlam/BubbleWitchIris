using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearIndicatorBehavior : MonoBehaviour
{
    float ratio = 0;
    float moveDuration = 1f;
    private Vector3 endPos;

    void Start()
    {
        endPos = this.transform.localPosition;
        endPos.y = 250;
    }

    
    void Update()
    {
        if(this.transform.localPosition.y > 250)
        {
            indicatorAnimation();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void indicatorAnimation()
    {
        ratio += Time.deltaTime * (1 / moveDuration);
        Vector3 currentPos = this.transform.localPosition;
        this.transform.localPosition = Vector3.Lerp(currentPos,endPos,ratio);
        this.GetComponent<CanvasGroup>().alpha = 1 - ratio/4;
    }
}
