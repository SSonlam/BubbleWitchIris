using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBulletPatternLinear : BubbleBulletPattern
{
    public override void FireAt(float angle)
    {
        var bullet = (BubbleBullet)Instantiate(arg.bulletPrefab);

        bullet.transform.position = transform.position;

        bullet.velocity = new Vector3(Mathf.Cos(angle),
                                      Mathf.Sin(angle),
                                      0) * (float)arg.velocityParameters[0];
        bullet.direction = bullet.velocity.normalized;
        bullet.angularVelocity = arg.angularVelocity;
        //could use vector for acceleration too ...
        bullet.acceleration = arg.acceleration;
        bullet.accelerationTimeout = arg.accelerationTime;
    }
}
