using UnityEngine;

public class PropRotation : MonoBehaviour
{
    public Vector3 rotationSpeed; // �ʴ� ȸ�� �ӵ� (����/��)

    void Start()
    {
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
