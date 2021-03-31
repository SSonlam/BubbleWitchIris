using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    [SerializeField]private PlayerBehavior ParentPlayer = null;
    public void setParent(PlayerBehavior g) { ParentPlayer = g; }

    [SerializeField] private Vector2 offset;
    private SpriteRenderer sprite;
    public int sortingOrder = 0;

    void Start()
    {      
        transform.parent = ParentPlayer.transform;
        transform.localPosition = (Vector3)offset;
    }

    void FixedUpdate()
    {
        //Debug.Log(sprite.sortingOrder);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D other = collision.GetContact(0).collider;
        switch (other.gameObject.tag)
        {
            case "BubbleSpirit":
                if (other.gameObject.GetComponent<BubbleSpirit>().state ==
                    BubbleSpirit.State.NORMAL)
                    ParentPlayer.Hit();
                break;
            case "EnemyBullet":
                ParentPlayer.Hit();
                break;
            default:
                break;
        }
    }

    public void show()
    {
        sprite = GetComponent<SpriteRenderer>();
        sortingOrder = 1;
        sprite.sortingOrder = sortingOrder;
    }
    public void hide()
    {
        sprite = GetComponent<SpriteRenderer>();
        sortingOrder = -1;
        sprite.sortingOrder = sortingOrder;        
    }
}
