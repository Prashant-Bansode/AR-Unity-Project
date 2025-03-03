using UnityEngine;
using UnityEngine.UI;

public class FireController : MonoBehaviour
{
    public Slider progressBar;  // Assign a UI Slider in the Inspector.
    private float extinguishProgress = 0f; // 0 = full fire; 1 = fully extinguished.
    private bool extinguished = false;

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = 1f - extinguishProgress;
        }
    }

    // Called to update the progress.
    public void UpdateExtinguishProgress(float progress)
    {
        extinguishProgress = Mathf.Clamp01(progress);
        if (progressBar != null)
            progressBar.value = 1f - extinguishProgress;
    }

    // Returns the current extinguish progress.
    public float GetExtinguishProgress()
    {
        return extinguishProgress;
    }

    // Called when extinguishing is complete.
    public void Extinguish()
    {
        extinguished = true;
        extinguishProgress = 1f;
        if (progressBar != null)
            progressBar.value = 0f; // Fully extinguished.
        // Optionally trigger an extinguish effect or animation here.
        gameObject.SetActive(false);
        FireManager.Instance.CheckAllFiresExtinguished();
    }

    public bool IsExtinguished()
    {
        return extinguished;
    }
}
