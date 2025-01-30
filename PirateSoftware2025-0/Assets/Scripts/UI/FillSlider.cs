using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For Slider and Image
using TMPro; // For TextMeshProUGUI
using DG.Tweening; // For DOTween animations

public class FillSlider : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider slider; // Reference to the Slider
    [SerializeField] private Image fillImage; // Reference to the Image (fill bar)
    [SerializeField] private TextMeshProUGUI valueText1; // First TextMeshPro text
    [SerializeField] private TextMeshProUGUI valueText2; // Second TextMeshPro text

    [Header("Color Settings")]
    [SerializeField] private Color lowValueColor = Color.red; // Color for low slider values
    [SerializeField] private Color highValueColor = Color.green; // Color for high slider values

    [Header("Animation Settings")]
    [SerializeField] private float shakeStrength = 10f; // Strength of the shake animation
    [SerializeField] private float shakeDuration = 0.5f; // Duration of the shake animation
    [SerializeField] private float scaleMultiplier = 1.5f; // Scale multiplier for the text
    [SerializeField] private float scaleDuration = 0.3f; // Duration of the scale animation
    [SerializeField] private float shakeThreshold = 0.5f; // Slider value threshold for continuous shaking

    private Tween shakeTween1; // Tween for the first text's shake animation
    private Tween shakeTween2; // Tween for the second text's shake animation

    private Vector3 originalText1Position; // Original position of the first text
    private Vector3 originalText2Position; // Original position of the second text
    private Vector3 originalText1Scale; // Original scale of the first text
    private Vector3 originalText2Scale; // Original scale of the second text

    private void Start()
    {
        // Ensure the slider reference is set
        if (slider == null)
        {
            Debug.LogError("Slider reference is not set!");
            return;
        }

        // Store the original positions and scales of the text objects
        originalText1Position = valueText1.transform.localPosition;
        originalText2Position = valueText2.transform.localPosition;
        originalText1Scale = valueText1.transform.localScale;
        originalText2Scale = valueText2.transform.localScale;

        // Add a listener to the slider's value change event
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // Initialize the UI with the slider's starting value
        OnSliderValueChanged(slider.value);
    }

    private void OnSliderValueChanged(float value)
    {
        // Update the text to display the slider's value
        string formattedValue = value.ToString("F1"); // Format to one decimal place

        // Change the fill color based on the slider's value
        float normalizedValue = value / slider.maxValue; // Normalize the value between 0 and 1

        // Only change the color if the value is above the minimum
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

        // Animate the text based on the slider's value
        AnimateText(value);
    }

    private void AnimateText(float value)
    {
        // Calculate the intensity of the animation based on the slider's value
        float normalizedValue = value / slider.maxValue; // Normalize the value between 0 and 1

        // Scale both text objects
        Vector3 targetScale = Vector3.one * (1 + (scaleMultiplier - 1) * normalizedValue);
        valueText1.transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack);
        valueText2.transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutBack);

        // Handle continuous shaking if the value is above the threshold
        if (normalizedValue >= shakeThreshold)
        {
            // Start continuous shaking if not already shaking
            if (shakeTween1 == null || !shakeTween1.IsPlaying())
            {
                shakeTween1 = valueText1.transform.DOShakePosition(shakeDuration, shakeStrength * normalizedValue)
                    .SetLoops(-1, LoopType.Restart); // Loop indefinitely
                shakeTween2 = valueText2.transform.DOShakePosition(shakeDuration, shakeStrength * normalizedValue)
                    .SetLoops(-1, LoopType.Restart); // Loop indefinitely
            }
        }
        else
        {
            // Stop continuous shaking if the value is below the threshold
            if (shakeTween1 != null && shakeTween1.IsPlaying())
            {
                shakeTween1.Kill(); // Stop the shake animation
                shakeTween2.Kill(); // Stop the shake animation
                shakeTween1 = null;
                shakeTween2 = null;

                // Reset the text positions and scales to their original state
                valueText1.transform.localPosition = originalText1Position;
                valueText2.transform.localPosition = originalText2Position;
                valueText1.transform.localScale = originalText1Scale;
                valueText2.transform.localScale = originalText2Scale;
            }
        }
    }
}