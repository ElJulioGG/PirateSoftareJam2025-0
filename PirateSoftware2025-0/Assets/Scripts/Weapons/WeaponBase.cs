using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] private bool disableArrowOnShoot;
    [SerializeField] private GameObject startArrow;
    // Know which is the active weapon
    [SerializeField] private GameObject ghostFireParticles;
    [SerializeField] private GameObject tempGhostParticles;
    public bool isActiveWeapon;
    [SerializeField] private GameObject trailRenderer;
    [SerializeField] private CameraShake camShake;
    [SerializeField] private float shootShakeAmp = 2;
    [SerializeField] private float shootShakeTime = 0.15f;

    [SerializeField] private float firstShotShakeMult = 1.5f;
    [SerializeField] private float maxShake = 3f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] CinemachineVirtualCamera vCam;

    [SerializeField] private UnityEvent shootEvent;

    public Rigidbody weaponRigidbody;
    public float recoilForce = 20f;
    public float firstShotMult = 1.5f;
    public Slider holdSlider;
    public float holdTime = 3f; // Time required to hold the mouse before shooting
    public float holdTimer = 0f; // Timer to track holding duration
    public bool firstShotFired = false; // Flag to check if the first shot has been fired

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
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f; //bullet lasts 3secs max in the air

    public GameObject muzzleEffect;

    // Add here the animations (later)
    private Animator animator;
    public event System.Action OnShoot;
    // Loading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    // Save the position of weapons in the player hands (for prefabs)
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

    private TrailRenderer trailRendererComponent;
    private float originalTrailWidth;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        bulletsLeft = magazineSize;
        animator = GetComponent<Animator>();

        trailRendererComponent = trailRenderer.GetComponent<TrailRenderer>();
        if (trailRendererComponent != null)
        {
            originalTrailWidth = trailRendererComponent.startWidth;
        }
    }

    public float offsetX = 0;
    public float offsetY = 0;
    public float offsetZ = 0;

    private void Start()
    {
        startArrow.SetActive(true);
        // isActiveWeapon = false;
        //playerCamera = GameObject.FindWithTag("Main").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.playerCanInput)
        {


            if (!GameManager.instance.levelStarted && !firstShotFired)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    holdTimer += Time.deltaTime;
                    holdSlider.value = holdTimer / holdTime; // Update slider value

                    // Calculate the percentage of hold time completed
                    float holdPercentage = Mathf.Clamp01(holdTimer / holdTime);

                    // Adjust shake intensity based on percentage
                    float currentShakeIntensity = maxShake * holdPercentage;
                    camShake.setShakeCam(currentShakeIntensity, 999f);

                    // Adjust zoom based on percentage
                    if (vCam != null)
                    {
                        CinemachineFramingTransposer framer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
                        if (framer != null)
                        {
                            float currentZoom = maxZoom * holdPercentage;
                            framer.m_TrackedObjectOffset.x = currentZoom; // Adjust zoom by changing the offset.x
                        }
                    }

                    if (holdTimer >= holdTime)
                    {
                        ghostFireParticles.SetActive(true);
                        Debug.Log("Ready to shoot after holding for 3 seconds.");
                    }
                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    ghostFireParticles.SetActive(false);
                    if (holdTimer >= holdTime)
                    {
                        // Stop camera shake if implemented
                        if (camShake != null) camShake.stopShake();

                        // Reset zoom if the hold was not long enough
                        if (vCam != null)
                        {
                            CinemachineFramingTransposer framer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
                            if (framer != null)
                            {
                                framer.m_TrackedObjectOffset.x = 0f; // Reset zoom to original position
                            }
                        }
                        firstShotFired = true;
                        if (disableArrowOnShoot)
                        {
                            startArrow.SetActive(false);
                        }
                        holdSlider.gameObject.SetActive(false);

                        // or another specific point on or near the weapon
                        GameObject newParticle = Instantiate(tempGhostParticles, this.transform, worldPositionStays: false);
                        newParticle.transform.SetParent(this.transform, false);
                        newParticle.transform.position = ghostFireParticles.transform.position;
                        Destroy(newParticle, 2f);
                        FireWeapon();

                        // Actions from both snippets
                        weaponRigidbody.useGravity = true;
                        GameManager.instance.levelStarted = true;
                    }
                    else
                    {
                        // Stop camera shake if implemented
                        if (camShake != null) camShake.stopShake();

                        // Reset zoom if the hold was not long enough
                        if (vCam != null)
                        {
                            CinemachineFramingTransposer framer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
                            if (framer != null)
                            {
                                framer.m_TrackedObjectOffset.x = 0f; // Reset zoom to original position
                            }
                        }
                    }

                    // Reset these regardless of whether the shot was fired
                    holdTimer = 0f;
                    holdSlider.value = 0f; // Reset slider
                }

                return; // Exit Update to prevent normal shooting logic
            }
            if (firstShotFired)
            {
                weaponRigidbody.useGravity = true;
                isActiveWeapon = true;
                firstShotFired = false;
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

                if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading)
                {
                    Reload();
                }

                // Reload automatically when magazine is empty
                if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
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
    }

    private void FixedUpdate()
    {
        // No changes here
    }

    // Will update as we advance
    private void FireWeapon()
    {

        trailRenderer.SetActive(false);
        shootEvent.Invoke();

        if (GameManager.instance.playerInBubble)
        {
            GameManager.instance.playerInBubble = false;
            weaponRigidbody.useGravity = true;
        }
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
            Vector3 appliedForce;
            if (firstShotFired)
            {
                camShake.setShakeCam(shootShakeAmp * firstShotShakeMult, shootShakeTime);
                appliedForce = transform.forward * recoilForce * firstShotMult; // Use the gun's forward direction for recoil
               
            }
            else
            {
                camShake.setShakeCam(shootShakeAmp, shootShakeTime);
                appliedForce = transform.forward * recoilForce;
            }
            weaponRigidbody.AddForce(appliedForce, ForceMode.Impulse);
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
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1) //we already shot once before this check
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
        OnShoot?.Invoke();
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

    void OnDrawGizmos()
    {
        if (bulletSpawn != null)
        {
            Gizmos.color = Color.red;
            Vector3 endPoint = bulletSpawn.position + CalculateDirectionAndSpread() * 50f; // Extend line for visibility
            Gizmos.DrawLine(bulletSpawn.position, endPoint);
        }
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        // Use the forward direction of the weapon
        Vector3 direction = -transform.forward;

        // Apply spread
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Add spread to the direction
        Vector3 spread = transform.right * x + transform.up * y;
        return (direction + spread).normalized;
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}