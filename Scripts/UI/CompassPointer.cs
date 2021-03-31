using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassPointer : MonoBehaviour
{
    static private Vector3 targetPosition;
    private RectTransform pointerRectTransform;
    Camera cam = null;
    static public void setClosestBubble(Vector3 inputPosition){targetPosition = inputPosition; }
    private float idleTimer;
    private float idleTimerCountdown;

    private void Awake()
    {
        //targetPosition = new Vector3(20, 20);
        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        cam = transform.Find("UiCamera").GetComponent<Camera>();
        idleTimer = 15f;
        idleTimerCountdown = idleTimer;
    }

    private void  FixedUpdate()
    {
        idleTimerCountdown -= 5f * Time.deltaTime;

        Vector3 toPosition = targetPosition;
        Vector3 fromPosition = Camera.main.transform.position;
        fromPosition.z = 0f;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        
        float borderSize = 50f;
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize
                            || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;
        if(Input.GetMouseButton(0))
        {
            idleTimerCountdown = idleTimer;
        }
        if(isOffScreen && idleTimerCountdown <= 0)
        {
                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                if(cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
                if(cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
                if(cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
                if(cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;
                Vector3 pointerWorldPosition = cam.ScreenToWorldPoint(cappedTargetScreenPosition);
                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
        }
        else
        {
            pointerRectTransform.localPosition = new Vector3(1000f, 1000f, 0);
        }
    }
}
