using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FillSlider : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI valueText1;
    [SerializeField] private TextMeshProUGUI valueText2;

    [Header("Color Settings")]
    [SerializeField] private Color lowValueColor = Color.red;
    [SerializeField] private Color highValueColor = Color.green;

    [Header("Animation Settings")]
    [SerializeField] private float shakeStrength = 10f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float scaleMultiplier = 1.5f;
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private float shakeThreshold = 0.5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource sfx1_press;  // Play on press
    [SerializeField] private AudioSource sfx2_loop;   // Loop while holding
    [SerializeField] private AudioSource sfx3_cancel; // Play on early release
    [SerializeField] private AudioSource sfx4_complete; // Play when max is reached


    [SerializeField] private bool playFirst = true;

    private bool isHolding = false;
    private bool reachedMax = false;

    private void Start()
    {
        if (slider == null)
        {
            Debug.LogError("Slider reference is not set!");
            return;
        }

        slider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(slider.value);
    }

    private void OnSliderValueChanged(float value)
    {
        float normalizedValue = value / slider.maxValue;

        if (value > slider.minValue)
        {
            fillImage.color = Color.Lerp(lowValueColor, highValueColor, normalizedValue);
            valueText2.color = Color.Lerp(lowValueColor, highValueColor, normalizedValue);
        }
        else
        {
            fillImage.color = Color.white;
            valueText2.color = Color.white;
        }

        AnimateText(value);

        // Check if max value is reached
        if (value >= slider.maxValue && !reachedMax)
        {
            reachedMax = true;
            sfx2_loop.Stop(); // Stop hold loop
            if (sfx4_complete) sfx4_complete.Play(); // Play max reached sound
        }
    }

    private void Update()
    {
        // Detect press
        if (Input.GetMouseButtonDown(0) && !isHolding)
        {
            isHolding = true;
            reachedMax = false;

            // Play first sound (press)
            if (playFirst)
            {
                AudioManager.instance.PlayerSteps("playerCharge");
                StartCoroutine(PlayLoopAfterDelay(AudioManager.instance.playerSource.clip.length));
                playFirst = false;
            }
        }

        // Detect release
        if (isHolding && Input.GetMouseButtonUp(0))
        {
            playFirst = true;
            isHolding = false;
            sfx2_loop.Stop(); // Stop hold loop

            if (slider.value < slider.maxValue) // Play cancel sound if not max
            {
                AudioManager.instance.PlayerSteps("gunCancel");
            }
        }
    }

    private IEnumerator PlayLoopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isHolding)
        {
            AudioManager.instance.PlayLoop("playerCharge2");
        }
    }

    private void AnimateText(float value)
    {
        float normalizedValue = value / slider.maxValue;
        Vector3 targetScale = Vector3.one * (1 + (scaleMultiplier - 1) * normalizedValue);
        valueText1.transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack);
        valueText2.transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack);
    }
}
