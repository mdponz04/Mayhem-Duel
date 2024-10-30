using TheEnemy;
using UnityEngine;

public class EnemyMovement
{
    private Pathfinding pathfinding;

    public Collider target {  get; set; }
    public EnemyMovement(Pathfinding pathfinding)
    {
        this.pathfinding = pathfinding;
    }
    public void HandleMoving(Collider other, float attackRange, Transform enemyTransform)
    {
        if (other != null)
        {
            float bufferDistance = 0.5f;
            float distanceToTarget = GetEdgeDistance(other, attackRange, enemyTransform);

            if (distanceToTarget <= 0f)
            {
                pathfinding.StopMoving();
            }
            else if (distanceToTarget >= bufferDistance)
            {
                pathfinding.ResumeMoving();
            }
        }
    }
    public void SetTarget(Collider newTarget)
    {
        if (newTarget != null)
        {
            target = newTarget;
            pathfinding.ChaseTarget(target.transform);
        }
        else
        {
            target = null;
            pathfinding.ChaseTarget(null);
        }
        
    }

    private float GetEdgeDistance(Collider target, float myColliderRadius, Transform myTransform)
    {
        Vector3 closestPointOnOther = target.ClosestPoint(myTransform.position);
        float distanceToClosestPoint = Vector3.Distance(myTransform.position, closestPointOnOther);
        return distanceToClosestPoint - myColliderRadius;
    }
    public bool isMoving() => pathfinding.isMoving();
}
