using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheEnemy
{
    public class Pathfinding : MonoBehaviour
    {
        [SerializeField] private Transform castleTransform;
        private Transform targetDestination;
        private NavMeshAgent agent;
        private float originalSpeed;
        private float resumeMovingCooldown = 1f;
        private float nextTimeResumeMoving = 0f;
        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            targetDestination = castleTransform;
            agent.destination = targetDestination.position;
            originalSpeed = agent.speed;
            
        }
        //Chase the transform(Default is castle transfrom) 
        public void ChaseTarget(Transform target)
        {
            if(target != null)
            {
                targetDestination = target;
            }
            else
            {
                targetDestination = castleTransform;
            }

            // Set the new destination immediately
            agent.SetDestination(targetDestination.position);
        }
        public void StopMoving()
        {
            agent.speed = Mathf.Lerp(agent.speed, 0f, Time.deltaTime * 3f); 
            if (agent.speed < 0.1f)
            {
                agent.speed = 0f;
                agent.isStopped = true;
            }
        }
        public void ResumeMoving()
        {
            if(Time.time > nextTimeResumeMoving)
            {
                agent.speed = originalSpeed;
                agent.isStopped = false;
            }
            nextTimeResumeMoving = Time.time + resumeMovingCooldown;
        }

        
    }
}

