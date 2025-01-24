using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraShake : MonoBehaviour
{

    private CinemachineVirtualCamera vCam;
    [SerializeField]private float shakeIntensity = 2f;
    [SerializeField]private float ShakeTime = 1f;

    private float timer;
    // Start is called before the first frame update
    private void Start()
    {
     
         stopShake();
        
    }
    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            shakeCam();
            print("CameraShake");
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                stopShake();
            }
        }
    }
    public void shakeCam()
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = shakeIntensity;
        timer = ShakeTime;
    }
    public void setShakeCam(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = intensity;
        timer = time;
    }
   
    public void stopShake()
    {
        CinemachineBasicMultiChannelPerlin _cbmcp = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;
        timer = 0;
    }
}
