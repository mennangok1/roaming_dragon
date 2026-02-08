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

    public enum EnemyState {patrolling, waiting, chasing, attacking, dizzy}
    private EnemyState currentState;
    
    private KnightEnvironmentCollision environmentCollisionScript;
    private KnightAttack attackScript;



    [SerializeField] private float dizzyDuration;

    protected void Awake() {
        // set patrol center, get rigidbody, animator, and collider components
        patrolCenterXPosition = transform.position.x;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();

        // get the environment collision and attack scripts in the child objects
        environmentCollisionScript = transform.Find("KnightEnvironmentCollision")
                                                .gameObject
                                                .GetComponent<KnightEnvironmentCollision>();
        attackScript = transform.Find("KnightAttack")
                                .gameObject
                                .GetComponent<KnightAttack>();

        patrolRoutine = StartCoroutine(Patrol());


    }
    private void Update()
    {
        Debug.Log(currentState);
        if (currentState == EnemyState.patrolling)
        {
            Walk();
        }
        else if (currentState == EnemyState.attacking || currentState == EnemyState.dizzy)
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

    public IEnumerator FeelDizzy()
    {
        currentState = EnemyState.dizzy;
        Debug.Log("inside FeelDizzy()");
        body.linearVelocity = Vector2.zero;
        animator.SetBool("isFeelingDizzy", true);
        animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(dizzyDuration);

        animator.SetBool("isFeelingDizzy", false);
        animator.SetBool("isWalking", true);
        body.linearVelocity = new Vector2(speed, 0);
        currentState = EnemyState.patrolling;

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
        return environmentCollisionScript.isColliding;
    }

    private void ResetAfterCollisionWithEnvironmentFlip()
    {
        environmentCollisionScript.ResetAfterFlip();
    }
    
    public void StartAttack()
    {
        Debug.Log("Inside StartAttack()");
        if (patrolRoutine != null )
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }
        currentState = EnemyState.attacking;
        animator.SetBool("isAttacking", true);
        animator.SetBool("isWalking", false);
        body.linearVelocity = Vector2.zero;
        Debug.Log("End of StartAttack()");
}


    public void GiveDamage()
    {
        Debug.Log("Inside GiveDamage()");
        if (attackScript != null && attackScript.player != null)
        {
            Debug.Log("Inside If, before playerHealth init");
            Health playerHealth = attackScript.player.GetComponent<Health>();
            Debug.Log("After playerHealth init");
            if (playerHealth != null && attackScript.isPlayerInRange)
            {
                Debug.Log("Before playerHealth Take damage");
                playerHealth.TakeDamage(damage);
            }
        }
        else
        {
            if (attackScript == null)
            {
                Debug.Log("attack script is null");
            }
            else
            {
                Debug.Log("player is null");
            }
        }
        Debug.Log("End of GiveDamage");
    }

    public void EndAttack()
    {
        Debug.Log("Inside EndAttack()");
        currentState = EnemyState.patrolling;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
        body.linearVelocity = new Vector2(speed, body.linearVelocity.y);
        attackScript.ResetAttackCooldown();
        patrolRoutine = StartCoroutine(Patrol());
        Flip();
        Debug.Log("End of EndAttack()");
    }

    public void SetCurrentState(EnemyState state)
    {
        currentState = state;
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }



}
