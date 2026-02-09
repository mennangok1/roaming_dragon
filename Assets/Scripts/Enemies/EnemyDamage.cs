using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    [Header ("Audio")]
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private LayerMask PlayerHitboxLayer;


    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
        {
            other.GetComponentInParent<Health>().TakeDamage(damage);
            SoundManager.instance.PlaySound(impactSound);
        }
    }
}
