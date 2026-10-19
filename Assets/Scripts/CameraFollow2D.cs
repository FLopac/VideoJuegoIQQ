using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = new Vector2(1.5f, 0.5f);
    [SerializeField] private float smoothTime = 0.15f;
    private Vector3 currentVelocity;

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref currentVelocity, smoothTime);
    }
}
