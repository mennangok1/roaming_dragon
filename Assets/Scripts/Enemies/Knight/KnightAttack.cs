using UnityEngine;

public class KnightAttack: MonoBehaviour
{
    [SerializeField] private float attackDelay;
    private float attackCooldown;
    private Animator animator;
    private CircleCollider2D collider;
    [SerializeField] private LayerMask playerLayer;
    public GameObject player {get; private set;}

    private bool isAttacking;
    private bool isWalking;
    public bool isPlayerInRange {get; private set;}

    private void Awake() {
        attackCooldown = 0;
        animator = GetComponentInParent<Animator>();
        collider = GetComponent<CircleCollider2D>();
        isWalking = true;
    }
    private void Update() {
        isPlayerInRange = collider.IsTouchingLayers(playerLayer);
        attackCooldown += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && attackCooldown > attackDelay)
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("isWalking", false);
            isWalking = false;
            player = other.gameObject;
        }
    }

    public void ResetAttackCooldown()
    {
        attackCooldown = 0;
    }

    public bool getIsWalking()
    {
        return isWalking;
    }
    public void setIsWalking(bool _isWalking)
    {
        isWalking = _isWalking;
    }

}