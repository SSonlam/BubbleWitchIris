using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBehavior : MonoBehaviour
{
    static private PlayerBehavior ParentPlayer = null;
    static public void setParent(PlayerBehavior g) { ParentPlayer = g; }
    private const float bulletSpeed = 20f;
    public Vector2 bulletDirection;
    public float lifeSpan;

    public bool disabled = false;
    // Start is called before the first frame update
    void Start()
    {
        lifeSpan = 1.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition += transform.up * (bulletSpeed * Time.fixedDeltaTime);
        lifeSpan -= Time.fixedDeltaTime;
        if(lifeSpan <= 0 && Vector3.Distance(ParentPlayer.transform.position,transform.position) > 4f)
        {
            destroySelf();
        }
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
        //ParentPlayer.eggDestroyed(this);
        //destroyed = true;
        Destroy(transform.gameObject);  // kills self
    }
}
