using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    [Header ("Audio")]
    [SerializeField] private AudioClip impactSound;

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
        {
            other.GetComponent<Health>().TakeDamage(damage);
            SoundManager.instance.PlaySound(impactSound);
        }
    }
}
