using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private PlayerStats playerStats;

    [Header("References")]
    public NavMeshAgent agent;

    public Transform player;

    [Header("Layer Masks")]
    public LayerMask whatIsGround, whatIsPlayer;

    //Enemy health
    [Header("Enemy's Health")]
    public float enemyHealth = 100f;

    //Patrolling
    [Header("Walk points")]
    public Vector3 walkPoint;
    bool walkPointSet;

    [Header("Distance")]
    public float walkPointRange;

    [Header("AI Speeds")]
    public float walkspeed;
    public float runspeed;

    //Attacking
    [Header("Attack data")]
    public float timeBetweenAttacks;
    public float sightRange, attackRange;
    bool alreadyAttacked;
    public bool playerInSightRange, playerInAttackRange;

    private void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        Debug.Log(playerStats);
    }

    private void Update()
    {
        //Check if zombie is dead
        if(enemyHealth <= 0)
        {
            Destroy(gameObject);
        }

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walk point reached
        if(distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
        
    }

    private void SearchWalkPoint()
    {
        //Calculate rnadom point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        agent.speed = walkspeed;

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.speed = runspeed;

        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesnt move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            //Attack code
            playerStats.health -= 20;

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
