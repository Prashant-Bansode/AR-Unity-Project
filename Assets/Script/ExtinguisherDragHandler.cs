using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ExtinguisherDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;                         // Reference to the Canvas.
    public ParticleSystem extinguisherEffect;       // Particle effect attached to the extinguisher UI button.
    public TextMeshProUGUI particleStatusText;      // TextMeshPro element for status display.

    // Static flag to signal that the extinguisher is active.
    public static bool IsExtinguisherActive { get; private set; } = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // Extinguishing variables:
    private FireController targetFire;
    private float extinguishStartTime = -1f;
    private float baseProgress = 0f;                     // Fire's extinguish progress when first detected.
    private const float extinguishDuration = 5f;         // Seconds needed to fully extinguish.
    private bool isDragging = false;
    private Vector2 currentDragPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if(extinguisherEffect != null)
            extinguisherEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if(particleStatusText != null)
            particleStatusText.text = "particle stop";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        IsExtinguisherActive = true; // Mark extinguisher active.
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        currentDragPosition = eventData.position;
        UpdateTargetFire(currentDragPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        currentDragPosition = eventData.position;
        UpdateTargetFire(currentDragPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        IsExtinguisherActive = false; // Mark extinguisher no longer active.
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if(extinguisherEffect != null && extinguisherEffect.isPlaying)
            extinguisherEffect.Stop();

        if(particleStatusText != null)
            particleStatusText.text = "particle stop";
    }

    void Update()
    {
        if (isDragging)
            UpdateTargetFire(currentDragPosition);
    }

    // Cast a ray from the UI's current screen position to detect a fire.
    private void UpdateTargetFire(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            FireController fire = hit.collider.GetComponent<FireController>();
            if (fire != null)
            {
                if (targetFire == null || targetFire != fire)
                {
                    targetFire = fire;
                    baseProgress = targetFire.GetExtinguishProgress();
                    extinguishStartTime = Time.time;
                }
                
                float elapsed = Time.time - extinguishStartTime;
                float cumulativeProgress = baseProgress + (elapsed / extinguishDuration);

                if (cumulativeProgress >= 1f)
                {
                    targetFire.Extinguish();
                    targetFire = null;
                    extinguishStartTime = -1f;
                    if(extinguisherEffect != null && extinguisherEffect.isPlaying)
                        extinguisherEffect.Stop();
                    if(particleStatusText != null)
                        particleStatusText.text = "particle stop";
                }
                else
                {
                    targetFire.UpdateExtinguishProgress(cumulativeProgress);
                    if(extinguisherEffect != null && !extinguisherEffect.isPlaying)
                        extinguisherEffect.Play();
                    if(particleStatusText != null)
                        particleStatusText.text = "particle playing";
                }
            }
            else
            {
                if(extinguisherEffect != null && extinguisherEffect.isPlaying)
                    extinguisherEffect.Stop();
                if(particleStatusText != null)
                    particleStatusText.text = "particle stop";
            }
        }
        else
        {
            if(extinguisherEffect != null && extinguisherEffect.isPlaying)
                extinguisherEffect.Stop();
            if(particleStatusText != null)
                particleStatusText.text = "particle stop";
        }
    }
}
