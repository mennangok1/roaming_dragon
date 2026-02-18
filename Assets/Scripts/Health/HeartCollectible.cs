using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float heartGain;
    [SerializeField] private bool isGlobalHealth;

    private void Awake() {
        heartGain = 1;
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.tag == "Player")
        {
            if (!isGlobalHealth && !trigger.GetComponentInParent<Health>().isHealthFull())
            {
                trigger.GetComponentInParent<Health>().GainHealth(heartGain);
                gameObject.SetActive(false);
            }
            else if (isGlobalHealth)
            {
                trigger.GetComponentInParent<Health>().GainGlobalHealth(heartGain);
                gameObject.SetActive(false);
            }
        
        }        
    }
}
