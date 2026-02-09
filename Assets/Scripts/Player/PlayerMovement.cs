using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator animator;

    private BoxCollider2D boxCollider;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private float xSpeed = 6f;

    [SerializeField] private float wallJumpForceX = 12f;
    [SerializeField] private float wallJumpForceY = 10f;
    [SerializeField] private float wallJumpLockTime = 0.15f;
    [SerializeField] private float defaultGravity = 5f;

    [SerializeField] private float wallJumpGravityMultiplier = 5f;

    private float horizontalInput;

    private float wallJumpLockCounter;

    private float wallJumpCooldown;


    [SerializeField] private float playerScale = 1.5f; 
    [SerializeField] private float jumpForce = 18f;

    [SerializeField] private bool onWall;

    [Header ("Audio")]
    [SerializeField] private AudioClip jumpSound;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

    }
        private void Update()
    {

        onWall = isOnWall();
        
        horizontalInput = Input.GetAxisRaw("Horizontal");

        wallJumpLockCounter -= Time.deltaTime;

        // Apply horizontal movement ONLY if not locked by wall jump
        if (wallJumpLockCounter <= 0f)
        {
            body.linearVelocity = new Vector2(
                horizontalInput * xSpeed,
                body.linearVelocity.y
            );

            Flip(horizontalInput);
        }

        // Ground jump
        if ( Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            GroundJump();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isOnWall() && !isGrounded())
        {
            WallJump();
        }

        // Wall slide
        if (isOnWall() && !isGrounded() && wallJumpLockCounter <= 0f)
        {
            body.gravityScale = 0f;
            body.linearVelocity = new Vector2(body.linearVelocity.x, -1f);
        }
        else
        {
            body.gravityScale = defaultGravity;
        }

        animator.SetBool("isRunning", horizontalInput != 0);
        animator.SetBool("isGrounded", isGrounded());
        animator.SetBool("isJumping", !isGrounded() && !isOnWall());
    }


    private void GroundJump()
    {
        SoundManager.instance.PlaySound(jumpSound);
        body.gravityScale = defaultGravity;
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
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

    private bool isGrounded()
    {

        RaycastHit2D raycastHitGround = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,0, Vector2.down, 0.1f, groundLayer);

        return raycastHitGround.collider != null;
    }

    private bool isOnWall()
    {
        RaycastHit2D raycastHitWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);

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



    public bool canAttack()
    {
        return !isOnWall();
    }



}
