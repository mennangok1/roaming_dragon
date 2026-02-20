using UnityEngine;

public abstract class HealthBase : MonoBehaviour
{
    [SerializeField] protected float initialHealth;
    [SerializeField] protected float currentHealth;


    protected virtual void Awake() {
        currentHealth = initialHealth;
    }

    public virtual void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
    }

    public bool IsHealthFull()
    {
        return currentHealth == initialHealth;
    }

    public bool IsDead()
    {
        return currentHealth < 0;
    }


    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public virtual void GainHealth(float gain)
    {
        currentHealth = Mathf.Clamp(currentHealth + gain, 0, initialHealth);
    }

}
