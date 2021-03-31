using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableVector 
{
    public float x;
    public float y;
    public int twoX;
    public int twoY;
    public float z = 1;

    public SerializableVector(Vector3 userInput)
    {
        x = userInput.x;
        y = userInput.y;
        z = userInput.z;
    }

    public SerializableVector(Vector2 userInput)
    {
        x = userInput.x;
        y = userInput.y;
    }

    public SerializableVector(Vector2Int userInput)
    {
        twoX = userInput.x;
        twoY = userInput.y;
    }

    public SerializableVector(Quaternion userInput)
    {
        x = userInput.x;
        y = userInput.y;
        z = userInput.z;
    }

    public Vector3 getVectorThree()
    {
        return new Vector3(x, y, z);
    }

    public Vector2 getVectorTwo()
    {
        return new Vector2(x, y);
    }

    public Vector2Int getVectorTwoInt()
    {
        return new Vector2Int(twoX, twoY);
    }

    public Quaternion getQuaternion()
    {
        return new Quaternion(x, y, z, 1);
    }
    
}
