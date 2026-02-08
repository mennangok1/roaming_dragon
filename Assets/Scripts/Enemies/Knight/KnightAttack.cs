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

    private bool isAttacking;
    public bool isPlayerInRange {get; private set;}

    private void Awake() {
        attackCooldown = attackDelay;
        collider = GetComponent<CircleCollider2D>();
    }
    private void Update() {
        isPlayerInRange = collider.IsTouchingLayers(playerLayer);
        attackCooldown += Time.deltaTime;
        if (isPlayerInRange)
        {
            Debug.Log("player is in range");
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && attackCooldown > attackDelay)
        {
            KnightEnemy enemy = GetComponentInParent<KnightEnemy>();
            player = other.gameObject;
            enemy.StartAttack();
        }

    }

    public void ResetAttackCooldown()
    {
        attackCooldown = 0;
    }

}