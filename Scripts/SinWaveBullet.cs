using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinWaveBullet : MonoBehaviour
{
    public float MoveSpeed;
 
     public float frequency;  // Speed of sine movement
     public float magnitude;   // Size of sine movement
     public Vector3 axis;
     public Vector3 input;
 
     public Vector3 pos;
     public bool disabled = false;
 
     void Start () {
         pos = transform.position;
         Destroy(gameObject, 3.0f);
         MoveSpeed = 20f;
         frequency = 10f;
         magnitude = .8f;
         //axis = transform.right * -1;  // May or may not be the axis you want
         
     }
     
     void Update () {
         axis = input;
         pos += transform.up * Time.deltaTime * MoveSpeed;
         transform.position = pos + axis * Mathf.Sin (Time.time * frequency) * magnitude;
     }
     public void SetEven()
     {
         input = transform.right;
     }
     public void SetOdd()
     {
         input = transform.right * -1;
     }

     private void OnCollisionEnter2D(Collision2D c)
    {
        if (disabled) return;
        
        switch (c.gameObject.tag)
        {
            case "Wall Top":
                disabled = true;
                destroySelf();
                break;
            case "BubbleSpirit":
                //disabled by bs collision
                if (c.gameObject.GetComponent<BubbleSpirit>().state == BubbleSpirit.State.NORMAL)
                    destroySelf();
                break;
            default:
                return;
        };
    }

    private void destroySelf()
    {
        Destroy(transform.gameObject);  // kills self
    }
}
