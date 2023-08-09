using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {

        #region Patrol Settings
        [Header("Patrol Settings")]
        [Tooltip("Place the Patrol Path for the enemy here")]
        [SerializeField] PatrolPath patrolPath;

        [Tooltip("How close the player needs to be for the enemy to chase")]
        [SerializeField] float chaseDistance = 5f;

        [Tooltip("How long the enemy will stand and wait after the enemy leaves the chaseDistance")]
        [SerializeField] float suspicionTime = 3f;

        [Tooltip("How close the enemy needs to get to the waypoint")]
        [SerializeField] float waypointTolerance = 1f;

        [Tooltip("How long the enemy will wait at the waypoint before continuing")]
        [SerializeField] float waypointDwellTime = 2f;

        //[Tooltip("How fast the enemy will chase the player")]
        //[SerializeField] float chaseSpeed = 4f;

        //[Tooltip("How fast the enemy patrols their patrolPath")]
        //[SerializeField] float patrolSpeed = 1f;

        [Tooltip("Reduces or increases movement based on a percentage (i.e 0.2 is 20% of max speed)")]
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        [Tooltip("Makes the enemy always chase the player once they've been detected")]
        [SerializeField] bool isAggressive = false;
        #endregion
        #region References
        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;
        NavMeshAgent navMeshAgent;
        #endregion
        #region StartPos and timers
        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;
        #endregion

        private void Start()
        {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            
            navMeshAgent = mover.GetComponent<NavMeshAgent>();
            guardPosition = transform.position;
        }

        private void Update()
        {
            if (health.IsDead()) return;
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();

            }
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;
            //navMeshAgent.speed = patrolSpeed;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
            
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            if (isAggressive)
            {
                AttackBehaviour();
            }
            else
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
                //navMeshAgent.speed = chaseSpeed / 2;
            }
        }

        private void AttackBehaviour()
        {
            //navMeshAgent.speed = chaseSpeed;
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;

        }

        // Called by Unity, in editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
