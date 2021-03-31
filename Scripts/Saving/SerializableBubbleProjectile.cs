using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableBubbleProjectile 
{
    public SerializableVector projectilePosition;
    public SerializableVector projectileRotation;
    public SerializableVector projectileDirection;
    public SerializableVector velocity;
    public float angularVelocity;
    public float acceleration;
    public float accelerationTimeout;
}
