using UnityEngine;

public class RenderingLayerToggle : MonoBehaviour
{
    // Public bool exposed in the Inspector to toggle Light Layer1
    public bool toggleLightLayer1;

    // Cached reference to the MeshRenderer component
    private MeshRenderer meshRenderer;

    // Bit mask for Light Layer 1
    private const uint LightLayer1Mask = 2; // Light Layer 1

    private void Awake()
    {
        // Get the MeshRenderer component attached to the GameObject
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer component not found on the GameObject.");
        }
    }

    private void Update()
    {
        if (meshRenderer != null)
        {
            // Get the current Rendering Layer Mask
            uint currentMask = meshRenderer.renderingLayerMask;

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

            // Apply the modified mask back to the MeshRenderer
            meshRenderer.renderingLayerMask = currentMask;
        }
    }
}
