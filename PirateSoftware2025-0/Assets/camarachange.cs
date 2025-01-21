using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camarachange : MonoBehaviour
{

    public GameObject camara;
    public GameObject camvas;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void startAnimation()
    {
        camara.SetActive(true);
    }
}
