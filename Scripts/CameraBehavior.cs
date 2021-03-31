using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraBehavior : MonoBehaviour
{
    public Transform follow;
    public float smoothSpeed = 1.75f;
	public Vector3 p;
    public float scale = 0.0f;
    private Vector3 minValues, maxValue;
    public Vector3 targetPosition;
    public int mapWidth = 45;
    public int mapHeight = 30;

    public float min_x = 8, min_y = 5, max_x = 36, max_y = 24;

    private void Start() 
    {
        minValues.x = min_x;
        minValues.y = min_y;
        minValues.z = -10;
        maxValue.x = max_x;
        maxValue.y = max_y;
        maxValue.z = -10;  
    }
	void FixedUpdate()
    {        
        
    }

     void LateUpdate()
    {
        FollowPlayer();
    }

    public void FollowPlayer()
    {
        Vector3 targetPosition = follow.position;
        //Verify if the targetPosition is out of bound or not
        //Limit it to the min and max values
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValues.x, maxValue.x),
            Mathf.Clamp(targetPosition.y, minValues.y, maxValue.y),
            Mathf.Clamp(targetPosition.z, minValues.z, maxValue.z));

        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }

}
