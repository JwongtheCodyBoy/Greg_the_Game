using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    public GameObject bullet;
    public float timeBetweenShooting;
    public float timeBetweenShots;
    public float spread;
    public float bulletForwardForce;
    public float bulletUpForce;
    //public float bulletRange;
    public float reloadTime;
    public int maxReloadAmount;
    public float recoilForce;

    public int magazineSize;
    public int bulletsPerTap;
    public bool allowButtonHold;
    public bool infiniteRange;
    public bool isBurstFire;
    public bool isShotgun;

    //private float range;
    private int bulletsLeft;
    private int bulletsShot;
    private int currentReloadAmount = 0;

    [Header("Bools")]
    private bool shooting, readyToShoot, reloading, allowInvoke;

    [Header("References")]
    public Camera cam;
    public Transform attackPoint;
    public LayerMask whatIsEnemy;
    public Rigidbody playerRb;
    public PlayerMovement pm;
    private PickupItem pickUpScript;

    [Header("Graphics")]
    public ParticleSystem muzzleFlash;
    //public GameObject bulletHole;

    private Vector3 originalPos;
    private float currentReloadTime;

    public CameraShake camShake;
    public float camShakeMagnitude;
    public float camShakeDuration;

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI reloadText;

    [Header("Inputs")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;

    private void Start()
    {
        //if (infiniteRange) range = Mathf.Infinity;
        //else range = bulletRange;

        bulletsLeft = magazineSize;
        readyToShoot = true;
        allowInvoke = true;

        originalPos = transform.position;
        pickUpScript = GetComponent<PickupItem>();
    }

    private void Update()
    {
        //if Player is Dead cannot shoot
        if (pm.isDead) return;

        MyInput();

        //Set Text
        if (isBurstFire) ammoText.SetText(bulletsLeft + "/" + magazineSize);
        else ammoText.SetText(bulletsLeft / bulletsPerTap + "/" + magazineSize / bulletsPerTap);
        reloadText.SetText((maxReloadAmount - currentReloadAmount) + "/" + (maxReloadAmount));

        //reloading
        if(reloading)
        {
            currentReloadTime += Time.deltaTime;
            float spinDelta = (Mathf.Cos(Mathf.PI * (currentReloadTime / reloadTime)) - 1f) / 2f;
            transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, spinDelta * 360f));
        }

        //if (Input.GetKeyUp(KeyCode.Keypad0)) DropToDestroy();
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(shootKey);
        else shooting = Input.GetKeyDown(shootKey);

        if (Input.GetKeyDown(reloadKey) && bulletsLeft < magazineSize && !reloading) Reload();
        //Auto Reload if run out of bullet and trying to shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletsLeft <= 0) return; 

        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));   //Creates a Ray at the middle of screen
        RaycastHit hit;

        Vector3 targetPoint;
        if (isShotgun)
            targetPoint = ray.GetPoint(9);
        else if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(100);    //Just get a point far from the player if ray does not hit anything


        //Calculate Direction with no Spread
        Vector3 directionNoSpread = targetPoint - attackPoint.position;

        //Calculate Direction with Spread
        Vector3 direction = directionNoSpread + new Vector3(x, y, 0);

        //Creating Bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = direction.normalized;                 //Rotate bullet to shoot direction

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * bulletForwardForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * bulletUpForce, ForceMode.Impulse);



        //Shake Camera
        StartCoroutine(camShake.Shake(camShakeDuration, camShakeMagnitude));

        //Graphics
        //Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
        muzzleFlash.Play();

        bulletsLeft--;
        bulletsShot--;


        if (allowInvoke)
        {
            Invoke(nameof(ResetShoot), timeBetweenShooting);
            allowInvoke = false;

            //Add recoil
            playerRb.AddForce(-direction.normalized * recoilForce, ForceMode.Impulse);
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
        if (currentReloadAmount > maxReloadAmount-1)
        {
            DropToDestroy();
            return; 
        }

        reloading = true;
        currentReloadTime = 0f;
        Invoke(nameof(ReloadFinish), reloadTime);
    }

    private void ReloadFinish()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        currentReloadAmount++;
    }

    private void DropToDestroy()
    {
        pickUpScript.Drop();
        pickUpScript.enabled = false;
        Invoke(nameof(DelayResetAmmo), 0.05f);
        EnemyGunScript.currentNumGuns--;
        Invoke(nameof(SelfDestruct), 1.7f);
    }

    private void DelayResetAmmo()
    {
        ammoText.SetText("");
        reloadText.SetText("");
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
