using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // Add DOTween namespace

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private GameObject followObject;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI ammoText2;
    [SerializeField] private WeaponBase weapon;
    [SerializeField] private float shakeStrength = 0.1f;
    [SerializeField] private float scaleMult = 1.5f;

    private Transform cameraTransform; // Reference to the camera's transform

    void Start()
    {
        ammoText.gameObject.SetActive(false);
        ammoText2.gameObject.SetActive(false);
        // Find the main camera (or assign a specific camera)
        cameraTransform = Camera.main.transform;

        if (cameraTransform == null)
        {
            Debug.LogWarning("No main camera found!");
        }

        // Subscribe to the OnShoot event
        if (weapon != null)
        {
            weapon.OnShoot += HandleShoot;
        }
    }

    void Update()
    {
        // Update position and ammo text
        gameObject.transform.position = followObject.transform.position;
        ammoText.text = $"{weapon.bulletsLeft}/{weapon.magazineSize}";
        ammoText2.text = $"{weapon.bulletsLeft}/{weapon.magazineSize}";
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // Make the Canvas face the camera
            FaceCamera();
        }
    }

    private void FaceCamera()
    {
        // Calculate the direction from the Canvas to the camera
        Vector3 directionToCamera = cameraTransform.position - transform.position;

        // Invert the direction to make the Canvas face the camera correctly
        directionToCamera = -directionToCamera;

        // Rotate the Canvas to face the camera
        transform.rotation = Quaternion.LookRotation(directionToCamera);

        // Optional: Lock the Canvas's rotation around the X and Z axes
        // This ensures the Canvas doesn't tilt or roll
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.x = 0; // Lock X rotation
        eulerAngles.z = 0; // Lock Z rotation
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    private void HandleShoot()
    {
        ammoText.gameObject.SetActive(true);
        ammoText2.gameObject.SetActive(true);
        // Scale up the text by 0.2 and shake it
        ammoText.transform.DOScale(ammoText.transform.localScale * scaleMult, 0.2f)
            .OnComplete(() =>
            {
                // Return to the original scale
                ammoText.transform.DOScale(Vector3.one, 0.2f);
            });

        // Shake the text
        ammoText.transform.DOShakePosition(0.2f, new Vector3(shakeStrength, shakeStrength, shakeStrength), 10, 90, false, true);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        if (weapon != null)
        {
            weapon.OnShoot -= HandleShoot;
        }
    }
}