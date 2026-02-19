using UnityEngine;
using System.Collections;
public class EnemyHealth : Health
{
    [SerializeField] private float feelDizzyIfHealthIsBelow;
    [SerializeField] private ParticleSystem damageParticles;
    [SerializeField] private float particleOffsetX = 2f;

    private ParticleSystem damageParticlesInstance;
    private SpriteRenderer renderer;

    private KnightEnemy knightEnemy;

    protected void Awake() {
        base.Awake();
        feelDizzyIfHealthIsBelow = 1;
        knightEnemy = GetComponent<KnightEnemy>();
        renderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        ApplyDamage(_damage, false, Vector2.zero);
    }

    public void TakeDamage(float _damage, Vector2 hitPoint)
    {
        ApplyDamage(_damage, true, hitPoint);
    }

    private void ApplyDamage(float _damage, bool applyRecoil, Vector2 hitPoint)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, initialHealth);

        SpawnDamageParticles(hitPoint);

        if (currentHealth > 0)
        {
            if (applyRecoil)
            {
                knightEnemy.ApplyProjectileRecoil(hitPoint);
            }

            if (currentHealth <= feelDizzyIfHealthIsBelow && knightEnemy.GetCurrentState() != KnightEnemy.EnemyState.Dizzy)
            {
                StartCoroutine(knightEnemy.FeelDizzy());
            }
            StartCoroutine(base.Invincibility());
        }
        else
        {
            if (knightEnemy.GetCurrentState() != KnightEnemy.EnemyState.Dead)
            {
                knightEnemy.Stop();
                knightEnemy.SetCurrentState(KnightEnemy.EnemyState.Dead);
                animator.SetBool("isDead", true);
                animator.SetBool("isFeelingDizzy", false);
            }
        }
    }


    private void SpawnDamageParticles(Vector2 hitpoint)
    {
        float direction = hitpoint.x - transform.position.x;
        Quaternion rotation;
        Vector3 particleLocation;
        if (direction > 0)
        {
            // so the attack is coming from right, then flip the effect
            rotation = Quaternion.Euler(0, 180, 0);
            particleLocation = new Vector3(hitpoint.x - particleOffsetX, hitpoint.y, 0);
        }
        else
        {
            rotation = Quaternion.identity;
            particleLocation = new Vector3(hitpoint.x + particleOffsetX, hitpoint.y, 0);
        }
        damageParticlesInstance = Instantiate(damageParticles, particleLocation, rotation);
    }



}
