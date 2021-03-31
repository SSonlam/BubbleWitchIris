using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBeam : MonoBehaviour
{
    public const float bulletSpeed = 15f;
    void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), GetComponent<Collider>());
    }
    void FixedUpdate()
    {
        transform.localPosition += transform.up * (bulletSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        //if (disabled) return;
        
        switch (c.gameObject.tag)
        {
            case "Wall Top":
                //disabled = true;
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
        //ParentPlayer.eggDestroyed(this);
        //destroyed = true;
        Destroy(transform.gameObject);  // kills self
    }
}
