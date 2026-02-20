using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator animator;

    private CapsuleCollider2D collider;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask thinPlatformLayer;

    [Header ("Horizontal Movement")]
    [SerializeField] private float xSpeed = 6f;
    [SerializeField] private float groundAcceleration = 80f;
    [SerializeField] private float airAcceleration = 45f;
    [SerializeField] private float groundDeceleration = 90f;
    [SerializeField] private float airDeceleration = 35f;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpForceX = 12f;
    [SerializeField] private float wallJumpForceY = 10f;
    [SerializeField] private float wallJumpLockTime = 0.15f;
    [SerializeField] private float wallJumpGravityMultiplier = 5f;
    [SerializeField] private float wallSlideGravityScale = 1f;

    [Header ("Ground Jump")]
    [SerializeField] private float defaultGravity = 5f;
    [SerializeField] private float upwardGravityMultiplier = 1f;
    [SerializeField] private float downwardGravityMultiplier = 2f;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpForce = 18f;
    [SerializeField] private float jumpBufferDuration = 1f;

    [Header ("Audio")]
    [SerializeField] private AudioClip jumpSound;

    [Header("Scale")]
    [SerializeField] private float playerScale = 1.5f; 

    [Header("Corner Correction")]
    [SerializeField] private bool enableCornerCorrection = true;
    [SerializeField] private float cornerCorrectionDistance = 0.08f;
    [SerializeField] private float cornerCeilingCheckDistance = 0.05f;
    [SerializeField] private float cornerProbeInset = 0.02f;
    [SerializeField] private float cornerPostCorrectionClearance = 0.2f;
    [SerializeField] private float cornerCorrectionCooldown = 0.06f;


    [SerializeField] private ParticleSystem walkParticles;
    private float coyoteCountdown;
    private float horizontalInput;

    private float wallJumpLockCounter;

    private float wallJumpCooldown;
    private PlayerAttack attackScript;

    private int jumpBufferCheckInterval = 20;
    private Coroutine jumpBufferRoutine;
    private float cornerCorrectionCooldownCounter;

    public enum PlayerMovementState {Idle, Running, Stunned, Dead}

    public enum PlayerPowerUpState {Normal, Invincible}
    public enum PlayerLocationState {OnAir, OnGround, OnWall}

    public PlayerMovementState currentMovementState {get; private set;} = PlayerMovementState.Idle;
    
    public PlayerPowerUpState currentPowerUpState {get; private set;} = PlayerPowerUpState.Normal;
    public PlayerLocationState currentLocationState {get; private set;} = PlayerLocationState.OnGround;
    private void Start() {
        GetComponent<PlayerRespawn>().SetInitialPosition(transform.position);
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        attackScript = GetComponent<PlayerAttack>();
    }
    private void Update()
    {
        //isGrounded = IsGrounded();
        horizontalInput = Input.GetAxisRaw("Horizontal");

        //isRunning = horizontalInput != 0;
        DetermineMovementState();
        DetermineLocationState();
        HandleDustParticles();

       //if (attackScript.isRecoiling)  
        if (currentMovementState == PlayerMovementState.Stunned)
            {
                HandleStun();
            }
        HandlePerFrameCounters();
        HandleHorizontalMovement();

        HandleJump();

        TryCornerCorrection();

        WallSlide();

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isRunning", currentMovementState == PlayerMovementState.Running);
        animator.SetBool("isGrounded", currentLocationState == PlayerLocationState.OnGround);
        animator.SetBool("isJumping", currentLocationState == PlayerLocationState.OnAir);
        animator.SetBool("isOnWall", currentLocationState == PlayerLocationState.OnWall);
    }
    private void DetermineMovementState()
    {
        if (currentMovementState == PlayerMovementState.Dead || currentMovementState == PlayerMovementState.Stunned)
        {
            return;
        }

        if (horizontalInput != 0)
        {
            ChangeMovementState(PlayerMovementState.Running);
        }
        else
        {
            ChangeMovementState(PlayerMovementState.Idle);
        }
    }

    public void DetermineMovementStateAfterRecoilAndRespawn()
    {
        if (horizontalInput != 0)
        {
            ChangeMovementState(PlayerMovementState.Running);
        }
        else
        {
            ChangeMovementState(PlayerMovementState.Idle);
        }
    }

    private void DetermineLocationState()
    {
        if (IsGrounded())
        {
            currentLocationState = PlayerLocationState.OnGround;
        }
        else if (IsOnWall())
        {
            currentLocationState = PlayerLocationState.OnWall;
        }
        else
        {
            currentLocationState = PlayerLocationState.OnAir;
        }

    }

    public void ChangeMovementState(PlayerMovementState newState)
    {
        currentMovementState = newState;
    }
        
    public void ChangePowerUpState(PlayerPowerUpState newState)
    {
        currentPowerUpState = newState;
    }
    private void GroundJump()
    {

        body.gravityScale = defaultGravity * upwardGravityMultiplier;
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        SoundManager.instance.PlaySound(jumpSound);
        coyoteCountdown = 0;
    }

    private void HandleDustParticles()
    {
        if (currentLocationState == PlayerLocationState.OnGround && currentMovementState == PlayerMovementState.Running)
        {
            // particles must be in the opposite of the input direction
            walkParticles.transform.rotation = horizontalInput < 0
                ? Quaternion.Euler(0, 180, 0)
                : Quaternion.identity;

            if (!walkParticles.isPlaying)
            {
                walkParticles.Play();
            }
        }
        else
        {
            if (walkParticles.isPlaying)
            {
                walkParticles.Stop();
            }

            
        }
    }

    private void HandleHorizontalMovement()
    {
        // Apply horizontal movement ONLY if not locked by wall jump
        if (wallJumpLockCounter <= 0f)
        {
            float targetSpeed = horizontalInput * xSpeed;
            float currentSpeed = body.linearVelocity.x;
            bool hasInput = Mathf.Abs(horizontalInput) > 0.01f;
            float accelRate = hasInput
                ? (currentLocationState == PlayerLocationState.OnGround ? groundAcceleration : airAcceleration)
                : (currentLocationState == PlayerLocationState.OnGround ? groundDeceleration : airDeceleration);

            float newSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                accelRate * Time.deltaTime
            );

            body.linearVelocity = new Vector2(newSpeed, body.linearVelocity.y);

            Flip(horizontalInput);
        }
    }
    private void HandlePerFrameCounters()
    {
        wallJumpLockCounter -= Time.deltaTime;
        cornerCorrectionCooldownCounter -= Time.deltaTime;

        if (currentLocationState == PlayerLocationState.OnGround)
            {
                //animator.SetBool("isOnWall", false);
                coyoteCountdown = coyoteTime;
            }
        else
            {
                coyoteCountdown -= Time.deltaTime;
            }
    }


    private void HandleStun()
    {
        animator.SetBool("isJumping", false);
        animator.SetBool("isRunning", false);
    }


    private void HandleJump()
    {
        // coyote timer logic was here before 19.02

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsCoyote())
            {
                // if grounded or in coyote, apply ground jump
                CancelJumpBuffer();
                GroundJump();
            }
            else if (currentLocationState == PlayerLocationState.OnWall && currentLocationState != PlayerLocationState.OnGround)
            {
                CancelJumpBuffer();
                WallJump();
            }
            else if (jumpBufferRoutine == null)
            {
                // if pressed jump but not grounded or on wall, wait for some duration and check if the player touches the ground to give the player some room for jumping
                jumpBufferRoutine = StartCoroutine(JumpBufferRoutine());
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);
        }
    }
    private void WallJump()
    {
        SoundManager.instance.PlaySound(jumpSound);
        int wallDir = WallDirection(); // +1 right wall, -1 left wall

        wallJumpLockCounter = wallJumpLockTime;
        body.gravityScale = defaultGravity * wallJumpGravityMultiplier;

        // Force diagonal launch AWAY from wall
        body.linearVelocity = new Vector2(
            -wallDir * wallJumpForceX,
            wallJumpForceY
        );

        // Face jump direction
        Flip(-wallDir);
    }

    private void WallSlide()
    {
        // Wall slide
        if (currentLocationState == PlayerLocationState.OnWall && currentLocationState != PlayerLocationState.OnGround && wallJumpLockCounter <= 0f)
        {
            body.gravityScale = wallSlideGravityScale;
            body.linearVelocity = new Vector2(body.linearVelocity.x, -wallSlideGravityScale);
        }
        else
        {
            ApplyVerticalGravity();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private bool IsGrounded()
    {

        RaycastHit2D raycastHitGround = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size,0, Vector2.down, 0.1f, groundLayer);
        RaycastHit2D raycastHitThinPlatform = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size,0, Vector2.down, 0.1f, thinPlatformLayer);

        return raycastHitGround.collider != null || raycastHitThinPlatform.collider != null;
    }
    private bool IsCoyote()
    {
        return coyoteCountdown > 0;
    }
    private bool IsOnWall()
    {
        RaycastHit2D raycastHitWall = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);

        return raycastHitWall.collider != null;
    }

    private void Flip(float horizontalInput)
    {
        if ( horizontalInput > 0.01f )
        {
            transform.localScale = new Vector3( playerScale, playerScale, playerScale );
        }
        else if ( horizontalInput < -0.01f )
        {
            transform.localScale = new Vector3(-playerScale, playerScale, playerScale);
        }
    }

    private int WallDirection()
    {
        // +1 = wall on right, -1 = wall on left
        return Mathf.RoundToInt(Mathf.Sign(transform.localScale.x));
    }

    private void ApplyVerticalGravity()
    {
        if (wallJumpLockCounter > 0f)
        {
            body.gravityScale = defaultGravity * wallJumpGravityMultiplier;
            return;
        }

        if (body.linearVelocity.y > 0.01f)
        {
            body.gravityScale = defaultGravity * upwardGravityMultiplier;
        }
        else if (body.linearVelocity.y < -0.01f)
        {
            body.gravityScale = defaultGravity * downwardGravityMultiplier;
        }
        else
        {
            body.gravityScale = defaultGravity;
        }
    }

    private void TryCornerCorrection()
    {
        if (!enableCornerCorrection || cornerCorrectionCooldownCounter > 0f) return;
        if (body.linearVelocity.y <= 0.01f) return;
        if (!HasCeilingAbove(0f, cornerCeilingCheckDistance)) return;

        bool leftBlocked = IsTopCornerBlocked(true);
        bool rightBlocked = IsTopCornerBlocked(false);

        if (leftBlocked == rightBlocked) return;

        float correctionDirection = leftBlocked ? 1f : -1f;

        if (!CanApplyCornerCorrection(correctionDirection)) return;

        transform.position += new Vector3(correctionDirection * cornerCorrectionDistance, 0f, 0f);
        cornerCorrectionCooldownCounter = cornerCorrectionCooldown;
    }

    private bool HasCeilingAbove(float xOffset, float checkDistance)
    {
        Vector2 center = (Vector2)collider.bounds.center + new Vector2(xOffset, 0f);
        Vector2 size = collider.bounds.size * 0.98f;
        RaycastHit2D hit = Physics2D.BoxCast(
            center,
            size,
            0f,
            Vector2.up,
            checkDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    private bool IsTopCornerBlocked(bool checkLeftCorner)
    {
        Bounds bounds = collider.bounds;
        float x = checkLeftCorner
            ? bounds.min.x + cornerProbeInset
            : bounds.max.x - cornerProbeInset;

        Vector2 origin = new Vector2(x, bounds.max.y);
        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.up,
            cornerCeilingCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    private bool CanApplyCornerCorrection(float correctionDirection)
    {
        Vector2 size = collider.bounds.size * 0.95f;
        RaycastHit2D horizontalBlock = Physics2D.BoxCast(
            collider.bounds.center,
            size,
            0f,
            Vector2.right * correctionDirection,
            cornerCorrectionDistance,
            groundLayer
        );

        if (horizontalBlock.collider != null) return false;

        return !HasCeilingAbove(
            correctionDirection * cornerCorrectionDistance,
            cornerPostCorrectionClearance
        );
    }


    private void CancelJumpBuffer()
    {
        if (jumpBufferRoutine != null)
        {
            StopCoroutine(jumpBufferRoutine);
            jumpBufferRoutine = null;
        }
    }

    private void OnDisable()
    {
        CancelJumpBuffer();
    }

    private IEnumerator JumpBufferRoutine()
    {
        for ( int i = 0; i < jumpBufferCheckInterval; i++ )
        {
            if (IsGrounded())
            {
                GroundJump();
                break;
            }
            yield return new WaitForSeconds(jumpBufferDuration / jumpBufferCheckInterval);
        }

        jumpBufferRoutine = null;
    }

}
