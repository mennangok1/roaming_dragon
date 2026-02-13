using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireBalls;

    [Header("Audio")]
    [SerializeField] private AudioClip fireballSound;

    private Rigidbody2D body;

    private float cooldownTimer = Mathf.Infinity;
    private Animator animator;
    private Player player;


    [Header ("Recoil")]
    public bool isRecoiling = false;

    [SerializeField] private float recoilDuration = 2f;
    [SerializeField] private float recoilForce = 5f;

    private void Awake() {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        body = GetComponent<Rigidbody2D>();

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

        StartCoroutine(RecoilRoutine());

    }


    IEnumerator RecoilRoutine()
    {
        isRecoiling = true;
        Vector2 recoilDir = new Vector2(-transform.localScale.x, 0);

        body.AddForce(recoilDir * recoilForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(recoilDuration);
        isRecoiling = false;
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
