using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternGenerator : MonoBehaviour
{
    public static BubbleBullet bulletPrefab1;
    
    public static BulletPatternGenerator instance;
    public PatternInfo[][] patterns  = new PatternInfo[][] {
        new PatternInfo[3],
        new PatternInfo[3],
        new PatternInfo[3],
        new PatternInfo[3],
        new PatternInfo[3]
    };

    private BubbleBulletPattern Linear()
    {
        return (BubbleBulletPatternLinear)Resources.Load("Prefabs/BubbleBulletPatternLinear",
                                                         typeof(BubbleBulletPatternLinear));
    }
    private BubbleBulletPattern Petal()
    {
        return (BubbleBulletPatternLemniscate)Resources.Load("Prefabs/BubbleBulletPatternLemniscate",
                                                             typeof(BubbleBulletPatternLemniscate));
    }
    public BubbleBulletPattern instantiatePatternInfo(PatternInfo pattern,
                                                      BubbleSpirit parent)
    {
        BubbleBulletPattern newPattern;
        switch (pattern.patternType)
        {
            case PatternType.Linear:
                newPattern = Instantiate(Linear(), parent.transform); break;
            case PatternType.Petal:
                newPattern = Instantiate(Petal(), parent.transform); break;
            default:
                Debug.Log("maybe bad pattern type ...");
                newPattern = Instantiate(Linear(), parent.transform); break;
        }
        newPattern.Init(pattern);
        newPattern.arg.bulletPrefab = bulletPrefab1;
        newPattern.parentBubble = parent;
        return newPattern;
    }

    public void Awake()
    {
        bulletPrefab1 = (BubbleBullet)Resources.Load("Prefabs/BubbleBulletPrefab",
                                                     typeof(BubbleBullet));
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                patterns[i][j] = new PatternInfo
                {
                    bcnt = 1, // bullet count
                    θ = 0,    // base angle
                    σ = 0,    // angle variation: +/- variation from player dir
                    Δθ = 0,   // angle delta: spread between each bullet
                    ω = 0,    // angular velocity (bullet sprite rotation)
                    a = 0,    // acceleration
                    at = 0,   // disable acceleration after this many seconds 
                    d = 0,    // delay time between each bullet in pattern
                    cd = 5f,  // cooldown time between each fire of pattern
                    ΔΣΘ = 0,  // change in pattern base angle after each firing
                    bulletPrefab = bulletPrefab1, // prefab to spawn
                    v = new double[] { 2.0 },     // velocity parameters (see class)
                    patternType = PatternType.Linear, // pattern velocity type
                };
            }
        } 
        // 15deg = 0.2617994 = PI/12
        const float PI = Mathf.PI;
        // EASY //
        // shoot 1 bullet every 5 seconds
        patterns[0][0] = new PatternInfo
        { bcnt = 1, θ = 0, σ = 0, Δθ = 0, ω = 0, a = 0, at = 0,
          d = 0, cd = 5f, ΔΣΘ = 0, v = new double[] { 2.0 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        // 3 shots in quick succession with some noise in player-aim
        patterns[0][1] = new PatternInfo
        { bcnt = 3, θ = 0, σ = PI/24, Δθ = 0, ω = 0, a = 0, at = 0,
          d = 0.3f, cd = 9f, ΔΣΘ = 0, v = new double[] { 3.5 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };
        
        // bewitching lotus pattern: 4x 90 degree shot that turns inward 
        patterns[0][2] = new PatternInfo
        { bcnt = 4, θ = 0, σ = 0, Δθ = PI/2f, ω = 0, a = -2.25f, at = 2,
          d = 0, cd = 5f, ΔΣΘ = PI/12f, v = new double[] { 2.4 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        // NORMAL //
        // shoot a 3-bullet spread shot every 7 seconds
        patterns[1][0] = new PatternInfo
        { bcnt = 3, θ = -PI/12f, σ = 0, Δθ = PI/12f, ω = 0, a = 0, at = 0,
          d = 0, cd = 7f, ΔΣΘ = 0, v = new double[] { 2.5 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        // bewitching lotus 2 
        patterns[1][1] = new PatternInfo
        { bcnt = 4, θ = 0, σ = 0, Δθ = PI/2f, ω = 0.2f, a = -2.25f, at = 2,
          d = 0, cd = 3.75f, ΔΣΘ = PI/12f, v = new double[] { 2.4 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        // slow spiral
        patterns[1][2] = new PatternInfo
        { bcnt = 1, θ = -PI/6f, σ = 0, Δθ = PI/6f, ω = 0, a = 0, at = 0,
          d = 0, cd = 0.25f, ΔΣΘ = PI/6f, v = new double[] { 1.75 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };        

        // ADVANCED //
        // two-armed spiral
        patterns[2][0] = new PatternInfo
        { bcnt = 2, θ = -PI/6f, σ = 0, Δθ = PI, ω = 0, a = 0, at = 0,
          d = 0, cd = 0.425f, ΔΣΘ = PI/6f, v = new double[] { 2 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };        

        // spread 2 (back, then accelerate forwards to pretty fast speed)
        patterns[2][1] = new PatternInfo
        { bcnt = 3, θ = -PI/16f, σ = 0, Δθ = PI/16f, ω = 0, a = -4.5f, at = 1.5f,
          d = 0, cd = 5f, ΔΣΘ = 0, v = new double[] { -3.0 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        // single bullet? lol

        // HARD //
        // fast random shots
        patterns[3][0] = new PatternInfo
        { bcnt = 6, θ = 0, σ = 2*PI, Δθ = 0, ω = 0, a = 0, at = 0,
          d = 0.1f, cd = 0.75f, ΔΣΘ = 0, v = new double[] { 5.8 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        // double fans
        patterns[3][1] = new PatternInfo
        { bcnt = 14, θ = -PI/32f, σ = 0, Δθ = PI - PI/20f, ω = 0, a = 0, at = 0,
          d = 0, cd = 0.350f, ΔΣΘ = PI/138f, v = new double[] { 1.2 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        // single bullet with noise
        patterns[3][2] = new PatternInfo
        { bcnt = 1, θ = 0, σ = PI/12, Δθ = 0, ω = 0, a = 0, at = 0,
          d = 0, cd = 4.5f, ΔΣΘ = 0, v = new double[] { 2.0 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        
        // LUNATIC //        
        // lemniscate velocity pattern
        patterns[4][0] = new PatternInfo
        { bcnt = 32, θ = 0, σ = 0, Δθ = 2 * 0.09817477f, ω = 0, a = 0, at = 0,
          d = 0, cd = 2.5f, ΔΣΘ = 0, v = new double[] { 2.08, 0.0, 4.0, 1.5, 2.0, 0.5 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Petal };

        // gengetsu right /before/ timeout phase
        patterns[4][1] = new PatternInfo
        { bcnt = 8, θ = 0, σ = 0, Δθ = PI - PI/48, ω = 0, a = 0, at = 0,
          d = 0, cd = 0.25f, ΔΣΘ = PI/18, v = new double[] { 5.5 },
          bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        //patterns[0][0] = new PatternInfo
        //{ bcnt = 22, θ = -PI/32f, σ = 0, Δθ = PI - PI/28f, ω = 0, a = 0, at = 0,
        //  d = 0, cd = 0.350f, ΔΣΘ = PI/48f, v = new double[] { 1.8 },
        //  bulletPrefab = bulletPrefab1, patternType = PatternType.Linear };

        
        instance = this;
    }

    // Method used to add a savedPatern. Doesn't work cause Idk how this works. :(
    public void addSavedPattern(BubbleSpirit b, PatternInfo info)
    {
        var pattern = instantiatePatternInfo(info, b);
        pattern.setPatternInfo(info);
        b.pattern = pattern;
    }


    public float generateNormalRandom(float mu, float sigma)
    {
        float rand1 = Random.Range(0.0f, 1.0f);
        float rand2 = Random.Range(0.0f, 1.0f);

        float n = Mathf.Sqrt(-2.0f * Mathf.Log(rand1))
            * Mathf.Cos((2.0f * Mathf.PI) * rand2);

        return (mu + sigma * n);
    }

    public void addToBubble(BubbleSpirit b, int unit_type, float difficulty)
    {
        float patternChance = 100.0f - difficulty / 3f;
        float noneShooter = Random.Range(-1.0f, 2.0f);
        //TODO[FINAL] research grid position, color, number in unit to
        // modulate the params of patterns
        if (unit_type == 1) //boss detection
        {
            if (b.gridPosition == Vector2Int.zero)
            {
                b.pattern = instantiatePatternInfo(patterns[4][0], b);
            }
        }
        float dist = generateNormalRandom(difficulty - 2, 1.75f);
        int bucket_num = Mathf.Min(Mathf.Max(Mathf.FloorToInt(dist / 20), 0), 4);
        if (unit_type >= 0) // add patterns to boss bubbles too now
        {
            //chance to not use pattern
            if (Random.value <= patternChance)
            {
                var bucket = patterns[bucket_num];
                var choice = bucket[Random.Range(0, bucket.Length)];
                b.pattern = instantiatePatternInfo(choice, b);
            }
        }
    }
}
