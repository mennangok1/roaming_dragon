using UnityEngine;

public class Health1 : MonoBehaviour
{
    [SerializeField] private float heartGain;

    private void Awake() {
        heartGain = 1;
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.tag == "Player" && !trigger.GetComponent<Health>().isHealthFull())
        {
            trigger.GetComponent<Health>().GainHealth(heartGain);
            gameObject.SetActive(false);
        
        }        
    }
}
