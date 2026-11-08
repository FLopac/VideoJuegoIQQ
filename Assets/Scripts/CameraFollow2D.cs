using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = Vector2.zero;
    [Min(0.01f)]
    [SerializeField] private float smoothTime = 0.18f;

    [Header("Encuadre del nivel")]
    [Tooltip("Collider que abarca el ancho visible del nivel, por ejemplo el de Suelo.")]
    [SerializeField] private Collider2D horizontalBounds;
    [SerializeField] private bool followVertical = false;

    private Camera view;
    private float fixedY;
    private float speedX;
    private float speedY;

    private void Awake()
    {
        view = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        fixedY = transform.position.y;
        speedX = 0f;
        speedY = 0f;
    }

    private void LateUpdate()
    {
        if (target == null || Time.deltaTime <= 0f) return;

        Vector3 current = transform.position;
        float desiredX = target.position.x + offset.x;
        bool bounded = TryGetCameraRange(out float minX, out float maxX);
        if (bounded) desiredX = Mathf.Clamp(desiredX, minX, maxX);

        float time = Mathf.Max(0.01f, smoothTime);
        float x = Mathf.SmoothDamp(current.x, desiredX, ref speedX, time);
        if (bounded)
        {
            // Respeta el encuadre incluso si cambian el zoom o el aspecto.
            float clampedX = Mathf.Clamp(x, minX, maxX);
            if (clampedX != x) speedX = 0f;
            x = clampedX;
        }

        float y = fixedY;
        if (followVertical)
            y = Mathf.SmoothDamp(current.y, target.position.y + offset.y, ref speedY, time);
        else
            speedY = 0f;

        transform.position = new Vector3(x, y, current.z);
    }

    private bool TryGetCameraRange(out float minX, out float maxX)
    {
        minX = maxX = 0f;
        if (view == null || !view.orthographic || horizontalBounds == null ||
            !horizontalBounds.enabled || !horizontalBounds.gameObject.activeInHierarchy)
            return false;

        Bounds level = horizontalBounds.bounds;
        if (level.size.x <= 0f) return false;

        float halfWidth = view.orthographicSize * view.aspect;
        minX = level.min.x + halfWidth;
        maxX = level.max.x - halfWidth;

        // Si cabe todo el nivel en pantalla, la vista permanece centrada.
        if (minX > maxX) minX = maxX = level.center.x;
        return true;
    }
}
