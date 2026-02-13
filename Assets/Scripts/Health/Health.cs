using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{

    [Header ("Health Parameters")]
    [SerializeField] protected float initialHealth;
    [SerializeField] public float currentHealth {get; protected set;}


    [Header("Audio")]
    [SerializeField] private AudioClip dieSound;
    [SerializeField] private AudioClip collectHeartSound;
    [SerializeField] private AudioClip bipSound;

    protected bool isDead;
    private bool isAtCheckpoint;

    protected Animator animator;
    private Rigidbody2D body;

    [Header("iFrames")]
    [SerializeField] private float invincibleDuration;
    [SerializeField] private int numOfFlashes;

    private float blinkWaitSeconds;

    private SpriteRenderer spriteRenderer;

    private bool isInvincible;


    protected void Awake() {
        currentHealth = initialHealth;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        blinkWaitSeconds = invincibleDuration / (2 * numOfFlashes);
        body = GetComponent<Rigidbody2D>();
        isDead = false;
    }

    public void TakeDamage(float _damage)
    {
        if (isInvincible) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, initialHealth);

        if (currentHealth > 0)
        {
            animator.SetTrigger("hurt");
            StartCoroutine(Invincibility());
        }
        else
        {
            if (!isDead)
            {
                SoundManager.instance.PlaySound(dieSound);
                animator.SetBool("isDead", true);
                isDead = true;
            }
        }
    }

    public void GainHealth( float gain )
    {
        SoundManager.instance.PlaySound(collectHeartSound);
        currentHealth = Mathf.Clamp(currentHealth + gain, 0, initialHealth);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }

    public bool isHealthFull()
    {
        return currentHealth == initialHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void DisableRigidbody()
    {
        body.simulated = false;
    }

    private void EnableRigidbody()
    {
        body.simulated = true;
    }

    protected IEnumerator Invincibility()

    {
        isInvincible = true;

        for (int i = 0; i < numOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1,0,0, 0.5f);
            if (!isAtCheckpoint)
            {
                SoundManager.instance.PlaySound(bipSound);
            }
            yield return new WaitForSeconds(blinkWaitSeconds);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkWaitSeconds);
        }

        isInvincible = false;
        isAtCheckpoint = false;
    }

    public void Respawn()
    {
        GainHealth(initialHealth);
        animator.SetBool("isDead", false);
        isDead = false;
        animator.Play("Idle");
        isAtCheckpoint = true;
        StartCoroutine(Invincibility());
        EnableRigidbody();
    }
}
