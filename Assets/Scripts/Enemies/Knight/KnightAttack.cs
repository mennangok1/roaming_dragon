using UnityEngine;
/*
The only functionality of this script is to detect when the player is colliding with the circle collider 2d.
The damage mechanism is handled in KnightEnemy script
*/
public class KnightAttack: MonoBehaviour
{
    [SerializeField] private float attackDelay;
    private float attackCooldown;
    private CircleCollider2D collider;
    [SerializeField] private LayerMask playerLayer;
    public GameObject player {get; private set;}

    public bool isPlayerInRange {get; private set;}
    private KnightEnemy enemy;

    private void Awake() {
        attackCooldown = attackDelay;
        collider = GetComponent<CircleCollider2D>();
        enemy = GetComponentInParent<KnightEnemy>();
    }
    private void Update() {
        isPlayerInRange = collider.IsTouchingLayers(playerLayer);
        attackCooldown += Time.deltaTime;
    }

    private void TryAttack(Collider2D other) {
        if (attackCooldown > attackDelay && !other.GetComponentInParent<Health>().IsDead())
        {
            player = other.transform.parent.gameObject;
            enemy.StartAttack();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy.GetCurrentState() == KnightEnemy.EnemyState.Dead) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
        {
            TryAttack(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (enemy.GetCurrentState() == KnightEnemy.EnemyState.Dead) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
        {
            TryAttack(other);
        }
    }

    public void ResetAttackCooldown()
    {
        attackCooldown = 0;
    }

}