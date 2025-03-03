using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlane))]
public class DotVisualizer : MonoBehaviour
{
    public GameObject dotPrefab;  // Assign a small dot prefab (e.g., a small sphere)
    private ARPlane arPlane;
    private List<GameObject> dots = new List<GameObject>();

    void Awake()
    {
        arPlane = GetComponent<ARPlane>();
        arPlane.boundaryChanged += OnBoundaryChanged;
    }

    void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        UpdateDots();
    }

    void UpdateDots()
    {
        // Remove previous dots
        foreach (var dot in dots)
        {
            Destroy(dot);
        }
        dots.Clear();

        // Get the boundary points and instantiate a dot at each point.
        foreach (var point in arPlane.boundary)
        {
            // Convert the 2D boundary point (x, y) to a 3D point in the plane’s space.
            Vector3 dotPosition = arPlane.transform.TransformPoint(new Vector3(point.x, 0f, point.y));
            GameObject dot = Instantiate(dotPrefab, dotPosition, Quaternion.identity, arPlane.transform);
            dots.Add(dot);
        }
    }
}
