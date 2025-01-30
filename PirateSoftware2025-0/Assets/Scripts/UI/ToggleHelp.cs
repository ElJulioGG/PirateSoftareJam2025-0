using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHelp : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool showHelp = true;

    [SerializeField] private GameObject[] helpArr;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            showHelp = !showHelp;
            foreach (GameObject go in helpArr)
            {
                if (showHelp)
                {
                    go.SetActive(false);

                }
                else
                {
                    go.SetActive(true);
                }

            }
        }
    }
}
