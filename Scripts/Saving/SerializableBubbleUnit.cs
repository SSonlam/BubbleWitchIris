using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableBubbleUnit
{
    public SerializableVector unitPosition; // vector3
    public List<BubbleSpirit> neighbors;
    public List<SerializableVector> offsets; // vector2Int
    public SerializableBubbleSpirit[] childrenBubble;
    public SerializableVector initialPosition; // vector3
    public SerializableVector movePosition; // vector3
    public float timeToMove;
    public float moveTimer;
    public float radius;
    public int bubbleCount;
}
