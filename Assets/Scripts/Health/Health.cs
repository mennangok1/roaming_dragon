using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{

    [Header ("Health Parameters")]
    [SerializeField] protected float initialHealth;
    [SerializeField] protected float initialGlobalHealth;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float currentGlobalHealth;


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

    private Player player;


    protected void Awake() {
        currentHealth = initialHealth;
        currentGlobalHealth = initialGlobalHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        blinkWaitSeconds = invincibleDuration / (2 * numOfFlashes);
        body = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    public void TakeDamage(float _damage)
    {
        if (player.currentPowerUpState == Player.PlayerPowerUpState.Invincible) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, initialHealth);

        if (currentHealth > 0)
        {
            animator.SetTrigger("hurt");
            StartCoroutine(Invincibility());
        }
        else
        {
            if (player.currentMovementState != Player.PlayerMovementState.Dead)
            {
                SoundManager.instance.PlaySound(dieSound);
                animator.SetBool("isDead", true);
                player.ChangeMovementState(Player.PlayerMovementState.Dead);
                currentGlobalHealth -= 1;
            }
        }
    }

    public void GainHealth( float gain )
    {
        SoundManager.instance.PlaySound(collectHeartSound);
        currentHealth = Mathf.Clamp(currentHealth + gain, 0, initialHealth);
    }
    public void GainGlobalHealth( float gain )
    {
        SoundManager.instance.PlaySound(collectHeartSound);
        currentGlobalHealth += gain;
    }

    public bool isHealthFull()
    {
        return currentHealth == initialHealth;
    }

    public bool IsDead()
    {
        return player.currentMovementState == Player.PlayerMovementState.Dead;
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
        player.ChangePowerUpState(Player.PlayerPowerUpState.Invincible);

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

        player.ChangePowerUpState(Player.PlayerPowerUpState.Normal);
        isAtCheckpoint = false;
    }

    public void Respawn()
    {
        GainHealth(initialHealth);
        animator.SetBool("isDead", false);
        player.DetermineMovementStateAfterRecoilAndRespawn();
        animator.Play("Idle");
        isAtCheckpoint = true;
        StartCoroutine(Invincibility());
        EnableRigidbody();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    public float GetCurrentGlobalHealth()
    {
        return currentGlobalHealth;
    }


}
