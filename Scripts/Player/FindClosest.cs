using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindClosest : MonoBehaviour
{
    private Vector3 closestEnemyPosition;

    // Update is called once per frame
    void FixedUpdate()
    {
        FindClosestEnemy();
        CompassPointer.setClosestBubble(closestEnemyPosition);
    }

    void FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        BubbleSpirit closestEnemy = null;
        BubbleSpirit[] allEnemies = GameObject.FindObjectsOfType<BubbleSpirit>();

        foreach(BubbleSpirit currentEnemy in allEnemies)
        {
            float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
            if(distanceToEnemy < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distanceToEnemy;
                closestEnemy = currentEnemy;
            }
        }

        if(closestEnemy != null)
        {
            closestEnemyPosition = new Vector3(closestEnemy.transform.position.x, closestEnemy.transform.position.y, 0);
        }
        //Debug.DrawLine(this.transform.position, closestEnemy.transform.position);
    }
}
