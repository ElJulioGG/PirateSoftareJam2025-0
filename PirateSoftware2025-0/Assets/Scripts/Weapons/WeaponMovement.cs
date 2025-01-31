using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    [SerializeField] private WeaponBase Weapon;
    public float rotationSpeed = 250f;
    public float rollSpeed = 50f;
    public float rollMultiplier = 1.5f;
    private Rigidbody rb;

    public AudioSource rotationSound; // Assign in Inspector

    private bool isRotating = false; // Track rotation state

    void Start()
    {
        GameObject weaponBaseObject = GameObject.FindWithTag("Gun");
        if (weaponBaseObject != null)
        {
            Weapon = weaponBaseObject.GetComponent<WeaponBase>();
        }
        rb = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero;
            rb.maxAngularVelocity = 50;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.playerCanInput)
        {
            RotateWeapon();
        }
    }

    void RotateWeapon()
    {
        Vector3 rotationDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) rotationDirection.x += 1;
        if (Input.GetKey(KeyCode.S)) rotationDirection.x -= 1;
        if (Input.GetKey(KeyCode.A)) rotationDirection.y -= 1;
        if (Input.GetKey(KeyCode.D)) rotationDirection.y += 1;

        if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)))
            rotationDirection.z -= rollMultiplier;
        else if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)))
            rotationDirection.z += rollMultiplier;

        if (Input.GetKey(KeyCode.E)) rotationDirection.z += 1;
        if (Input.GetKey(KeyCode.Q)) rotationDirection.z -= 1;

        if (rotationDirection.sqrMagnitude > 0.0001f)
        {
            rotationDirection.Normalize();
            float finalRotationSpeed = Input.GetKey(KeyCode.LeftShift) ? rotationSpeed / 3f : rotationSpeed;
            transform.Rotate(rotationDirection * finalRotationSpeed * Time.deltaTime, Space.Self);

            if (rb != null)
            {
                Vector3 torque = new Vector3(rotationDirection.x, rotationDirection.y, rotationDirection.z);
                rb.AddTorque(torque * finalRotationSpeed * Time.deltaTime, ForceMode.Acceleration);
            }

            // Play sound if not already playing
            if (!isRotating)
            {
                isRotating = true;
                if (rotationSound && !rotationSound.isPlaying)
                {
                    if (Weapon.insideBubble)
                    {
                        rotationSound.Play();
                    }
                    
                }
            }
        }
        else
        {
            if (rb != null) rb.angularVelocity = Vector3.zero;

            // Stop sound when not rotating
            if (isRotating)
            {
                isRotating = false;
                if (rotationSound)
                {
                    rotationSound.Stop();
                }
            }
        }
    }
}
