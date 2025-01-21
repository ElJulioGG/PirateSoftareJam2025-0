using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBase : MonoBehaviour
{
    // Know which is the active weapon
    public bool isActiveWeapon;

    public Rigidbody weaponRigidbody;
    public float recoilMult = 20f;
    public Slider holdSlider;
    public float holdTime = 3f; // Time required to hold the mouse before shooting
    public float holdTimer = 0f; // Timer to track holding duration
    public bool firstShotFired = false; // Flag to check if the first shot has been fired

    //public Camera playerCamera; // Cambiar, no funciona en prefab

    // Shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // Burst mode
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Spread/bloom
    public float spreadIntensity;

    // Bullet
    //public GameObject playerCamera;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f; //bullet lasts 3secs max in the air

    public GameObject muzzleEffect;

    // Add here the animations (later)
    private Animator animator;

    // Loading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    // Save the pposition of weapons in the player hands (for prefabs)
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    //Damage
    public float damageAmount = 25f;

    // Weapon detector (for sound and other stuff)
    public enum WeaponModel
    {
        M1911Pistol,
        M48Rifle,
        BENNELLIM4Shotgun
    }

    public WeaponModel thisWeaponModel;


    // UI HUD
    public TMPro.TextMeshProUGUI ammoDisplay; //Use TMPro. before the use of TMPGUI


    public enum ShootingMode //shooting modes
    {
        Single, 
        Burst, 
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;

        bulletsLeft = magazineSize;

        animator = GetComponent<Animator>();
    }


    public float offsetX = 0;
    public float offsetY = 0;
    public float offsetZ = 0;

    private void Start()
    {
        //playerCamera = GameObject.FindWithTag("Main").GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.levelStarted && !firstShotFired)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                holdTimer += Time.deltaTime;
                holdSlider.value = holdTimer / holdTime; // Update slider value
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && holdTimer >= holdTime)
            {
                FireWeapon();
                firstShotFired = true;
                holdTimer = 0f;
                holdSlider.value = 0f; // Reset slider
                GameManager.instance.levelStarted = true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && holdTimer < holdTime)
            {
                holdTimer = 0f;
                holdSlider.value = 0f; // Reset slider
            }

            return;
        }
        // Ensure the level has not started
        if (!GameManager.instance.levelStarted && !firstShotFired)
        {
            // Check if the mouse button is held down
            if (Input.GetKey(KeyCode.Mouse0))
            {
                holdTimer += Time.deltaTime; // Increment the hold timer
                if (holdTimer >= holdTime)
                {
                    Debug.Log("Ready to shoot after holding for 3 seconds.");
                }
            }

            // Check if the mouse button is released after 3 seconds
            if (Input.GetKeyUp(KeyCode.Mouse0) && holdTimer >= holdTime)
            {
                FireWeapon(); // Fire the weapon
                firstShotFired = true; // Mark the first shot as fired
                holdTimer = 0f; // Reset the hold timer
            }

            // Reset the hold timer if the button is released early
            if (Input.GetKeyUp(KeyCode.Mouse0) && holdTimer < holdTime)
            {
                holdTimer = 0f;
            }

            return; // Exit Update to prevent normal shooting logic
        }

        if (firstShotFired)
        {
            isActiveWeapon = true;
        }
        // Normal shooting logic once the first shot is fired or the level has started
        if (isActiveWeapon)
        {
            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.emptyMagazineSound1911.Play();
            }

            // SINGLE SHOT
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (bulletsLeft > 0)
                {
                    FireWeapon();
                }
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false)
            {
                Reload();
            }

            // Reload automatically when magazine is empty
            if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
            {
                Reload();
            }

            // AUTO SHOT
            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if (AmmoManager.Instance.ammoDisplay != null)
            {
                AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
            }
        }
    }

    private void FixedUpdate() // No cambia NADA 
    {
        //Vector3 newPos = new Vector3(playerCamera.transform.position.x + offsetX, playerCamera.transform.position.y + offsetY, playerCamera.transform.position.z+ offsetZ);

        //transform.position = playerCamera.transform.position;
       // transform.rotation = playerCamera.transform.rotation;
        
    }
    // Will update as we advance
    private void FireWeapon()
    {
        // Cancel all momentum before shooting
        if (weaponRigidbody != null)
        {
            weaponRigidbody.velocity = Vector3.zero;
            weaponRigidbody.angularVelocity = Vector3.zero;
        }

        bulletsLeft--;
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL"); // Always use the same name "RECOIL" as the parameter for the trigger

        readyToShoot = false; // Prevent issues with multiple shots
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // Pointing the bullet to face the shooting direction
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // Apply recoil force in the opposite direction of the gun's forward direction
        if (weaponRigidbody != null)
        {
            Vector3 recoilForce = transform.forward * recoilMult; // Use the gun's forward direction for recoil
            weaponRigidbody.AddForce(recoilForce, ForceMode.Impulse);
        }

        // Destroy bullet after some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // Checking if we are done shooting
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Check Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) //we already shoot once before this check
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }



    private void Reload()
    {
        isReloading = true;
        SoundManager.Instance.reloadingSound1.Play();

        animator.SetTrigger("RELOAD");

        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {

        bulletsLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // Shooting from the middle of the screen to check where are we poiting at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Cambiar, no funciona en prefab
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // Hitting Something
            targetPoint = hit.point;
        }
        else
        {
            // Shooting at the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Returning the shooting direction and spread 
        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
