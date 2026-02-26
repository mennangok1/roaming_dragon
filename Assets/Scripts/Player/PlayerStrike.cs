using UnityEngine;
using System.Collections;

public class PlayerStrike : MonoBehaviour
{


    [SerializeField] private float strikeCooldown;

    [SerializeField] private float damage;

    public bool canDamage = false;
    private PlayerAttack playerAttack;
    private Player playerScript;
    private BoxCollider2D strikeCollider;
    private Animator animator;
    [SerializeField] private AudioClip strikeImpactSound;
    private GameObject player;
    private Vector3 target;
    private void Start()
    {
        player = transform.parent.gameObject;
        playerAttack = player.GetComponent<PlayerAttack>();
        playerScript = player.GetComponent<Player>();
        animator = player.GetComponent<Animator>();
        strikeCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        strikeCollider.enabled = canDamage;
        if (Input.GetKeyDown(KeyCode.R) && playerAttack.GetCurrentActionState() != PlayerAttack.PlayerActionState.Striking)
        {
            StartStrike();
        }
        animator.SetBool("isStriking", playerAttack.GetCurrentActionState() == PlayerAttack.PlayerActionState.Striking);    
    }


    private void StartStrike()
    {
        playerAttack.SetCurrentActionState(PlayerAttack.PlayerActionState.Striking);
    }



    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyHitbox"))
        {
            canDamage = false;
            EnemyHealth enemyHealth = collider.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, transform.position);
                SoundManager.instance.PlaySound(strikeImpactSound);
            }
        }
    }



}
