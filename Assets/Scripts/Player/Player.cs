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

    private float coyoteCountdown;
    private float horizontalInput;

    private float wallJumpLockCounter;

    private float wallJumpCooldown;

    private bool onWall;

    private PlayerAttack attackScript;

    public bool isGrounded {get; private set;}

    private int jumpBufferCheckInterval = 20;
    private Coroutine jumpBufferRoutine;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        attackScript = GetComponent<PlayerAttack>();

    }
    private void Update()
    {
        isGrounded = IsGrounded();
        if (attackScript.isRecoiling)   
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            return;
        }
        onWall = isOnWall();
        
        horizontalInput = Input.GetAxisRaw("Horizontal");

        wallJumpLockCounter -= Time.deltaTime;

        // Apply horizontal movement ONLY if not locked by wall jump
        if (wallJumpLockCounter <= 0f)
        {
            float targetSpeed = horizontalInput * xSpeed;
            float currentSpeed = body.linearVelocity.x;
            bool hasInput = Mathf.Abs(horizontalInput) > 0.01f;
            float accelRate = hasInput
                ? (isGrounded ? groundAcceleration : airAcceleration)
                : (isGrounded ? groundDeceleration : airDeceleration);

            float newSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                accelRate * Time.deltaTime
            );

            body.linearVelocity = new Vector2(newSpeed, body.linearVelocity.y);

            Flip(horizontalInput);
        }

        if (IsGrounded())
        {
            coyoteCountdown = coyoteTime;
        }
        else
        {
            coyoteCountdown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsCoyote())
            {
                // if grounded or in coyote, apply ground jump
                CancelJumpBuffer();
                GroundJump();
            }
            else if (isOnWall() && !IsGrounded())
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

        // Wall slide
        if (isOnWall() && !IsGrounded() && wallJumpLockCounter <= 0f)
        {
            body.gravityScale = 0f;
            body.linearVelocity = new Vector2(body.linearVelocity.x, -1f);
        }
        else
        {
            ApplyVerticalGravity();
        }

        animator.SetBool("isRunning", horizontalInput != 0);
        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isJumping", !IsGrounded() && !isOnWall());
    }


    private void GroundJump()
    {

        body.gravityScale = defaultGravity * upwardGravityMultiplier;
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
        SoundManager.instance.PlaySound(jumpSound);
        coyoteCountdown = 0;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private bool IsGrounded()
    {

        RaycastHit2D raycastHitGround = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size,0, Vector2.down, 0.1f, groundLayer);

        return raycastHitGround.collider != null;
    }
    private bool IsCoyote()
    {
        return coyoteCountdown > 0;
    }
    private bool isOnWall()
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



    public bool canAttack()
    {
        return !isOnWall();
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
