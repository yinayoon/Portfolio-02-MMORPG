using UnityEngine;

public class SkinnedRenderingLayerToggle : MonoBehaviour
{
    // Public bool exposed in the Inspector to toggle Light Layer1
    public bool toggleLightLayer1;

    // Cached reference to the SkinnedMeshRenderer component
    private SkinnedMeshRenderer skinnedMeshRenderer;

    // Bit mask for Light Layer 1
    private const uint LightLayer1Mask = 2; // Light Layer 1

    private void Awake()
    {
        // Get the SkinnedMeshRenderer component attached to the GameObject
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        //if (skinnedMeshRenderer == null)
        //{
        //    Debug.LogError("SkinnedMeshRenderer component not found on the GameObject.");
        //}
    }

    private void Update()
    {
        if (skinnedMeshRenderer != null)
        {
            // Get the current Rendering Layer Mask
            uint currentMask = skinnedMeshRenderer.renderingLayerMask;

            if (toggleLightLayer1)
            {
                // Enable Light Layer 1 by setting the bit
                currentMask |= LightLayer1Mask;
            }
            else
            {
                // Disable Light Layer 1 by clearing the bit
                currentMask &= ~LightLayer1Mask;
            }

            // Apply the modified mask back to the SkinnedMeshRenderer
            skinnedMeshRenderer.renderingLayerMask = currentMask;
        }
    }
}
