using UnityEngine;
using System.Collections;

public class KnightEnemy : MonoBehaviour {
    [SerializeField] private float damage;
    private Vector3 destination;
    [SerializeField] private float speed;
    [SerializeField] private float patrolDistance;

    [SerializeField] private float patrolWalkDuration;
    [SerializeField] private float patrolBreakDuration;
    private Coroutine patrolRoutine;

    private Animator animator;
    private Rigidbody2D body;
    private BoxCollider2D collider;

    private float patrolCenterXPosition;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    private enum EnemyState {patrolling, waiting, chasing, attacking}
    private EnemyState currentState = EnemyState.patrolling;
    private bool isAttacking;
    
    private Transform environmentCollision;
    private GameObject environmentCollisionObject;

    private Transform knightAttack;
    private GameObject knightAttackObject;
    private KnightAttack attackScript;


    protected void Awake() {
        patrolCenterXPosition = transform.position.x;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();

        environmentCollision = transform.Find("KnightEnvironmentCollision");
        environmentCollisionObject = environmentCollision.gameObject;
        
        knightAttack = transform.Find("KnightAttack");
        knightAttackObject = knightAttack.gameObject;
        attackScript = knightAttackObject.GetComponent<KnightAttack>();
        patrolRoutine = StartCoroutine(Patrol());


    }
    private void Update()
    {
        if (currentState == EnemyState.patrolling)
        {
            Walk();
        }
        if (!attackScript.getIsWalking())
        {
            body.linearVelocity = Vector2.zero;
        }
        if(IsCollidingWithEnvironment())
        {
            Flip();
            ResetAfterCollisionWithEnvironmentFlip();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    IEnumerator Patrol()
    {

        while (true)
        {
            // walking state
            currentState = EnemyState.patrolling;
            animator.SetBool("isWalking", true);
            Walk();
            yield return new WaitForSeconds(patrolWalkDuration);

            //waiting state
            currentState = EnemyState.waiting;
            body.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            yield return new WaitForSeconds(patrolBreakDuration);
        }
        
    }
    private void Walk()
    {
        if (IsFacingRight() && transform.position.x < patrolCenterXPosition + patrolDistance)
            {
                body.linearVelocity = new Vector2( speed, body.linearVelocity.y);
            }   
            else if (!IsFacingRight() && transform.position.x > patrolCenterXPosition - patrolDistance)
            {
                body.linearVelocity = new Vector2( -speed, body.linearVelocity.y);
            } 
            else
            {
                Flip();
            }
    }
    private bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }

    protected void Flip()
    {
        transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private bool IsCollidingWithEnvironment()
    {
        return environmentCollisionObject.GetComponent<KnightEnvironmentCollision>().isColliding;
    }

    private void ResetAfterCollisionWithEnvironmentFlip()
    {
        environmentCollisionObject.GetComponent<KnightEnvironmentCollision>().ResetAfterFlip();
    }
    
    private void GiveDamage()
    {
    if (attackScript != null && attackScript.player != null)
        {
            Health playerHealth = attackScript.player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isWalking", true);
        body.linearVelocity = new Vector2(speed, body.linearVelocity.y);
        attackScript.ResetAttackCooldown();
        attackScript.setIsWalking(true);
        //Flip();
    }

}
