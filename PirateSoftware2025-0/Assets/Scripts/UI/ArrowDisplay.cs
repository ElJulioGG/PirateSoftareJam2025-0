using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening; // Add DOTween namespace

public class ArrowDisplay : MonoBehaviour
{

    private Transform cameraTransform; // Reference to the camera's transform

    void Start()
    {
        // Find the main camera (or assign a specific camera)
        cameraTransform = Camera.main.transform;

        if (cameraTransform == null)
        {
            Debug.LogWarning("No main camera found!");
        }

       
    }

    void Update()
    {
        
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

  

   
}