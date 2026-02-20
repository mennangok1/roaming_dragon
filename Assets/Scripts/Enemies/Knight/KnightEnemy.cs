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



    public enum EnemyState {Walking, Waiting, Dead, Dizzy, Recoiling, Attacking}
    private EnemyState currentState;

    private KnightEnvironmentCollision environmentCollisionScript;
    private KnightAttack attackScript;

    [Header("Audio")]
    [SerializeField] private AudioClip swordAttackSound;
    [SerializeField] private AudioClip dizzySound;
    [SerializeField] private AudioClip impactSound;



    [SerializeField] private float dizzyDuration;
    [SerializeField] private float projectileRecoilForce = 5f;
    [SerializeField] private float projectileRecoilDuration = 0.2f;
    private Coroutine recoilRoutine;

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
        if (currentState == EnemyState.Walking)
        {
            Walk();
        }
        else if (currentState == EnemyState.Attacking || currentState == EnemyState.Dizzy || currentState == EnemyState.Dead)
        {
            Stop(); 
        }
        if(IsCollidingWithEnvironment())
        {
            Flip();
            ResetAfterCollisionWithEnvironmentFlip();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = attackScript.player.GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            currentState = EnemyState.Walking;
            animator.SetBool("isWalking", true);
            Walk();
            float countdown = 0f;
            while (countdown < patrolWalkDuration && currentState == EnemyState.Walking)
            {
                countdown += Time.deltaTime;
                yield return null;
                continue;
            }

            //waiting state
            currentState = EnemyState.Waiting;
            Stop();
            animator.SetBool("isWalking", false);
            countdown = 0f;
            while (countdown < patrolBreakDuration && currentState == EnemyState.Waiting)
            {
                countdown += Time.deltaTime;
                yield return null;
                continue;
            }
        }
    }

    public IEnumerator FeelDizzy()
    {
        if (currentState == EnemyState.Dizzy) yield break;

        if (patrolRoutine != null )
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }
        SoundManager.instance.PlaySound(dizzySound);
        currentState = EnemyState.Dizzy;
        Stop();
        animator.SetBool("isFeelingDizzy", true);
        animator.SetBool("isWalking", false);

        yield return new WaitForSeconds(dizzyDuration);

        animator.SetBool("isFeelingDizzy", false);
        animator.SetBool("isWalking", true);
        body.linearVelocity = new Vector2(speed, 0);
        currentState = EnemyState.Walking;
        patrolRoutine = StartCoroutine(Patrol());
    }

    private void Walk()
    {
        if (currentState == EnemyState.Dizzy || currentState == EnemyState.Dead) return;
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

    public void Stop()
    {
        body.linearVelocity = Vector2.zero;
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
        if (currentState == EnemyState.Dizzy || currentState == EnemyState.Dead) return;
        if (patrolRoutine != null )
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }
        currentState = EnemyState.Attacking;
        animator.SetBool("isAttacking", true);
        animator.SetBool("isWalking", false);
        Stop();
}


    public void GiveDamage()
    {
        SoundManager.instance.PlaySound(swordAttackSound);
        if (attackScript != null && attackScript.player != null)
        {
            Health playerHealth = attackScript.player.GetComponent<Health>();
            if (playerHealth != null && attackScript.isPlayerInRange)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public void EndAttack()
    {
        currentState = EnemyState.Walking;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
        body.linearVelocity = new Vector2(speed, body.linearVelocity.y);
        attackScript.ResetAttackCooldown();
        Flip();
        patrolRoutine = StartCoroutine(Patrol());
    }

    public void SetCurrentState(EnemyState state)
    {
        currentState = state;
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }

    private void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    private void DisableComponents()
    {
        collider.enabled = false;
        body.simulated = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    public void ApplyProjectileRecoil(Vector2 hitPoint)
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }

        if (recoilRoutine != null)
        {
            StopCoroutine(recoilRoutine);
        }

        recoilRoutine = StartCoroutine(RecoilRoutine(hitPoint.x));
    }

    private IEnumerator RecoilRoutine(float hitPointX)
    {
        if (patrolRoutine != null)
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }

        currentState = EnemyState.Recoiling;
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
        Stop();

        float recoilDirectionX = Mathf.Sign(transform.position.x - hitPointX);
        if (recoilDirectionX == 0f)
        {
            recoilDirectionX = IsFacingRight() ? -1f : 1f;
        }

        Vector2 recoilDir = new Vector2(recoilDirectionX, 0f);
        body.AddForce(recoilDir * projectileRecoilForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(projectileRecoilDuration);

        if (currentState != EnemyState.Recoiling)
        {
            recoilRoutine = null;
            yield break;
        }

        currentState = EnemyState.Walking;
        animator.SetBool("isWalking", true);
        patrolRoutine = StartCoroutine(Patrol());
        recoilRoutine = null;
    }
    

}
