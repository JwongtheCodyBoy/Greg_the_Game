using UnityEngine;

public class TestShooting : MonoBehaviour
{
    [Header("Reference")]
    private Rigidbody rb;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [Header("Stats")]
    public float health;
    public float attackRange;

    [Header("Projectile")]
    public GameObject projectile;
    public float projectileForwardForce;
    public float projectileUpForce;


    [Header("Attacking")]
    public float timeBetweenAttacks;
    bool alreadyAttacking;

    [Header("States")]
    public bool idle;
    public bool attack;
    private bool isDead;
    /*
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    */

    private void Update()
    {
        if (isDead) return;

        //Draw Raycast infront of dummy to see if player is infront, if so attack player
        if (Physics.Raycast(transform.position, transform.forward, attackRange, whatIsPlayer)) AttackPlayer();
    }

    private void AttackPlayer()
    {
        //Make enemy not move when attacking

        if (!alreadyAttacking)
        {
            //Attack code
            Rigidbody projectileRigidBody = Instantiate(projectile, transform.position + transform.forward * 2f, transform.rotation).GetComponent<Rigidbody>();
            projectileRigidBody.AddForce(transform.forward * projectileForwardForce + transform.up * projectileUpForce, ForceMode.Impulse);

            //Divider for pretty code

            alreadyAttacking = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacking = false;
    }


    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            Invoke(nameof(DestroyEnemy), 5f);
            if (isDead)
                rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}