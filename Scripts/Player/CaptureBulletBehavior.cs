using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureBulletBehavior : MonoBehaviour
{
    static private PlayerBehavior ParentPlayer = null;
    static public void setParent(PlayerBehavior g) { ParentPlayer = g; }

    public bool disabled = false;
    public int rebounds = 1;
    public const float bulletSpeed = 25f;
    void FixedUpdate()
    {
        transform.localPosition += transform.up * (bulletSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (disabled) return;
        switch (collision.gameObject.tag)
        {
            case "BubbleSpirit":
                if (collision.gameObject.GetComponent<BubbleSpirit>().state == BubbleSpirit.State.NORMAL)
                {
                    BubbleSpirit capturedBubble = collision.gameObject.GetComponent<BubbleSpirit>();
                    ParentPlayer.SetCapture(capturedBubble);
                    destroySelf();
                }
                break;
            case "Wall Top":
                disabled = true;
                destroySelf();
                break;
            default:
                return;
        }
    }

    private void destroySelf()
    {
        //ParentPlayer.eggDestroyed(this);
        //destroyed = true;
        Destroy(transform.gameObject);  // kills self
    }
}
