using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 11f;
    [SerializeField] private float coyoteTime = 0.10f;
    [SerializeField] private float jumpBuffer = 0.12f;

    [Header("Suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Vector3 startPosition;
    private bool leftHeld;
    private bool rightHeld;
    private float horizontal;
    private float coyoteCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        startPosition = transform.position;
        body.freezeRotation = true;
    }

    private void Update()
    {
        int keyboardDirection = 0;
        bool keyboardJump = false;

#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            bool left = keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed;
            bool right = keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed;
            keyboardDirection = (right ? 1 : 0) - (left ? 1 : 0);
            keyboardJump = keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        keyboardDirection = (right ? 1 : 0) - (left ? 1 : 0);
        keyboardJump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
#endif

        int touchDirection = (rightHeld ? 1 : 0) - (leftHeld ? 1 : 0);
        horizontal = Mathf.Clamp(keyboardDirection + touchDirection, -1f, 1f);
        if (sprite != null && horizontal != 0f) sprite.flipX = horizontal < 0f;

        if (keyboardJump) RequestJump();
        jumpBufferCounter -= Time.deltaTime;

        if (groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer) != null)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Vector2 velocity = GetVelocity();
        velocity.x = horizontal * moveSpeed;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            velocity.y = jumpSpeed;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        SetVelocity(velocity);
    }

    public void SetLeftHeld(bool held) { leftHeld = held; }
    public void SetRightHeld(bool held) { rightHeld = held; }
    public void RequestJump() { jumpBufferCounter = jumpBuffer; }

    public void ResetAfterHit()
    {
        body.position = startPosition;
        SetVelocity(Vector2.zero);
        horizontal = 0f;
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
    }

    private Vector2 GetVelocity()
    {
#if UNITY_6000_0_OR_NEWER
        return body.linearVelocity;
#else
        return body.velocity;
#endif
    }

    private void SetVelocity(Vector2 value)
    {
#if UNITY_6000_0_OR_NEWER
        body.linearVelocity = value;
#else
        body.velocity = value;
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
