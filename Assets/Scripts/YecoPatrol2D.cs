using UnityEngine;

public class YecoPatrol2D : MonoBehaviour
{
    [SerializeField] private float recorrido = 10f;
    [SerializeField] private float velocidad = 2f;

    [Header("Dibujo: arrastra el Sprite Renderer del hijo Visual")]
    [SerializeField] private SpriteRenderer visualRenderer;

    [Header("Animacion de vuelo: opcional")]
    [SerializeField] private Sprite[] flightFrames;
    [SerializeField] private float flightFramesPerSecond = 10f;

    private Vector3 inicio;
    private float distanciaRecorrida;
    private float animationTime;

    private void Awake()
    {
        inicio = transform.position;
        if (visualRenderer != null) return;

        foreach (SpriteRenderer candidate in GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (candidate.gameObject != gameObject && candidate.enabled &&
                candidate.gameObject.activeInHierarchy && candidate.sprite != null)
            {
                visualRenderer = candidate;
                break;
            }
        }
        if (visualRenderer == null) visualRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float length = Mathf.Max(0.01f, recorrido);
        distanciaRecorrida += Mathf.Max(0f, velocidad) * Time.deltaTime;
        float x = inicio.x + Mathf.PingPong(distanciaRecorrida, length);
        transform.position = new Vector3(x, inicio.y, inicio.z);

        if (visualRenderer == null) return;
        // Gira solamente el dibujo; el punto LanzadorYeco sigue bajo el cuerpo.
        visualRenderer.flipX = Mathf.Repeat(distanciaRecorrida, length * 2f) >= length;

        if (flightFrames == null || flightFrames.Length == 0) return;
        int index = Mathf.FloorToInt(animationTime * Mathf.Max(0.1f, flightFramesPerSecond)) % flightFrames.Length;
        if (flightFrames[index] != null) visualRenderer.sprite = flightFrames[index];
        animationTime += Time.deltaTime;
    }
}
