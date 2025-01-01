using UnityEngine;

public class RenderingLayerManager : MonoBehaviour
{
    public bool layerManagingToggle;
    public SkinnedRenderingLayerToggle[] skinnedRenderingLayerToggles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (layerManagingToggle == true)
        {
            for (int i = 0; i < skinnedRenderingLayerToggles.Length; i++)
            {
                skinnedRenderingLayerToggles[i].toggleLightLayer1 = true;
            }
        }
        else if (layerManagingToggle == false)
        {
            for (int i = 0; i < skinnedRenderingLayerToggles.Length; i++)
            {
                skinnedRenderingLayerToggles[i].toggleLightLayer1 = false;
            }
        }
    }
}
