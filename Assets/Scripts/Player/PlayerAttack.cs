using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireBalls;

    [Header("Audio")]
    [SerializeField] private AudioClip fireballSound;

    private float cooldownTimer = Mathf.Infinity;
    private Animator animator;
    private Player player;

    private void Awake() {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer > attackCooldown && player.canAttack())
        {
            Attack();
        }

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        SoundManager.instance.PlaySound(fireballSound);
        animator.SetTrigger("attack");
        cooldownTimer = 0;   

        //pool fireballs
        int availableFireballIndex = FindFireball();
        fireBalls[ availableFireballIndex ].transform.position = firePoint.position;
        fireBalls[ availableFireballIndex ].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));

    }


    private int FindFireball()
    {

        for ( int i = 0; i < fireBalls.Length; i++ )
        {
            if (!fireBalls[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
}
