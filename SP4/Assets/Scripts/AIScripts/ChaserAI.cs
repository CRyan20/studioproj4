using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaserAI : MonoBehaviour
{
    [Header("Chaser States")]
    public EnemyState currState = EnemyState.PATROL;
    public enum EnemyState
    {
        PATROL,
        CHASE,
        ATTACK,
        DEAD
    }

    [Header("Enemy Data")]
    public NavMeshAgent chaser;
    public Transform player;
    private float rotationSpeed = 1f;
    public GameObject lookAt;
    public float lookAtOffset = 0.5f;
    private float originalLookAtYPos;
    public HealthSystem healthSystem;

    [Header("Patrol State")]
    public float patrolSpeed = 2f;
    public Transform[] waypoints;
    public float waypointRange = 3f; //range when enemy swaps to next waypoint
    private int currWaypointIndex = 1;
    private bool hasReachedWaypoint = false;

    [Header("Chase State")]
    public float chaseRange = 15f;
    public float chaseSpeed = 10f;

    [Header("Animation")]
    public Animator animator;

    [Header("Field of View")]
    public float fovAngle = 90f; // Field of view angle
    public float viewDistance = 10f; // Maximum distance the AI can see


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();

        //original y pos
        originalLookAtYPos = lookAt.transform.position.y;
        // Position the eyes relative to the enemy's position and forward direction
        Vector3 lookAtPosition = transform.position + transform.forward * lookAtOffset;
        lookAtPosition.y = originalLookAtYPos;
        lookAt.transform.position = lookAtPosition;
        lookAt.transform.rotation = transform.rotation;
        //start state is patrol
        currState = EnemyState.PATROL;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case EnemyState.PATROL:
                Patrol();
                break;
            case EnemyState.CHASE:
                Chase();
                break;
            case EnemyState.ATTACK:
                Attack();
                break;
            case EnemyState.DEAD:
                Dead();
                break;
        }

        if (chaser.velocity.magnitude > 0.1f) //if its moving
        {
            // Get the direction of movement (normalized)
            Vector3 moveDirection = chaser.velocity.normalized;

            // Calculate the rotation to look in the direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        //health check
        if (healthSystem.currentHealth <= 0)
        {
            healthSystem.currentHealth = 0;
            currState = EnemyState.DEAD;
        }

        //take damage
        
    }

    void Patrol()
    {
        chaser.speed = patrolSpeed;
        if (Vector3.Distance(transform.position, player.position) < chaseRange)
        {
            //switch to chase state
            currState = EnemyState.CHASE;
            return;
        }

        //if it hasnt reached waypoint
        if (!hasReachedWaypoint)
        {
            // Check the distance to the current waypoint
            float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currWaypointIndex].position);

            // Check if the enemy is close enough to the current waypoint
            if (distanceToWaypoint <= waypointRange)
            {
                // Move to the next waypoint
                currWaypointIndex = (currWaypointIndex + 1) % waypoints.Length;
                hasReachedWaypoint = true;
            }
        }
        else
        {
            hasReachedWaypoint = false;
        }

        // Set destination to the current waypoint
        chaser.SetDestination(waypoints[currWaypointIndex].position);
        animator.SetBool("Walking", true);
    }

    void Chase()
    {
        chaser.speed = chaseSpeed;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < chaseRange)
        {
            if (distanceToPlayer > chaser.stoppingDistance)
            {
                chaser.SetDestination(player.position);
            }
            else
            {
                chaser.velocity = Vector3.zero;
                currState = EnemyState.ATTACK;
            }

            if (IsPlayerInFOV(player.position))
            {
                return;
            }
        }
        else
        {
            //if not in range make it patrol state
            currState = EnemyState.PATROL;
        }
    }

    void Attack()
    {
        //health decrease, attack anim here etc
        animator.SetBool("Walking", true);
        animator.SetBool("Attack", true);

        //not in range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > chaseRange)
        {
            animator.SetBool("Attack", false);
            currState = EnemyState.PATROL;
        }
    }

    void Dead()
    {
        chaser.velocity = Vector3.zero;
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    IEnumerator DestroyAfterDeathAnimation()
    {
        animator.SetBool("Death", true);
        yield return new WaitForSeconds(2.1f);
        Destroy(gameObject);
    }

    bool IsPlayerInFOV(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        float angle = Vector3.Angle(directionToTarget, transform.forward);

        if (angle <= fovAngle * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        // Draw the field of view
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, fovAngle * 0.5f, 0) * transform.forward * viewDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -fovAngle * 0.5f, 0) * transform.forward * viewDistance);
    }
}
