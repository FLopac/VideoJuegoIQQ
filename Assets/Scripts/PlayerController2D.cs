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
    // Se conservan estos campos para mantener las referencias de la escena.
    // El salto usa los contactos del Rigidbody2D, no la escala del dibujo.
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dibujo: arrastra el Sprite Renderer del hijo Visual")]
    [SerializeField] private SpriteRenderer visualRenderer;

    [Header("Animacion sencilla: opcional")]
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] runFrames;
    [SerializeField] private Sprite jumpUpSprite;
    [SerializeField] private Sprite jumpDownSprite;
    [SerializeField] private float idleFramesPerSecond = 6f;
    [SerializeField] private float runFramesPerSecond = 10f;

    private enum VisualState { Idle, Run, Rising, Falling }
    private Rigidbody2D body;
    private PhysicsMaterial2D noFrictionMaterial;
    private Sprite defaultSprite;
    private Vector3 startPosition;
    private readonly ContactPoint2D[] groundContacts = new ContactPoint2D[32];
    private bool leftHeld;
    private bool rightHeld;
    private float horizontal;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float animationTime;
    private VisualState visualState;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        body.bodyType = RigidbodyType2D.Dynamic;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (visualRenderer == null) visualRenderer = FindVisualRenderer();
        if (visualRenderer != null) defaultSprite = visualRenderer.sprite;

        // Evita que la friccion de los contactos laterales sostenga al jugador.
        noFrictionMaterial = new PhysicsMaterial2D("Jugador_SinFriccion")
        {
            friction = 0f,
            bounciness = 0f
        };
        body.sharedMaterial = noFrictionMaterial;
        foreach (Collider2D hitbox in GetComponentsInChildren<Collider2D>(true))
        {
            if (hitbox.attachedRigidbody == body)
                hitbox.sharedMaterial = noFrictionMaterial;
        }

        if (groundLayer.value == 0)
            Debug.LogWarning("Jugador: selecciona la capa del Suelo en Ground Layer.", this);
        if (body.gravityScale <= 0f)
            Debug.LogWarning("Jugador: Gravity Scale debe ser mayor que cero para caer.", this);
    }

    private SpriteRenderer FindVisualRenderer()
    {
        foreach (SpriteRenderer candidate in GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (candidate.gameObject != gameObject && candidate.enabled &&
                candidate.gameObject.activeInHierarchy && candidate.sprite != null)
                return candidate;
        }
        return GetComponent<SpriteRenderer>();
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
            keyboardJump = keyboard.spaceKey.wasPressedThisFrame ||
                keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        keyboardDirection = (right ? 1 : 0) - (left ? 1 : 0);
        keyboardJump = Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
#endif

        int touchDirection = (rightHeld ? 1 : 0) - (leftHeld ? 1 : 0);
        horizontal = Mathf.Clamp(keyboardDirection + touchDirection, -1f, 1f);
        if (keyboardJump) RequestJump();
        UpdateVisual();
    }

    private void FixedUpdate()
    {
        Vector2 velocity = GetVelocity();
        IsGrounded = velocity.y <= 0.1f && HasGroundContact();
        coyoteCounter = IsGrounded
            ? Mathf.Max(0f, coyoteTime)
            : Mathf.Max(0f, coyoteCounter - Time.fixedDeltaTime);

        // Cambiar X conserva la velocidad vertical de salto y de caida.
        velocity.x = horizontal * moveSpeed;
        if (jumpBufferCounter > 0f && (IsGrounded || coyoteCounter > 0f))
        {
            velocity.y = jumpSpeed;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            IsGrounded = false;
        }

        SetVelocity(velocity);
        jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - Time.fixedDeltaTime);
    }

    private bool HasGroundContact()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayer);
        filter.useTriggers = false;
        int count = body.GetContacts(filter, groundContacts);
        for (int i = 0; i < count; i++)
        {
            // Solo las superficies que sostienen desde abajo cuentan como suelo.
            // La normal de una pared vertical no supera este umbral.
            if (groundContacts[i].normal.y >= 0.65f) return true;
        }
        return false;
    }

    private void UpdateVisual()
    {
        if (visualRenderer == null) return;
        if (horizontal != 0f) visualRenderer.flipX = horizontal < 0f;

        bool hasAnimation = (idleFrames != null && idleFrames.Length > 0) ||
            (runFrames != null && runFrames.Length > 0) ||
            jumpUpSprite != null || jumpDownSprite != null;
        if (!hasAnimation) return;

        Vector2 velocity = GetVelocity();
        VisualState next = !IsGrounded
            ? (velocity.y > 0.1f ? VisualState.Rising : VisualState.Falling)
            : (Mathf.Abs(velocity.x) > 0.1f ? VisualState.Run : VisualState.Idle);

        if (next != visualState)
        {
            visualState = next;
            animationTime = 0f;
        }

        Sprite selected = defaultSprite;
        if (next == VisualState.Rising && jumpUpSprite != null)
            selected = jumpUpSprite;
        else if (next == VisualState.Falling && jumpDownSprite != null)
            selected = jumpDownSprite;
        else if (next == VisualState.Idle || next == VisualState.Run)
        {
            Sprite[] frames = next == VisualState.Run ? runFrames : idleFrames;
            float fps = next == VisualState.Run ? runFramesPerSecond : idleFramesPerSecond;
            if (frames != null && frames.Length > 0)
            {
                int index = Mathf.FloorToInt(animationTime * Mathf.Max(0.1f, fps)) % frames.Length;
                if (frames[index] != null) selected = frames[index];
            }
        }

        if (selected != null) visualRenderer.sprite = selected;
        animationTime += Time.deltaTime;
    }

    public void SetLeftHeld(bool held) { leftHeld = held; }
    public void SetRightHeld(bool held) { rightHeld = held; }
    public void RequestJump() { jumpBufferCounter = Mathf.Max(0.02f, jumpBuffer); }

    public void ResetAfterHit()
    {
        body.position = startPosition;
        SetVelocity(Vector2.zero);
        horizontal = 0f;
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
        IsGrounded = false;
        animationTime = 0f;
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

    private void OnDestroy()
    {
        if (noFrictionMaterial != null) Destroy(noFrictionMaterial);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
