using UnityEngine;
using System.Collections;
public class EnemyHealth : Health
{
    [SerializeField] private float feelDizzyIfHealthIsBelow;

    private KnightEnemy knightEnemy;

    protected void Awake() {
        base.Awake();
        feelDizzyIfHealthIsBelow = 1;
        knightEnemy = GetComponent<KnightEnemy>();
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

        if (currentHealth > 0)
        {
            if (applyRecoil)
            {
                knightEnemy.ApplyProjectileRecoil(hitPoint);
            }

            if (currentHealth <= feelDizzyIfHealthIsBelow && knightEnemy.GetCurrentState() != KnightEnemy.EnemyState.dizzy)
            {
                Debug.Log("Feel dizzy set current state");
                StartCoroutine(knightEnemy.FeelDizzy());
            }
            StartCoroutine(base.Invincibility());
        }
        else
        {
            if (!isDead)
            {
                knightEnemy.Stop();
                knightEnemy.SetCurrentState(KnightEnemy.EnemyState.dead);
                isDead = true;
                animator.SetBool("isDead", true);
                animator.SetBool("isFeelingDizzy", false);
            }
        }
    }



}
