using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class PlaceFire : MonoBehaviour
{
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private ARRaycastManager raycastManager;

    private List<GameObject> placedFires = new List<GameObject>();
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private float singleTouchStartTime = -1f;
    private bool hasSpawnedForCurrentTouch = false;
    private const float touchHoldDuration = 3f;
    private float lastMultiTouchTime = -1f;
    private const float multiTouchCooldown = 0.5f;

    void Update()
    {
        int touchCount = Input.touchCount;
        if (touchCount == 0)
        {
            ResetTouch();
            return;
        }

        // Prevent fire spawning if extinguisher is being dragged.
        if (ExtinguisherDragHandler.IsExtinguisherActive)
        {
            ResetTouch();
            return;
        }

        if (touchCount > 1)
        {
            lastMultiTouchTime = Time.time;
            ResetTouch();
            return;
        }
        
        if (Time.time - lastMultiTouchTime < multiTouchCooldown)
            return;

        Touch touch = Input.GetTouch(0);
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            ResetTouch();
            return;
        }

        if (touch.phase == TouchPhase.Began)
        {
            singleTouchStartTime = Time.time;
            hasSpawnedForCurrentTouch = false;
        }
        else if ((touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) && !hasSpawnedForCurrentTouch)
        {
            if (Time.time - singleTouchStartTime >= touchHoldDuration)
            {
                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    GameObject fireInstance = Instantiate(firePrefab, hitPose.position, hitPose.rotation);
                    placedFires.Add(fireInstance);
                    FireController fireController = fireInstance.GetComponent<FireController>();
                    if (fireController != null && FireManager.Instance != null)
                    {
                        FireManager.Instance.RegisterFire(fireController);
                    }
                    else
                    {
                        Debug.LogWarning("FireController component missing or FireManager instance not found.");
                    }
                    hasSpawnedForCurrentTouch = true;
                }
            }
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            ResetTouch();
        }
    }

    private void ResetTouch()
    {
        singleTouchStartTime = -1f;
        hasSpawnedForCurrentTouch = false;
    }
}
