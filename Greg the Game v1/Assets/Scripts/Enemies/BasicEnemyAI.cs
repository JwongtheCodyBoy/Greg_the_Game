using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public GameObject parentObject;
    public KillCounter killCounter;
    public SpawnerManager spawnerManager;

    public Transform player;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [Header("Stats")]
    public float health;
    public float accel;

    [Header("Projectile")]
    public GameObject projectile;
    public float projectileForwardForce;
    public float projectileUpForce;

    [Header("Patroling")]
    public Vector3 walkPoint;
    public float walkPointRange;
    private bool walkPointSet;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    bool alreadyAttacking;

    [Header("States")]
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange;
    public bool playerInAttackRange;
    public bool isDead;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isDead) return;

        //Checking for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);             //future things to work on, Offset Sight to somewhere infront of Enemy so Enemy doesnt have eyes behind head
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange && playerInAttackRange) AttackPlayer();
        else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (!playerInSightRange && !playerInAttackRange) Patrolling();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        SetAccelearation(accel - (accel * 0.15f));      //Returns Enemy back tot default walking speed
        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //When walk point is reached automatically find a new walk point
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    public void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        SetAccelearation(accel + (accel *0.15f));    //Enemy moves 15% faster when chasing player
        agent.SetDestination(player.position);
    }

    public bool attacking;
    private void AttackPlayer()
    {
        //Make enemy not move when attacking
        agent.SetDestination(transform.position);

        transform.LookAt(new Vector3(player.position.x , transform.position.y, player.position.z));

        if (!alreadyAttacking)
        {
            //Attack code
            //Rigidbody projectileRigidBody = Instantiate(projectile, transform.position + transform.forward *2f, Quaternion.identity).GetComponent<Rigidbody>();
            //projectileRigidBody.AddForce(transform.forward * projectileForwardForce + transform.up * projectileUpForce, ForceMode.Impulse);

            //Divider for pretty code

            alreadyAttacking = true;
            attacking = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacking = false;
        attacking = false;
    }


    public void TakeDamage(int damage)
    { 
        health -= damage;

        if (health <= 0)
        {
            if (!isDead)
            {
                killCounter.IncrementCount();
                spawnerManager.SubtractEnemy();
            }
            isDead = true;
            GetComponent<NavMeshAgent>().enabled = false;
            Invoke(nameof(DestroyEnemy), 5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
        Destroy(parentObject);
    }

    public void SetAccelearation(float accel)
    {
        agent.acceleration = accel;
    }

    public float GetAcceleration()
    {
        return agent.acceleration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;                                                        //future things to work on, Offset Sight to somewhere infront of Enemy so Enemy doesnt have eyes behind head
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
