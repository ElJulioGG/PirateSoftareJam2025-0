using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class LevelStartManager : MonoBehaviour
{
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private GameObject spawnPos;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private float introWaitTime = 0.5f;
    [SerializeField] private float introDropTime = 0.2f;
    [SerializeField] private float cameraTransitionTime = 0.5f;

    [SerializeField] private Animator canvasAnimator; // Reference to the player's Animator component

    void Start()
    {
        slider.SetActive(false);
        GameManager.instance.playerCanInput = false;
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

        // Start the coroutine
        StartCoroutine(StartLevelSequence());
    }

    void Update()
    {
        // Restart the scene when the R key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(RestartLevelSequence());
        }
    }

    void RestartScene()
    {
        // Get the currently active scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void NextScene()
    {
        // Get the currently active scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.playerCanInput == false)
        {
            player.transform.position = spawnPos.transform.position;
        }
    }

    // Coroutine to handle the level start sequence
    private IEnumerator StartLevelSequence()
    {
        yield return new WaitForSeconds(introWaitTime);

        // Randomly select an animation
        int randomAnimation = Random.Range(1, 5); // Generates a random number between 1 and 4
        string animationName = "Transition" + randomAnimation + "_Out";
        canvasAnimator.Play(animationName);

        playerRb.useGravity = true;

        yield return new WaitForSeconds(introDropTime);
        // Allow player input
        
        GameManager.instance.playerCanInput = true;
        yield return new WaitForSeconds(cameraTransitionTime);
        slider.SetActive(true);
        // Allow player input
        cameraManager.cameraMode = 0;
    }

    public IEnumerator RestartLevelSequence()
    {
        slider.SetActive(false);
        // Randomly select an animation
        int randomAnimation = Random.Range(1, 5); // Generates a random number between 1 and 4
        string animationName = "Transition" + randomAnimation + "_In";
        canvasAnimator.Play(animationName);

        yield return new WaitForSeconds(introWaitTime+0.5f);

        RestartScene();
    }
    public IEnumerator NextLevelSequence()
    {
        slider.SetActive(false);
        // Randomly select an animation
        int randomAnimation = Random.Range(1, 5); // Generates a random number between 1 and 4
        string animationName = "Transition" + randomAnimation + "_In";
        canvasAnimator.Play(animationName);

        yield return new WaitForSeconds(introWaitTime + 0.5f);

        RestartScene();
    }
}