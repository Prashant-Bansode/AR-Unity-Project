using UnityEngine;
using System.Collections.Generic;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance;
    public List<FireController> fireList = new List<FireController>();
    public GameObject completionPopup; // Assign a UI panel with the message "Fire extinguished successfully" (set inactive by default).

    void Awake()
    {
        Instance = this;
    }

    public void RegisterFire(FireController fire)
    {
        if (!fireList.Contains(fire))
            fireList.Add(fire);
    }

    // Check if all fires are extinguished.
    public void CheckAllFiresExtinguished()
    {
        foreach (FireController fire in fireList)
        {
            if (fire.gameObject.activeSelf)
                return; // Some fires are still active.
        }

        // All fires are extinguished. Show the completion popup.
        if (completionPopup != null)
            completionPopup.SetActive(true);
    }
}
