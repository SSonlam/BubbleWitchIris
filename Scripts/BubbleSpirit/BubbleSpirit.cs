using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


//pseudo enum
[Serializable]
public struct BubbleColor
{
    public const int red = 0,
        blue = 1,
        yellow = 2,
        purple = 3,
        rainbow = 4,
        garbage = 5;
    public const int count = garbage + 1;
    // match function that ignores 'garbage' block color
    // and matches any color against 'rainbow'
    public static bool match(int a, int b)
    {
        return (a != garbage || b != garbage)
            && (a == b
                || (a == rainbow || b == rainbow));
    }
}

public class BubbleSpirit : MonoBehaviour
{
    public Texture2D redTexture = null;
    public Texture2D blueTexture = null;
    public Texture2D yellowTexture = null;
    public Texture2D purpleTexture = null;
    private Sprite mySprite;
    private Light2D myLight;
    //public Texture2D[] colorTextures = new Texture2D[]{redTexture, blueTexture, yellowTexture};

    public enum State {
        NORMAL = 0,
        CAPTURED,
        LAUNCHED,
        CLEARED
    }
    
    public BubbleUnit parentUnit;
    private GridLayout parentGrid; // this one is ok to keep, since it's just cached
    public Vector2Int gridPosition;

    public State state; // could probably be private
    public int color;
    public float bubbleSpeed = 20f;
    public int rebounds = 1;
    public Vector3 launchDirection;
    public bool searched; // for parent path searching ... not impl yet.
    private PlayerBehavior playerTarget = null;

    public BubbleBulletPattern pattern; //maybe an array
    public bool cleared;
    public bool isChain;
    public int neighborCount;

    void Awake()
    {
        state = State.NORMAL;
    }

    void Start()
    {
        myLight = GetComponent<Light2D>();
        playerTarget = (PlayerBehavior)FindObjectOfType(typeof(PlayerBehavior));
        if(state == State.NORMAL)
        {
            updateNeighborCountActual();
        }
    }

