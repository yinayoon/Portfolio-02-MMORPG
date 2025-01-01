using UnityEngine;

public class PropRotation : MonoBehaviour
{
    public Vector3 rotationSpeed; // 초당 회전 속도 (각도/초)

    void Start()
    {
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
