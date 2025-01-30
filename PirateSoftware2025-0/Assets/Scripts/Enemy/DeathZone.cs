using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private LevelStartManager levelStartManager;
    [SerializeField] private bool levelGoal = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!levelGoal)
            {
                StartCoroutine(levelStartManager.RestartLevelSequence());
            }
            else
            {
                StartCoroutine(levelStartManager.NextLevelSequence());
            }
            
        }
    }
}
