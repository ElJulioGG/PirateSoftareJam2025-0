using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] private CinemachineVirtualCamera staticVirtualCam;
    [SerializeField] CinemachineVirtualCamera virtualCam;
    [SerializeField] float cameraMode;
    private GameObject goal;

    void Start()
    {
        // Search for the object named "Goal" at the start of the game
        goal = GameObject.Find("Goal");

        if (goal == null)
        {
            Debug.LogError("Goal object not found in the scene!");
        }
    }

    void Update()
    {
        switch (cameraMode)
        {
            case 0:
                virtualCam.Follow = player.transform;
                virtualCam.LookAt = goal.transform;
                virtualCam.Priority = 10;
                staticVirtualCam.Priority = 0;
                break;
            case 1:
               // staticVirtualCam.LookAt = player.transform;
              //  staticVirtualCam.Follow = goal.transform;

                virtualCam.Priority = 0;
                staticVirtualCam.Priority = 10;
                break;

        }
    }
}