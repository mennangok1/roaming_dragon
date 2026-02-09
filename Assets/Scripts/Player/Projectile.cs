using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private float damage;
    private float lifetimeCounter;

    private bool hit;

    private float direction;

    private BoxCollider2D boxCollider;
    private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip impactSound;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        speed = 10f;
        lifetimeCounter = 0f;
        lifetime = 5f;
    }

    private void Update() {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0 );
        lifetimeCounter += Time.deltaTime;

        if (lifetimeCounter > lifetime)
        {
            animator.SetTrigger("explode");
            gameObject.SetActive(false);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyHitbox"))
        {
            hit = true;
            boxCollider.enabled = false;
            animator.SetTrigger("explode");
            collision.GetComponentInParent<EnemyHealth>().TakeDamage(damage);
            SoundManager.instance.PlaySound(impactSound);
        
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            boxCollider.enabled = false;
            animator.SetTrigger("explode");
            SoundManager.instance.PlaySound(impactSound);
        }
    }

    public void SetDirection(float _direction)
    {
        lifetimeCounter = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        } 

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false); 
    }
}
