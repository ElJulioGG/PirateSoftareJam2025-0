using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostBubblePull : MonoBehaviour
{
    public float maxForce = 10000f; // Maximum pulling force
    public float forceIncreaseRate = 10f; // Rate at which force increases every 0.1 seconds
    public string playerTag = "Player";
    public string ammoTag = "AmmoDisplay";
    public string gunTag = "Gun";// Tag assigned to the player object
    public string playerTrailTag = "PlayerTrail"; // Tag for the player trail
    public float stopDistance = 0.2f; // Distance at which the player is considered "at the center"

    [SerializeField] private Transform player;
    [SerializeField] private WeaponBase playerWeaponBase;
    [SerializeField] private AmmoDisplay ammoDisplay;
    [SerializeField] private Rigidbody playerRb;
    private bool isPulling = false;
    private float currentForce = 0f; // The current force applied to the player
    private float forceTimer = 0f; // Timer for increasing the force
    private Vector3 appliedTorque; // Store the applied torque

    // Renderer for changing color
    [SerializeField] private Renderer outerBubbleRenderer;
    [SerializeField] private GameObject outerBubble;
    [SerializeField] private float scaleUpTime = 0.25f;
    [SerializeField] private float scaleUpMult = 1.5f;

    // New object to shrink
    [SerializeField] private GameObject shrinkObject;
    [SerializeField] private float shrinkTime = 0.25f;
    [SerializeField] private float shrinkMult = 0.6667f; // 1 / 1.5 to shrink by 1.5 times

    // Reference to the CameraShake component
    private CameraShake camShake;

    void Start()
    {
        // Find the player by tag
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        GameObject weaponBaseObject = GameObject.FindWithTag(gunTag);
        GameObject ammoDisplayObject = GameObject.FindWithTag(ammoTag);

        if (weaponBaseObject != null)
        {
            playerWeaponBase = weaponBaseObject.GetComponent<WeaponBase>();
        }
        else
        {
            Debug.LogError($"No GameObject found with tag '{playerTag}'!");
        }
        if (ammoDisplayObject != null)
        {
            ammoDisplay = ammoDisplayObject.GetComponent<AmmoDisplay>();
        }
        else
        {
            Debug.LogError($"No GameObject found with tag '{playerTag}'!");
        }

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = playerObject.GetComponent<Rigidbody>();

            if (playerRb == null)
            {
                Debug.LogError("Player does not have a Rigidbody component!");
            }
        }
        else
        {
            Debug.LogError($"No GameObject found with tag '{playerTag}'!");
        }

        // Get the renderer component for color changes
        outerBubbleRenderer = GetComponent<Renderer>();
        if (outerBubbleRenderer == null)
        {
            Debug.LogError("No Renderer component found on this object!");
        }

        // Find the CameraShake script in the scene
        camShake = FindObjectOfType<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("No CameraShake component found in the scene!");
        }
    }

    void FixedUpdate()
    {
        if (isPulling && playerRb != null)
        {
            Vector3 directionToCenter = (transform.position - player.position);
            float distance = directionToCenter.magnitude;

            if (distance > stopDistance)
            {
                // Increase force every 0.1 seconds
                forceTimer += Time.fixedDeltaTime;
                if (forceTimer >= 0.1f)
                {
                    forceTimer = 0f;
                    currentForce += forceIncreaseRate; // Increase the force applied
                    currentForce = Mathf.Min(currentForce, maxForce); // Cap the force at maxForce
                }

                // Apply the increased force towards the center
                Vector3 force = directionToCenter.normalized * currentForce;
                playerRb.AddForce(force, ForceMode.Acceleration);

                // Apply torque (this was added in OnTriggerEnter)
                playerRb.AddTorque(appliedTorque);
            }
            else
            {
                AudioManager.instance.PlayerSteps("gunBubble");
                playerWeaponBase.insideBubble = true;
                playerWeaponBase.bulletsLeft = playerWeaponBase.bulletsLeft + 1;
                ammoDisplay.HandleShoot();
                camShake.shakeCam();
                // Shrink the new object
                if (shrinkObject != null)
                {
                    Vector3 originalShrinkScale = shrinkObject.transform.localScale;
                    shrinkObject.transform.DOScale(originalShrinkScale * shrinkMult, shrinkTime)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            shrinkObject.transform.DOScale(originalShrinkScale, shrinkTime).SetEase(Ease.InQuad);
                        });
                }
                // Snap the player to the center and stop motion
                player.position = transform.position; // Directly set the position
                playerRb.velocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero; // Stop rotation
                appliedTorque = Vector3.zero; // Reset the torque
                isPulling = false; // Stop pulling once snapped
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            AudioManager.instance.PlayerSteps("playerSplash1");
            if (!isPulling)
            {
                Vector3 originalScale = outerBubble.transform.localScale;

                outerBubble.transform.DOScale(originalScale * scaleUpMult, scaleUpTime)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        outerBubble.transform.DOScale(originalScale, scaleUpTime).SetEase(Ease.InQuad);
                    });

                if (outerBubbleRenderer != null)
                {
                    Color originalColor = outerBubbleRenderer.material.color;
                    outerBubbleRenderer.material.DOColor(Color.green, scaleUpTime)
                        .OnComplete(() =>
                        {
                            outerBubbleRenderer.material.DOColor(originalColor, scaleUpTime);
                        });
                }

                // Enable Player Trail
                GameObject playerTrail = FindDescendantWithTag(player, playerTrailTag);

                if (playerTrail != null)
                {
                    playerTrail.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("Player Trail not found!");
                }

                // Apply random torque
                appliedTorque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * 10f; // Example torque strength

                // Use camera shake if available
                if (camShake != null)
                {
                    camShake.shakeCam();
                }

                
            }
            isPulling = true; // Start pulling when the player first enters
            playerRb.useGravity = false;
            GameManager.instance.playerInBubble = true;
        }
    }

    // Helper method to find a descendant by tag
    private GameObject FindDescendantWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
            else
            {
                GameObject result = FindDescendantWithTag(child, tag);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return null;
    }
}