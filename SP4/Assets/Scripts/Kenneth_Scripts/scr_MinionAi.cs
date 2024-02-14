using UnityEngine.AI;
using UnityEngine;

public class scr_MinionAi : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public LayerMask whatIsGround, whatIsPlayer;
    private Animator animator;

    [Header("Stats")]
    public float health = 65;

    // Patrolling
    [Header("Patrol")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    [Header("Attack")]
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // Range
    [Header("Range")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // States
    private enum MinionState
    {
        Patrol,
        Chase,
        Attack,
        Dead,
    }

    private MinionState currentState = MinionState.Patrol;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
        //target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check if player in sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        switch (currentState)
        {
            case MinionState.Patrol:
                if (playerInSightRange && !playerInAttackRange)
                {
                    currentState = MinionState.Chase;
                    ChasePlayer();
                }
                else if (playerInSightRange && playerInAttackRange)
                {
                    currentState = MinionState.Attack;
                    AttackPlayer();
                }
                else
                {
                    Patroling();
                }
                break;

            case MinionState.Chase:
                if (!playerInSightRange)
                {
                    currentState = MinionState.Patrol;
                    Patroling();
                }
                else if (playerInAttackRange)
                {
                    currentState = MinionState.Attack;
                    AttackPlayer();
                }
                else
                {
                    ChasePlayer();
                }
                break;

            case MinionState.Attack:
                if (!playerInSightRange)
                {
                    currentState = MinionState.Patrol;
                    Patroling();
                }
                else if (!playerInAttackRange)
                {
                    currentState = MinionState.Chase;
                    ChasePlayer();
                }
                else
                {
                    AttackPlayer();
                }
                break;

            case MinionState.Dead:
                // Handle Dead state if needed
                break;
        }

    }

    private void Patroling()
    {
        animator.SetBool("isChasing", false);
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        animator.SetBool("isChasing", true);
        agent.SetDestination(target.position);
        agent.speed = 5;
        agent.acceleration = 6;
    }

    private void AttackPlayer()
    {
        // Make sure enemy does not move
        agent.SetDestination(transform.position);

        transform.LookAt(target);
        
        if (!alreadyAttacked)
        {
            /// Attack code here
            
            ///
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
