using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsWall;
    public GameObject bulletHole;

    [Header("Stats")]
    [Range(0f, 1f)] public float bounciness;
    public bool useGravity;
    public float bulletSize;

    [Header("Damage")]
    public int explosionDamage;
    public float explosionRadius;
    public float explosionForce;

    [Header("Lifetime")]
    public int maxCollisions;
    public float maxLifeTime;
    public bool explodeOnTouch = true;

    private bool exploded;
    private int collisions;
    private PhysicMaterial phyisc_mat;
    private bool createNewHole = true;

    private RaycastHit hit;

    private void Start()
    {
        exploded = false;
        Setup();
    }
    
    private void FixedUpdate()
    {
        //When to explode
        //When colliding more than allowed times to colide
        if (collisions > maxCollisions && !exploded) Explode();

        //Count down lifetime
        maxLifeTime -= Time.deltaTime;
        if (maxLifeTime <= 0 && !exploded) Explode();
    }

    private void Explode()
    {
        exploded = true;

        //Instantiate explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        //Check for enemies in explosion
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRadius, whatIsEnemies);

        for (int i = 0; i < enemies.Length; i++)
        {
            //Damage all enemies in range of explosion
            if (enemies[i].GetComponent<BasicEnemyAI>())
                enemies[i].GetComponent<BasicEnemyAI>().TakeDamage(explosionDamage);

            //Add Explosion force (if enemy has rigidbody
            if (enemies[i].GetComponent<Rigidbody>())
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);

            //Damage player in range of explosion
            if (enemies[i].GetComponent<HealthHandler>())
            {
                enemies[i].GetComponent<HealthHandler>().TakeDamagePlayer(explosionDamage);
                Debug.Log("Bullet hit player");
            }
                
        }


        //Create bullet hole if hit wall
        if (createNewHole && bulletHole != null && Physics.Raycast(transform.position,transform.forward, out hit, bulletSize + 0.2f, whatIsWall))
        {
            Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
            createNewHole = false;
        }

        //Destroy bullet after hitting something with little delay
        Invoke(nameof(SelfDestruct), 0.01f);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;

        //Explode if bullet hits an enemy and explodeOnTouch is activated
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch && !exploded) Explode();
        
    }

    private void Setup()
    {
        //Create new Physics material
        phyisc_mat = new PhysicMaterial();
        phyisc_mat.bounciness = bounciness;
        phyisc_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        phyisc_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = phyisc_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
