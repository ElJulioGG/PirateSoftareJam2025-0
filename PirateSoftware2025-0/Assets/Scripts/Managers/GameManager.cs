using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //[Header("Player Variables")]
    //[SerializeField] public int lastSelectedNote = 0;
    //[SerializeField] public float securityLevel = 0f;
    //[Header("Player Stats")]
    //[SerializeField] public int playerHealth = 3;
    //[Header("Player Status")]
    //[SerializeField] public bool playerCanDialog = true;
    //[SerializeField] public bool playerIsInDialog = true;
    //[SerializeField] public bool playerCanMove = true;
    //[SerializeField] public bool playerIsHit = false;
    //[SerializeField] public bool playerDied = false;
    //[SerializeField] public bool playerCanAction = true;
    //[SerializeField] public bool playerInvincibility = false;
    [SerializeField] public bool levelStarted = false;


    private void Awake()
    {
        // AudioManager.instance.PlayMusic("StealthTheme");//Only for now 

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void setPlayerCanMove(bool newState)
    {
        //playerCanMove = newState;
    }
    //private void Start()
    //{
    //    EventManager.OnTimerStart();
    //}
}