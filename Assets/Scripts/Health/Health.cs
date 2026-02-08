using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{

    [Header ("Health Parameters")]
    [SerializeField] protected float initialHealth;
    [SerializeField] public float currentHealth {get; protected set;}


    protected bool isDead;

    protected Animator animator;

    [Header("iFrames")]
    [SerializeField] private float invincibleDuration;
    [SerializeField] private int numOfFlashes;

    private float blinkWaitSeconds;

    private SpriteRenderer spriteRenderer;


    protected void Awake() {
        currentHealth = initialHealth;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        blinkWaitSeconds = invincibleDuration / (2 * numOfFlashes);
        isDead = false;
    }

    public void TakeDamage(float _damage)
    {
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
                animator.SetTrigger("die");
                animator.SetBool("isDead", true);
                GetComponent<Player>().enabled = false;
                isDead = true;
            }
        }
    }

    public void GainHealth( float gain )
    {
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


    protected IEnumerator Invincibility()

    {
        Physics2D.IgnoreLayerCollision(9, 10, true);

        for (int i = 0; i < numOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1,0,0, 0.5f);
            yield return new WaitForSeconds(blinkWaitSeconds);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkWaitSeconds);
        }

        Physics2D.IgnoreLayerCollision(9, 10, false);
    }
}
