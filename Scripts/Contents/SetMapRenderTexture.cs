using UnityEngine;

public class SetMapRenderTexture : MonoBehaviour
{
    public Camera camera;        // 메인 카메라
    public RenderTexture renderTex; // 연결할 Render Texture

    void Start()
    {
        // 메인 카메라에 Render Texture 연결
        if (camera != null && renderTex != null)
        {
            camera.targetTexture = renderTex;
        }
    }

    void OnDestroy()
    {
        // 연결 해제
        if (camera != null)
        {
            camera.targetTexture = null;
        }
    }
}
