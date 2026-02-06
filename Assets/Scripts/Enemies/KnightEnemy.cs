using UnityEngine;
using System.Collections;

public class KnightEnemy : MonoBehaviour {
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
    


    private void Awake() {
        patrolCenterXPosition = transform.position.x;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();

        patrolRoutine = StartCoroutine(Patrol());
    }
    private void Update()
    {

        if (currentState == EnemyState.patrolling)
        {
            Walk();
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

    private void Flip()
    {
        transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    
}
