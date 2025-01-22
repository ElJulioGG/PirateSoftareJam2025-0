using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation
    public float rollSpeed = 50f; // Speed for rolling sideways
    public float rollMultiplier = 1.5f; // Multiplier to make rolling more dynamic
    private Rigidbody rb; // Optional Rigidbody for physics-based rolling

    void Start()
    {
        // Reference Rigidbody if physics is used
        rb = GetComponent<Rigidbody>();

        // Reset transform rotation to default (identity)
        transform.rotation = Quaternion.identity;

        // Reset Rigidbody's angular velocity if it exists
        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {
        RotateWeapon();
    }

    void RotateWeapon()
    {
        // Initialize rotation direction
        Vector3 rotationDirection = Vector3.zero;

        // Check for WASD inputs and set rotation direction
        if (Input.GetKey(KeyCode.W))
        {
            rotationDirection.x += 1; // Rotate forward
        }
        if (Input.GetKey(KeyCode.S))
        {
            rotationDirection.x -= 1; // Rotate backward
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotationDirection.y -= 1; // Rotate left
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotationDirection.y += 1; // Rotate right
        }

        // Add roll effect when moving diagonally
        if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)))
        {
            rotationDirection.z -= rollMultiplier; // Roll left
        }
        else if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)))
        {
            rotationDirection.z += rollMultiplier; // Roll right
        }

        // Add sideways rotation with E and Q keys
        if (Input.GetKey(KeyCode.E))
        {
            rotationDirection.z += 1; // Roll clockwise
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rotationDirection.z -= 1; // Roll counterclockwise
        }

        // Normalize rotation direction to ensure consistent speed (diagonal movement isn't faster)
        if (rotationDirection != Vector3.zero)
        {
            rotationDirection.Normalize(); // Normalize the vector

            // Apply rotation to the transform for visual rotation
            transform.Rotate(rotationDirection * rotationSpeed * Time.deltaTime, Space.Self);

            // Apply torque to the Rigidbody for physics-based rotation
            if (rb != null)
            {
                Vector3 torque = new Vector3(rotationDirection.x, rotationDirection.y, rotationDirection.z);
                rb.AddTorque(torque * rotationSpeed * Time.deltaTime, ForceMode.Acceleration);
            }
        }
    }
}
