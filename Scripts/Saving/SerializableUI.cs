using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerializableUI 
{
    public float rollCooldown;
    public float rollFill;
    public float trapCooldown;
    public float trapFill;
    public float captureCooldown;
    public float captureFill;
    public int trapCount;

    public bool trapMax;
    public bool captureMax;
    public bool rollMax;
    public bool lifeMax;
}
