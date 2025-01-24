using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class LevelStartManager : MonoBehaviour
{
    //[SerializeField] CameraShake camShake;
    //[SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private GameObject spawnPos;
    //public WeaponBase weaponBase;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.levelStarted = false;
        player = GameObject.FindGameObjectWithTag("Player");

        // Check if the player GameObject was found
        if (player != null)
        {
            Debug.Log("Player GameObject found!");
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'Player' found.");
        }


        if (GameManager.instance.levelStarted == false)
        {
            player.transform.position = spawnPos.transform.position;
            playerRb.useGravity = false;
        }
    }

    // Update is called once per frame
 

    void Update()
    {

        // Restart the scene when the R key is pressed
        if (Input.GetKeyDown(KeyCode.R)) // Corrected method for checking key press
        {
            RestartScene();
        }
    }

    void RestartScene()
    {
        // Get the currently active scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

private void FixedUpdate()
    {
        if (GameManager.instance.levelStarted == false)
        {
           // player.transform.position = transform.position;

        }

        if (GameManager.instance.levelStarted == false)
        {
            player.transform.position = spawnPos.transform.position;
           
        }
        else
        {
            //playerRb.useGravity = true;
        }
    }
}
