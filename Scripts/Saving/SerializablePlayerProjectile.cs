using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializablePlayerProjectile 
{
    public SerializableVector bulletDirection; // vector2, used for normal and splash
    public SerializableVector bulletRotation; // Quaternion
    public float lifeSpan; // Used for normal and splash
    public int rebounds;
    public bool disabled; // Used for all 3

    // SinWave
    public float MoveSpeed; // SinWave and SplashBullet

    public float frequency;  // Speed of sine movement
    public float magnitude;   // Size of sine movement
    public SerializableVector axis; // vector 3
    public SerializableVector input; // vector 3 

    public SerializableVector pos; // vector 3

}
