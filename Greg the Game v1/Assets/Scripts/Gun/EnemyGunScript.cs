using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunScript : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    private Transform enemyPos;
    private BasicEnemyAI aiScript;
    private Gun gunScript;
    private PickupItem itemScript;
    public Rigidbody rb;
    public BoxCollider coll;

    [Header("Gun Stats")]
    public GameObject enemyBullet;
    public float timeBetweenShooting;
    private float timeBetweenShots;
    private float spread;
    private float bulletForwardForce;
    private float bulletUpForce;
    //public float bulletRange;
    private float reloadTime;
    private float recoilForce;

    private int magazineSize;
    private int bulletsPerTap;
    private bool allowButtonHold;
    //public bool infiniteRange;
    //public bool isBurstFire;
    private bool isShotgun;

    //private float range;
    private int bulletsLeft;
    private int bulletsShot;

    [Header("Bools")]
    private bool readyToShoot, reloading, allowInvoke, patrolling, attacking; //shooting
    public bool dropWeapon;
    public static int maxNumGuns;
    public static int currentNumGuns = 0;

    [Header("Graphics")]
    private ParticleSystem muzzleFlash;
    //private GameObject bulletHole;

    private Vector3 originalPos;
    private float currentReloadTime;

    private void Awake()
    {
        //Getting Scripts
        gunScript = GetComponent<Gun>();
        itemScript = GetComponent<PickupItem>();
        rb = GetComponent<Rigidbody>(); 
        coll = GetComponent<BoxCollider>();
        enemyPos = this.transform.parent.parent.Find("bad Guy").transform;
        aiScript = this.transform.parent.parent.GetComponentInChildren<BasicEnemyAI>();
        /*
        try
        {
            enemyPos = this.transform.parent.parent.Find("bad Guy").transform;
            aiScript = this.transform.parent.parent.GetComponentInChildren<BasicEnemyAI>();
        }
        catch{
            Debug.Log("This Gun is on the Ground");
        }
        */

        //Copying Gun Stats from gunScript
        if (timeBetweenShooting == 0 ) timeBetweenShooting = gunScript.timeBetweenShooting;
        timeBetweenShots = gunScript.timeBetweenShots;
        spread = gunScript.spread;
        bulletForwardForce = gunScript.bulletForwardForce;
        bulletUpForce = gunScript.bulletUpForce;
        reloadTime = gunScript.reloadTime;

        magazineSize = gunScript.magazineSize;
        bulletsPerTap = gunScript.bulletsPerTap;
        allowButtonHold = gunScript.allowButtonHold;
        isShotgun = gunScript.isShotgun;

        bulletsLeft = magazineSize;
        readyToShoot = true;
        allowInvoke = true;

        originalPos = transform.position;

        muzzleFlash = gunScript.muzzleFlash;
        //bulletHole = gunScript.bulletHole;

        //Disabling gunScript and itemScript
        if (!itemScript.equipped)
        {
            gunScript.enabled = false;
            itemScript.enabled = false;
        }
        //Disables this script if the gun belongs to the player at start
        /*
        try
        {
            if (this.transform.parent.parent.GetComponent<PlayerCam>() != null) this.enabled = false;
        }
        catch
        {
            Debug.Log("This Gun is on the player");
        }
        */
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //Stops update if the gun is on the ground
        if (this.transform.parent == null) return;

        //checks if enemy is patrolling
        patrolling = (!aiScript.playerInSightRange && !aiScript.playerInAttackRange);

        //If patrolling or run out of bullets reload
        if (patrolling && bulletsLeft < magazineSize && !reloading) Reload();
        if (readyToShoot && aiScript.attacking && !reloading && bulletsLeft <= 0) Reload();

        //if attacking and can shoot shoot gun
        if (aiScript.attacking && readyToShoot && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }

        //Reload animation (spin gun)
        if (reloading)
        {
            currentReloadTime += Time.deltaTime;
            float spinDelta = (Mathf.Cos(Mathf.PI * (currentReloadTime / reloadTime)) - 1f) / 2f;
            transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, spinDelta * 360f));
        }

        //If dead and can drop weapon drop weapon
        if (aiScript.isDead && dropWeapon && (currentNumGuns < maxNumGuns)) Drop();


        //Disable this Script
        if (this.transform.parent == null)
            this.enabled = false;
    }

    private void Shoot()
    {
        if (bulletsLeft <= 0) return;

        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        
        Ray ray = new Ray(enemyPos.position, -transform.right); //Creates a Ray at enemy in direction of gun
        RaycastHit hit;

        Vector3 targetPoint;
        if (isShotgun)
            targetPoint = ray.GetPoint(9);
        else if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(70);    //Just get a point far from the player if ray does not hit anything


        //Calculate Direction with no Spread
        Vector3 directionNoSpread = targetPoint - attackPoint.position;

        //Calculate Direction with Spread
        Vector3 direction = directionNoSpread + new Vector3(x, y, 0);

        //Creating Bullet
        GameObject currentBullet = Instantiate(enemyBullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = direction.normalized;                 //Rotate bullet to shoot direction

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * bulletForwardForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(transform.up * bulletUpForce, ForceMode.Impulse);


        //Graphics
        //Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
        muzzleFlash.Play();

        bulletsLeft--;
        bulletsShot--;


        if (allowInvoke)
        {
            allowInvoke = false;
            Invoke(nameof(ResetShoot), timeBetweenShooting);
        }

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke(nameof(Shoot), timeBetweenShots);
    }

    private void ResetShoot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        currentReloadTime = 0f;
        Invoke(nameof(ReloadFinish), reloadTime);
    }

    private void ReloadFinish()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }

    void Drop() 
    {
        currentNumGuns++;
        //Set parent to none
        transform.SetParent(null);

        //Make RigidBody not Kinematic and box Colider not a trigger
        rb.useGravity = true;
        rb.isKinematic = false;
        coll.isTrigger = false;

        //AddForce
        rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
        rb.AddForce(transform.up * 2f, ForceMode.Impulse);
        //Add randomRotation
        float random = Random.Range(-1, 1);
        rb.AddTorque(new Vector3(random, random, random));

        itemScript.enabled = true;
        /*
        Transform temp;
        temp = itemScript.player;
        itemScript.player = this.transform;

        itemScript.Drop();
        itemScript.setFull();
        itemScript.player = temp;
        */
    }

    void Setup()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        coll.isTrigger = true;

        transform.position = originalPos;
    }

    public void SetMaxNumGuns(int max)
    {
        maxNumGuns = max;
    }
}
