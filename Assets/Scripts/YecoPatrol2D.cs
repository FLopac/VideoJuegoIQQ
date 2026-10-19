using UnityEngine;

// Mueve al yeco de izquierda a derecha entre su posición inicial y esa posición + recorrido.
public class YecoPatrol2D : MonoBehaviour
{
    [SerializeField] private float recorrido = 10f;
    [SerializeField] private float velocidad = 2f;

    private Vector3 inicio;
    private float distanciaRecorrida;

    private void Awake()
    {
        inicio = transform.position;
    }

    private void Update()
    {
        distanciaRecorrida += Mathf.Max(0f, velocidad) * Time.deltaTime;
        float x = inicio.x + Mathf.PingPong(distanciaRecorrida, Mathf.Max(0.01f, recorrido));
        transform.position = new Vector3(x, inicio.y, inicio.z);
    }
}
