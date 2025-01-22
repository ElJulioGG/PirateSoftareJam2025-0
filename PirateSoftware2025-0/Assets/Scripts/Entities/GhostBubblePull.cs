using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBubblePull : MonoBehaviour
{
    public float maxForce = 10000f; // Maximum pulling force
    public float forceIncreaseRate = 10f; // Rate at which force increases every 0.1 seconds
    public string playerTag = "Player"; // Tag assigned to the player object
    public float stopDistance = 0.2f; // Distance at which the player is considered "at the center"

    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody playerRb;
    private bool isPulling = false;
    private float currentForce = 0f; // The current force applied to the player
    private float forceTimer = 0f; // Timer for increasing the force

    void Start()
    {
        // Find the player by tag
        GameObject playerObject = GameObject.FindWithTag(playerTag);

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
            }
            else
            {
                // Snap the player to the center and stop motion
                player.position = transform.position; // Directly set the position
                playerRb.velocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
                isPulling = false; // Stop pulling once snapped
                
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPulling = true; // Start pulling when the player first enters
            playerRb.useGravity = false;
            GameManager.instance.playerInBubble = true;
        }
    }
}
