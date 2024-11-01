using TheEnemy;
using UnityEngine;
using UnityEngine.AI;

namespace TheEnemy
{
    public class EnemyMove
    {
        private Pathfinding pathfinding;
        public Collider target {  get; set; }

        public EnemyMove(Pathfinding pathfinding)
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
            if(newTarget != null)
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
        //get the distance from 
        private float GetEdgeDistance(Collider other, float myColliderRadius, Transform currentTransform)
        {
            Vector3 closestPointOnOther = other.ClosestPoint(currentTransform.position);
            float distanceToClosestPoint = Vector3.Distance(currentTransform.position, closestPointOnOther);
            return distanceToClosestPoint - myColliderRadius;
        }
        public bool IsMoving() => pathfinding.IsMoving();
    }
}
