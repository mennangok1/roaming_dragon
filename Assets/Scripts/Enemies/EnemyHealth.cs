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
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, initialHealth);

        if (currentHealth > 0)
        {
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
