using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private GameObject spawnPos;
    //public WeaponBase weaponBase;
    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.levelStarted == false)
        {
            player.transform.position = spawnPos.transform.position;
            playerRb.useGravity = false;
        }
        else
        {
            playerRb.useGravity = true;
        }
    }
    private void FixedUpdate()
    {
        if (GameManager.instance.levelStarted == false)
        {
           // player.transform.position = transform.position;

        }
    }
}
