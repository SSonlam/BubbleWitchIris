using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class BubbleBulletPatternLemniscate : BubbleBulletPattern
{
    public override void FireAt(float angle)
    {

        var bullet = (BubbleBullet)Instantiate(arg.bulletPrefab);
        bullet.transform.position = transform.position;

        var v = arg.velocityParameters;
        var l = math.sin(v[3] + v[4] * angle);
        //        var speed = math.pow((v[0] + v[1] * angle + v[2]
        //                                      * l), v[5]);
        var speed = math.sqrt((v[0] + v[1] * angle + v[2] * l));
        if (speed != speed) speed = 0.5f; // float underflow issue ...
        bullet.velocity = new Vector3(Mathf.Cos(angle),
                                      Mathf.Sin(angle),
									  0) * (float)speed;
        bullet.direction = bullet.velocity.normalized;
        bullet.angularVelocity = arg.angularVelocity;
        //could use vector for acceleration too ...
        bullet.acceleration = arg.acceleration;
        bullet.accelerationTimeout = arg.accelerationTime;
    }
}
