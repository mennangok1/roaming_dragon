using UnityEngine;
using System.Collections;

public class KnightEnemy : MonoBehaviour {
    [SerializeField] private float damage;
    private Vector3 destination;
    [SerializeField] private float speed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float chaseDuration;
    [SerializeField] private float blockedChaseCooldown = 0.35f;
    [SerializeField] private float patrolDistance;

    [SerializeField] private float patrolWalkDuration;
    [SerializeField] private float patrolBreakDuration;
    private Coroutine patrolRoutine;

    private Animator animator;
    private Rigidbody2D body;
    private BoxCollider2D collider;
    private SpriteRenderer renderer;

    private float patrolCenterXPosition;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;



    public enum EnemyState {Walking, Waiting, Dead, Dizzy, Recoiling, Attacking, Chasing}
    private EnemyState currentState;

    private KnightEnvironmentCollision environmentCollisionScript;
    private KnightAttack attackScript;

    [Header("Audio")]
    [SerializeField] private AudioClip swordAttackSound;
    [SerializeField] private AudioClip dizzySound;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioClip detectSound;
    [SerializeField] [Range(0,1)] private float detectSoundVolume;

    private GameObject player;

    [SerializeField] private float dizzyDuration;
    [SerializeField] private float projectileRecoilForce = 5f;
    [SerializeField] private float projectileRecoilDuration = 0.2f;
    private Coroutine recoilRoutine;
    private Coroutine chaseRoutine;
    private bool wasCollidingWithEnvironment;
    private float chaseBlockedUntilTime;
    private bool isChaseBlockedByEnvironment;

    protected void Awake() {
        // set patrol center, get rigidbody, animator, and collider components
        patrolCenterXPosition = transform.position.x;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        renderer = transform.Find("DetectionFX").GetComponent<SpriteRenderer>();
        renderer.enabled = false;

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
        else if (currentState == EnemyState.Chasing)
        {
            if (chaseRoutine == null)
            {
                chaseRoutine = StartCoroutine(ChaseRoutine());
            }

        }
        bool isCollidingWithEnvironment = IsCollidingWithEnvironment();
        if (isCollidingWithEnvironment && !wasCollidingWithEnvironment)
        {
            if (currentState == EnemyState.Chasing)
            {
                isChaseBlockedByEnvironment = true;
                Stop();
                animator.SetBool("isWalking", false);
            }
            else if (currentState == EnemyState.Walking)
            {
                Flip();
                ResetAfterCollisionWithEnvironmentFlip();
            }
        }
        wasCollidingWithEnvironment = isCollidingWithEnvironment;
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
        if (chaseRoutine != null)
        {
            StopCoroutine(chaseRoutine);
            chaseRoutine = null;
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

    public void StartChase()
    {
        if (Time.time < chaseBlockedUntilTime)
        {
            return;
        }

        if (currentState == EnemyState.Dead || currentState == EnemyState.Dizzy || currentState == EnemyState.Recoiling)
        {
            return;
        }

        if (currentState == EnemyState.Attacking || currentState == EnemyState.Chasing)
        {
            return;
        }

        if (patrolRoutine != null)
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }

        currentState = EnemyState.Chasing;
        isChaseBlockedByEnvironment = false;
        renderer.enabled = true;
        SoundManager.instance.PlaySound(detectSound, detectSoundVolume);
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    public void StopChaseAndResumePatrol(bool blockedByEnvironment = false)
    {
        if (currentState != EnemyState.Chasing)
        {
            return;
        }

        bool shouldResolveBlockedCollision = isChaseBlockedByEnvironment;
        if (blockedByEnvironment || shouldResolveBlockedCollision)
        {
            chaseBlockedUntilTime = Time.time + blockedChaseCooldown;
        }

        if (shouldResolveBlockedCollision)
        {
            Flip();
            ResetAfterCollisionWithEnvironmentFlip();
            wasCollidingWithEnvironment = false;
        }

        isChaseBlockedByEnvironment = false;
        renderer.enabled = false;
        if (chaseRoutine != null)
        {
            StopCoroutine(chaseRoutine);
            chaseRoutine = null;
        }

        Stop();
        currentState = EnemyState.Walking;
        animator.SetBool("isWalking", true);

        if (patrolRoutine == null)
        {
            patrolRoutine = StartCoroutine(Patrol());
        }
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

    private IEnumerator ChaseRoutine()
    {
        if (patrolRoutine != null)
        {
            StopCoroutine(patrolRoutine);
            patrolRoutine = null;
        }
        float countdown = 0f;
        while (countdown < chaseDuration && currentState == EnemyState.Chasing)
        {
            if (player == null)
            {
                StopChaseAndResumePatrol();
                yield break;
            }

            if (isChaseBlockedByEnvironment)
            {
                Stop();
                animator.SetBool("isWalking", false);
            }
            else if (IsFacingRight() && IsPlayerToTheRight())
            {
                animator.SetBool("isWalking", true);
                body.linearVelocity = new Vector2( chaseSpeed, body.linearVelocity.y);
            }   
            else if (!IsFacingRight() && !IsPlayerToTheRight())
            {
                animator.SetBool("isWalking", true);
                body.linearVelocity = new Vector2( -chaseSpeed, body.linearVelocity.y);
            } 
            else
            {
                Flip();
            }
            countdown += Time.deltaTime;
            yield return null;
        }

        if (currentState == EnemyState.Chasing)
        {
            StopChaseAndResumePatrol(isChaseBlockedByEnvironment);
        }
        chaseRoutine = null;
        
    }
    

    private bool IsPlayerToTheRight()
    {
        return transform.position.x <  player.transform.position.x;
    }

    private void RunToPosition(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
    }
}
