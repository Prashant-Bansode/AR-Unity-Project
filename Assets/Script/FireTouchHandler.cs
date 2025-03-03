using UnityEngine;
using System.Collections.Generic;

public class FireTouchHandler : MonoBehaviour
{
    private bool isScaling = false;
    private Transform selectedFire;
    private float initialDistance;
    private Vector3 initialScale;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // If not already scaling, check which fire was touched first
            if (!isScaling)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch0.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Fire"))
                {
                    selectedFire = hit.transform;
                    initialDistance = Vector2.Distance(touch0.position, touch1.position);
                    initialScale = selectedFire.localScale;
                    isScaling = true;
                }
            }

            // If a fire is selected, apply scaling
            if (isScaling && selectedFire != null)
            {
                float currentDistance = Vector2.Distance(touch0.position, touch1.position);
                float scaleFactor = currentDistance / initialDistance;
                selectedFire.localScale = initialScale * scaleFactor;
            }
        }
        else
        {
            isScaling = false; // Reset when fingers are lifted
            selectedFire = null;
        }
    }
}
