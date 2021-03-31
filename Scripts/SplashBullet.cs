using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashBullet : MonoBehaviour
{
    public bool disabled = false;
    private const float bulletSpeed = 20f;
    Vector2 bulletDirection;
    public float lifeSpan;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition += transform.up * (bulletSpeed * Time.fixedDeltaTime);
        lifeSpan -= Time.fixedDeltaTime;
        /*
        if(lifeSpan <= 0 && Vector3.Distance(ParentPlayer.transform.position,transform.position) > 4f)
        {
            destroySelf();
        }
        */
    }


    private void OnCollisionEnter2D(Collision2D c)
    {
        if (disabled) return;
        
        switch (c.gameObject.tag)
        {
            case "Wall Top":
                disabled = true;
                GameObject e = Instantiate(Resources.Load("Prefabs/Whirlwind") as
                                   GameObject);
                e.transform.localPosition = transform.localPosition;
                destroySelf();
                break;
            case "BubbleSpirit":
                //disabled by bs collision
                if (c.gameObject.GetComponent<BubbleSpirit>().state == BubbleSpirit.State.NORMAL)
                {
                    GameObject f = Instantiate(Resources.Load("Prefabs/Whirlwind") as
                                   GameObject);
                    f.transform.localPosition = transform.localPosition;
                    destroySelf();
                }
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
    /*
    private void OnCollisionEnter2D(Collision2D c)
    {
        if (disabled) return;
        
        switch (c.gameObject.tag)
        {
            case "Wall Top":
            case "BubbleSpirit":
                //disabled by bs collision
                disabled = true;
                if (c.gameObject.GetComponent<BubbleSpirit>().state == BubbleSpirit.State.NORMAL)
                {
                    GameObject e = Instantiate(Resources.Load("Prefabs/Splasher") as
                                   GameObject);
                    e.transform.localPosition = transform.localPosition;
                    destroySelf();
                }
                break;
            default:
                return;
        };
    }

    private void destroySelf()
    {
        Destroy(transform.gameObject);  // kills self
    }
    */
}