    [SerializeField] private float orbit_x = 0.9f;
    [SerializeField] private float orbit_y = 0.4f;
    void FixedUpdate()
    {
            
        //myLight.pointLightOuterRadius = Mathf.Lerp(minRange, maxRange, Mathf.PingPong(Time.time, flickerSpeed));
        // only states with frame behavior are capture and launch,
        // where they have projectile-like behavior
        float delta = bubbleSpeed * Time.fixedDeltaTime;
        switch (state)
        {
            case State.NORMAL: 
                break;
            case State.CAPTURED:
                if(GameManager.theManager.GetBubbleCounter() == 1)
                {
                    GameManager.theManager.setCleared();
                }
                float a = transform.parent.GetComponent<PlayerBehavior>().angle;
                var lookDir = Vector3.RotateTowards(transform.parent.up, -transform.parent.right,
                                                    a * Mathf.Deg2Rad, 1.0f);
                lookDir.y *= orbit_y; lookDir.x *= orbit_x;
                if (Vector3.Distance(transform.localPosition, Vector3.zero) <= 1.5f)
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition,
                                                                  lookDir,
                                                                  delta);
                else
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition,
                                                                  Vector3.zero,
                                                                  delta);
                break;
            case State.LAUNCHED:
                // travel in launchDirection until a collision happens
                transform.position += launchDirection * delta; //delta bugged
                if (transform.position.x < 0f || transform.position.y < 0f
                || transform.position.x > 45f || transform.position.y > 30f)
                    Clear();
                break;
            case State.CLEARED:
                myLight.enabled = false;
                clearAnimation();
                break;
        }        
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D other = collision.collider;
        switch (state)
        {
            case State.NORMAL:
                if(other.gameObject.tag == "Bullet"
                   && !other.GetComponent<PlayerBulletBehavior>().disabled)
                {
                    FindObjectOfType<AudioManager>().Play("Bubble_Hit"); 
                    FindObjectOfType<AudioManager>().Play("Bubble_Cleared");
                    other.GetComponent<PlayerBulletBehavior>().disabled = true;
                    Clear();
                }
                if(other.gameObject.tag == "Beam"
                   && !other.GetComponent<SinWaveBullet>().disabled)
                {
                    FindObjectOfType<AudioManager>().Play("Bubble_Hit"); 
                    FindObjectOfType<AudioManager>().Play("Bubble_Cleared");
                    other.GetComponent<SinWaveBullet>().disabled = true;
                    Clear();
                }
                if(other.gameObject.tag == "Splash"
                   && !other.GetComponent<SplashBullet>().disabled)
                {
                    FindObjectOfType<AudioManager>().Play("Bubble_Hit"); 
                    FindObjectOfType<AudioManager>().Play("Bubble_Cleared");
                    other.GetComponent<SplashBullet>().disabled = true;
                    Clear();
                }
                if(other.gameObject.tag == "SplashDamage")
                {
                    FindObjectOfType<AudioManager>().Play("Bubble_Hit"); 
                    FindObjectOfType<AudioManager>().Play("Bubble_Cleared");
                    Clear();
                }
                if(other.gameObject.tag == "Capture"
                   && !other.GetComponent<CaptureBulletBehavior>().disabled)
                {
                    other.GetComponent<CaptureBulletBehavior>().disabled = true;
                    Captured();
                }
                if(other.gameObject.tag == "Wall Top")
                {
                    BubbleUnit parentScript = this.transform.parent.GetComponent<BubbleUnit>();

                    parentScript.hitWall();
                    //Debug.Log("hitting wall");
                }
                break;

            case State.LAUNCHED:
                // hits a bubble: stick, get parent, add, and call trymatch
                if (other.gameObject.tag == "BubbleSpirit"
                    && other.GetComponent<BubbleSpirit>().state == State.NORMAL)
                {
                    FindObjectOfType<AudioManager>().Play("Bubble_HitBubble");
                    AdoptedBy(other.GetComponent<BubbleSpirit>().parentUnit);
                    state = State.NORMAL;
                    tryMatch();
                } else if (other.gameObject.tag == "Wall Top")
                {
                    if (rebounds-- > 0)
                    {
                        launchDirection = Vector3.Reflect(launchDirection, collision.GetContact(0).normal);
                    } else
                    {
                        //Change this to something else like impact collision with wall
                        //FindObjectOfType<AudioManager>().Play("Bubble_Hit"); 
                        FindObjectOfType<AudioManager>().Play("Bubble_Cleared");
                        Clear();
                    }
                }
                //TODO if hits a wall, reparent into new unit instead? discuss
                break;
        }
    }

    // get colors from BubbleColor.red/blue/etc
    public void SetColor(int bubbleColor)
    {
        if (bubbleColor >= BubbleColor.count)
        {
            Debug.LogError("bad bubble color!! got: " + bubbleColor, this);
        }
        color = bubbleColor;
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        // this sucks and unity is not making it easy on me
        // swapping out materials didnt work as easily as hoped
        // maybe need to be looked into more
        // alternate method: create sprite and swap out sprites completely
        if (color == BubbleColor.red)
        {
            myLight = GetComponent<Light2D>();
            myLight.color = Color.red /3f;
            mySprite = Sprite.Create(redTexture, new Rect(0.0f, 0.0f, redTexture.width, redTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            //sr.material.mainTexture = redTexture;
        }
        if (color == BubbleColor.blue)
        {
            myLight = GetComponent<Light2D>();
            myLight.color = Color.blue; 
            mySprite = Sprite.Create(blueTexture, new Rect(0.0f, 0.0f, blueTexture.width, blueTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            //sr.material.mainTexture = blueTexture;
        }
        if (color == BubbleColor.yellow)
        {
            myLight = GetComponent<Light2D>();
            myLight.color = Color.yellow;
            mySprite = Sprite.Create(yellowTexture, new Rect(0.0f, 0.0f, yellowTexture.width, yellowTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
           //sr.material.mainTexture = yellowTexture;
        }
        if (color == BubbleColor.purple)
        {
            myLight = GetComponent<Light2D>();
            myLight.color = Color.red + Color.blue;
            mySprite = Sprite.Create(purpleTexture, new Rect(0.0f, 0.0f, purpleTexture.width, purpleTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
           //sr.material.mainTexture = purpleTexture;
        }
        sr.sprite = mySprite;
    }

    public void setLaunch()
    {
        state = State.LAUNCHED;
    }

    public bool tryLaunch(Vector3 direction)
    {
        if (state != State.CAPTURED)
        {
            Debug.LogWarning("tried to launch a spirit that isn't captured");
            return false;
        } else if
                (Vector3.Distance(transform.position,
                                  transform.parent.position)
                 > 2.0f) {
            Debug.Log("not close enough to player for launch! wait on anim");
            return false;
        } else
        {
            FindObjectOfType<AudioManager>().Play("Iris_Launch");
            state = State.LAUNCHED;
            launchDirection = direction;
            Unparent();
            return true;
        }
    }

    // called once grid position is determined.
    private void UpdateGridPosition()
    {
        transform.localPosition = parentGrid.CellToLocal((Vector3Int)gridPosition);
    }

    // used when an independent bubble collides and is added to a bubbleunit
    // e.g. before capture chaining to determine the closest grid placement
    // TODO[BETA]: lerp to new localposition for smoother snap?
    private void SnapToGrid()
    {
        gridPosition = (Vector2Int)parentGrid.WorldToCell(transform.position);
        UpdateGridPosition();
    }

    private void AdoptedBy(BubbleUnit newParentUnit)
    {
        /* for combining/launching captured bubbles to set new parent */
        transform.parent = newParentUnit.transform;
        // should this be in start or moved to a separate UpdateParent()?
        parentGrid = transform.parent.GetComponent<GridLayout>();
        parentUnit = newParentUnit;
        SnapToGrid();
        parentUnit.addBubble(this);
        UpdateGridPosition();
    }

    private void Unparent() 
    {
        if (parentUnit) parentUnit.removeBubble(this);
        transform.parent = null;
        parentUnit = null;
    }
    //        (-1, 1) ( 0, 1)
    //    (-1, 0) ( 0, 0) ( 1, 0)
    //        (-1,-1) ( 0,-1)
    public void setParent(BubbleUnit newParentUnit, Vector2Int cell)
    {
        gridPosition = cell;
        // move to separate updateparent?
        transform.parent = newParentUnit.transform;
        parentGrid = transform.parent.GetComponent<GridLayout>();
        parentUnit = newParentUnit;
        parentUnit.addBubble(this);
        UpdateGridPosition();
    }

    private void Captured()
    {
        state = State.CAPTURED;
        Unparent();
        transform.parent = playerTarget.transform;
        rebounds = 1;
    }

    private bool isUpdatePending = false;
    public void updateNeighborCount()
    {
        if (isUpdatePending || cleared)
            return;
        // wait a little bit so all the changes are in
        Invoke("updateNeighborCountActual", 0.04f); 
        isUpdatePending = true;
    }

    private void updateNeighborCountActual()
    {
        if (cleared) return;
        isUpdatePending = false;
        neighborCount = 0;
        BubbleNeighbors bn = parentUnit.getNeighbors(this);
        foreach (BubbleSpirit neighbor in bn.neighbors)
        {
            if (neighbor != null) neighborCount++;
        }
    }

    private void tryMatch()
    {
        BubbleNeighbors bn = parentUnit.getNeighbors(this);
        //Debug.Log(string.Join(",",bn.List()));
        bool hasMatch = false;
        //it's possible that the one we collided with is not the match,
        //but there is a match after snapping, so check all
        foreach (BubbleSpirit neighbor in bn.neighbors)
        {
            if (neighbor != null &&
                BubbleColor.match(neighbor.color, this.color))
            {
                FindObjectOfType<AudioManager>().Play("Bubble_Matched");
                hasMatch = true;
                break;
            } else if (neighbor != null)
            {
                neighbor.updateNeighborCount();
            }
        }
        if (hasMatch)
        {            
            ChainClear();
        } else
        {
            updateNeighborCount();
        }
    }
    
    public void Clear()
    {
        if (state == State.NORMAL)
        {
            BubbleNeighbors bn = parentUnit.getNeighbors(this);
            foreach (BubbleSpirit neighbor in bn.neighbors)
            {
                if (neighbor != null)
                    neighbor.updateNeighborCount();
            }
        }
        cleared = true;
        state = State.CLEARED;       
        Unparent();
        if (pattern != null)
            Destroy(pattern.gameObject);
        //trigger animation, yield and delete
    }

    // Method used to animate the bubble spirit after getting hit by a 
    private void clearAnimation()
    {
        Vector3 bubbleSize = transform.localScale;

        if (bubbleSize.x > 1f)
        {
                bubbleSize.x -= 16f * Time.fixedDeltaTime;
                bubbleSize.y -= 16f * Time.fixedDeltaTime;
                transform.localScale = bubbleSize;
        }
        else
        {
            // Change the opacity of the bubble spirit to indicate that it is cleared
            SpriteRenderer bubbleRender = GetComponent<SpriteRenderer>();
            Color bubbleColor = bubbleRender.color;

            if (bubbleColor.a != 0.4f)
            {
                bubbleColor.a = 0.4f;
                bubbleRender.color = bubbleColor;
            }

            // Hard code making sure that the scale of the bubble spirit is 1x1
            if (bubbleSize.x != 1f)
            {
                bubbleSize.x = 1f;
                bubbleSize.y = 1f;
                transform.localScale = bubbleSize;
            }

            Vector3 IrisPos = GameObject.FindGameObjectWithTag("Player").transform.localPosition;

            // initialize a distanceFrom(Iris) float to keep track of distance
            float distanceFrom = Vector3.Distance(IrisPos, transform.localPosition);

            if (distanceFrom > 0.5f)
            {
                // Two different lerp function to facilitate a specific look for the bubble spirit travel speed
                if(distanceFrom > 10f)
                {
                    transform.position = Vector3.Lerp(transform.position, IrisPos, 5f * Time.fixedDeltaTime); 
                }
                else if (distanceFrom > 2.5f)
                {
                    transform.position = Vector3.Lerp(transform.position, IrisPos, 8f * Time.fixedDeltaTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, IrisPos, bubbleSpeed * Time.fixedDeltaTime);
                }
                //Play this sound when bubble almost reaches the player
                //FindObjectOfType<AudioManager>().Play("Bubble_Collect");
            }
            else
            {
                // call clear destroy once reach 0.5f distance from Iris
                if(isChain == true)
                {
                    chainDestroy();
                }
                else
                {
                    clearDestroy();
                }
            }
        }
    }


    private void clearDestroy()
    {       
        RunStatistics.Instance.bubblesCleared++;
        RunStatistics.Instance.totalScore += 15;
        GameManager.theManager.bubbleCleared(true);
        Destroy(gameObject);
    }

    private void chainDestroy()
    {
        RunStatistics.Instance.bubblesCleared++;
        RunStatistics.Instance.bubblesChainCleared[color]++;
        GameManager.theManager.bubbleCleared(false);
        playerTarget.addtrapCount();
        GameManager.theManager.addChain();
        Destroy(gameObject);
    }

    public void ChainClear()
    {
        if (cleared)
            return;  // set 0
        //RunStatistics.Instance.bubblesChainCleared[color]++;
        cleared = true; // set intsum 1

        List<BubbleSpirit> bn_list = parentUnit.getNeighbors(this).neighbors;
        foreach (BubbleSpirit bn in bn_list)
        {
            if (bn != null && BubbleColor.match(color, bn.color))
            {
                bn.ChainClear(); // sum += bm.chaincleared()                
            }
        }
        Debug.Log("playerTarget.getTrapCount(): " + playerTarget.getTrapCount());
        isChain = true;
        Clear();
        //return sum
    }

    public void quickSaveRestoreCapture()
    {
        state = State.CAPTURED;
    }
}
