using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile2D : MonoBehaviour
{
    [SerializeField] private float lifetime = 7f;
    [SerializeField] private LayerMask groundLayers;
    private Rigidbody2D body;
    private Vector2 velocity;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        GetComponent<Collider2D>().isTrigger = true;
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction, float speed)
    {
        velocity = direction.normalized * speed;
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController2D player = other.GetComponent<PlayerController2D>();
        if (player != null)
        {
            player.ResetAfterHit();
            Destroy(gameObject);
            return;
        }

        if ((groundLayers.value & (1 << other.gameObject.layer)) != 0)
            Destroy(gameObject);
    }
}
