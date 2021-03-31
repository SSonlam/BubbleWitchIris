using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Mathematics;

[System.Serializable]
public enum PatternType : int
{
    Linear,
    Petal
}

[System.Serializable]
[StructLayout(LayoutKind.Explicit,Size=72)]
public struct PatternInfo
{
    [FieldOffset(0)]public int bulletCount;[FieldOffset(0)]public int bcnt;
    [FieldOffset(4)]public float baseAngle;[FieldOffset(4)]public float θ;
    [FieldOffset(8)]public float angleVariation;[FieldOffset(8)] public float σ;
    [FieldOffset(12)]public float angleDelta;[FieldOffset(12)] public float Δθ;
    [FieldOffset(16)]public float angularVelocity;[FieldOffset(16)]public float ω;
    [FieldOffset(20)]public float acceleration;[FieldOffset(20)]public float a;
    [FieldOffset(24)]public float accelerationTime;[FieldOffset(24)]public float at;
    [FieldOffset(28)]public float delayTime;[FieldOffset(28)]public float d;
    [FieldOffset(32)]public float patternCooldown;[FieldOffset(32)]public float cd;
    [FieldOffset(36)]public float patternAngleDeltaAfter;[FieldOffset(36)]public float ΔΣΘ;

    [FieldOffset(40)]public float lifetime;
    [FieldOffset(40)]public float l;
    [FieldOffset(44)]public float patternLifetime;
    [FieldOffset(44)]public float pl;
    [FieldOffset(48)]public int step;

    [FieldOffset(52)]public PatternType patternType; //0 = linear, 2 = petal
    [FieldOffset(56)]public BubbleBullet bulletPrefab;
    [FieldOffset(64)]public double[] velocityParameters;
    [FieldOffset(64)]public double[] v;
}

[System.Serializable]
public abstract class BubbleBulletPattern : MonoBehaviour
{
    public PatternInfo arg;

    public BubbleBulletPattern Init(PatternInfo patternInfo)
    {
        this.arg = patternInfo;
        if (arg.bulletCount > 0 && float.IsNaN(arg.angleDelta))
        {
            arg.angleDelta = (2.0f * Mathf.PI) / arg.bulletCount;
        } else if (arg.bulletCount == 0 && !float.IsNaN(arg.angleDelta))
        {
            arg.bulletCount = Mathf.RoundToInt((2.0f * Mathf.PI) / arg.angleDelta);
        } else {}

        arg.step = 0;
        arg.lifetime = 0;
        arg.patternLifetime = 0.0001f;
        return this;
    }

    private PlayerBehavior playerTarget;
    public BubbleSpirit parentBubble;
    public void Start()
    {
        playerTarget =
            (PlayerBehavior)FindObjectOfType(typeof(PlayerBehavior));
        if (!playerTarget) { Debug.Log("Bullet system cannot find player"); }
    }

    const float activationRadius = 15f;
    public void FixedUpdate()
    {
        if(GameManager.theManager.canMove
           && parentBubble.state == BubbleSpirit.State.NORMAL)
        {
            arg.patternLifetime -= Time.fixedDeltaTime;
            var playerDist = Vector3.Distance(transform.position,
                                              playerTarget.transform.position);
            // 1. is pattern delay up?
            // 2. is the between-bullet delay satisfied?
            // 3. is the player close enough?
            // 4. is the neighbor count pattern activation satisfied?
            if (((arg.lifetime <= 0 && arg.delayTime != 0) || playerDist < activationRadius)
                && arg.patternLifetime <= 0
                && (parentBubble.neighborCount <= 3
                    && (parentBubble.neighborCount % 2 == 0)))
            {
                arg.lifetime -= Time.fixedDeltaTime;
                // while there are more bullets in the cycle and it's time
                while (arg.step < arg.bulletCount && arg.lifetime <= 0f)
                {
                    float angle = Mathf.Atan2(
                        playerTarget.transform.position.y - transform.position.y,
                        playerTarget.transform.position.x - transform.position.x
                    ) + arg.baseAngle + arg.angleDelta * arg.step +
                        Random.Range(-arg.angleVariation,arg.angleVariation);
                    FireAt(angle);
                    
                    ++arg.step;
                    arg.lifetime += arg.delayTime;
                }
                // no more bullets or delay is 0; prime next pattern use
                if (arg.lifetime <= 0f)
                {
                    arg.step = 0;
                    arg.patternLifetime = arg.patternCooldown;
                    arg.baseAngle += arg.patternAngleDeltaAfter;
                } //else {
            }
        }
    }
    public void setPatternParameters(double[] v)
    {
        arg.velocityParameters = v;
    }

    public void setPatternInfo(PatternInfo state)
    {
        arg = state;
    }

    abstract public void FireAt(float angle);
}
