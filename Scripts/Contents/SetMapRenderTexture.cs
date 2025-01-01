using UnityEngine;

public class SetMapRenderTexture : MonoBehaviour
{
    public Camera camera;        // ���� ī�޶�
    public RenderTexture renderTex; // ������ Render Texture

    void Start()
    {
        // ���� ī�޶� Render Texture ����
        if (camera != null && renderTex != null)
        {
            camera.targetTexture = renderTex;
        }
    }

    void OnDestroy()
    {
        // ���� ����
        if (camera != null)
        {
            camera.targetTexture = null;
        }
    }
}
