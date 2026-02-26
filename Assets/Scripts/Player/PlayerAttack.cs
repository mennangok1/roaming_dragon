using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{

    [Header ("Attack")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireBalls;
    [SerializeField] private float chargerCapacity;
    [SerializeField] private float fillChargerCooldown;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float currentFireballsAvailable = 3f;

    private Coroutine chargerRoutine;

    [Header("Strike")]
    [SerializeField] private float flyForce;
    [SerializeField] private float riseDuration;
    private Vector3 velocity = Vector3.zero;

    [Header("Audio")]
    [SerializeField] private AudioClip fireballSound;
    [SerializeField] private AudioClip strikeStartSound;


    private Rigidbody2D body;

    private float cooldownTimer = Mathf.Infinity;
    private Animator animator;
    private Player player;


    [Header ("Recoil")]

    [SerializeField] private float recoilDuration = 0.12f;
    [SerializeField] private float recoilKickSpeed = 6f;
    [SerializeField] private float groundRecoilDamping = 80f;
    [SerializeField] private float airRecoilDamping = 25f;
    private Coroutine recoilRoutine;
    

    public enum PlayerActionState {None, Attacking, Striking}
    public PlayerActionState currentActionState {get; private set;} = PlayerActionState.None;

    private PlayerStrike playerStrike;
    private void Awake() {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        body = GetComponent<Rigidbody2D>();
        currentFireballsAvailable = chargerCapacity;
        playerStrike = transform.Find("StrikeHitbox").GetComponent<PlayerStrike>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer > attackCooldown && CanAttack())
        {
            Attack();
        }
        if (!IsChargerFull() && chargerRoutine == null)
        {
            chargerRoutine = StartCoroutine(FillChargerRoutine());
        }
        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        if (IsChargerEmpty())
        {
          HandleCannotAttack();
          return;  
        } 

        SoundManager.instance.PlaySound(fireballSound);
        animator.SetTrigger("attack");
        cooldownTimer = 0;   

        //pool fireballs
        currentFireballsAvailable = Mathf.Clamp(currentFireballsAvailable - 1, 0, chargerCapacity);
        int availableFireballIndex = FindFireball();
        fireBalls[ availableFireballIndex ].transform.position = firePoint.position;
        fireBalls[ availableFireballIndex ].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));

        if (recoilRoutine != null)
        {
            StopCoroutine(recoilRoutine);
        }

        recoilRoutine = StartCoroutine(RecoilRoutine());

    }

    private bool IsChargerEmpty()
    {
        return currentFireballsAvailable == 0;
    }

    public bool IsChargerFull()
    {
        return currentFireballsAvailable == chargerCapacity;
    }
    private void HandleCannotAttack()
    {
        
    }

    IEnumerator FillChargerRoutine()
    {
        if (IsChargerFull()) yield break;
        yield return new WaitForSeconds(fillChargerCooldown);
        currentFireballsAvailable = Mathf.Clamp(currentFireballsAvailable + 1, 0, chargerCapacity);
        chargerRoutine = null;
    }
    IEnumerator RecoilRoutine()
    {
        player.ChangeMovementState(Player.PlayerMovementState.Stunned);
        float recoilDirectionX = -Mathf.Sign(transform.localScale.x);
        if (recoilDirectionX == 0f)
        {
            recoilDirectionX = -1f;
        }

        // Initial kick: gives the recoil an immediate "impact" feel.
        body.linearVelocity = new Vector2(body.linearVelocity.x + (recoilDirectionX * recoilKickSpeed), body.linearVelocity.y);

        float elapsed = 0f;
        while (elapsed < recoilDuration)
        {
            float damping = player.currentLocationState == Player.PlayerLocationState.OnGround ? groundRecoilDamping : airRecoilDamping;
            float newVelocityX = Mathf.MoveTowards(body.linearVelocity.x, 0f, damping * Time.fixedDeltaTime);
            body.linearVelocity = new Vector2(newVelocityX, body.linearVelocity.y);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        player.DetermineMovementStateAfterRecoilAndRespawn();
        recoilRoutine = null;
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


    public bool CanAttack()
    {
        return player.currentLocationState != Player.PlayerLocationState.OnWall;
    }

    public float GetChargerCapacity()
    {
        return chargerCapacity;
    }

    public float GetCurrentFireballsAvailable()
    {
        return currentFireballsAvailable;
    }


    public void SetCurrentActionState(PlayerActionState actionState)
    {
        currentActionState = actionState;
    }

    public PlayerActionState GetCurrentActionState()
    {
        return currentActionState;
    }


    #region Strike
    
    public void Rise()
    {
        // will be called by the animator when the dragon starts to fly
        Vector3 target = new Vector3(transform.position.x, transform.position.y + flyForce, transform.position.z);
        SoundManager.instance.PlaySound(strikeStartSound);
        transform.position = Vector3.SmoothDamp(transform.position,
                                                 target,
                                                 ref velocity,
                                                 riseDuration);

        playerStrike.canDamage = true;
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);
    }
    public void Land()
    {
        playerStrike.canDamage = false;
    }
    public void EndStrike()
    {
        SetCurrentActionState(PlayerActionState.None);
    }
    #endregion
}
