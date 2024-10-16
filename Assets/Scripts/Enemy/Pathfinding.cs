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

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            targetDestination = castleTransform;
        }
        private void Update()
        {
            HandleMoving();
        }
        private void HandleMoving()
        {
            if(!agent.isStopped)
            {
                if (agent.destination != targetDestination.position)
                {
                    agent.destination = targetDestination.position;  // Only update when target changes

                    Debug.Log(gameObject.name + " chase: " + targetDestination.name);
                }
            }
        }
        //Chase the transform(Default is castle transfrom) 
        public void ChaseTarget(Transform transform)
        {
            ResumeMoving();
            if(transform != null)
            {
                targetDestination = transform;
                
                return;
            }

            targetDestination = castleTransform;
            
        }
        public void StopMoving()
        {
            agent.isStopped = true;
        }
        public void ResumeMoving()
        {
            agent.isStopped = false;
        }
    }
}

