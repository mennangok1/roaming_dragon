using UnityEngine;
using System.Collections;
public class FireTrap : MonoBehaviour
{

    [Header ("Firetrap Timers")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float activeTime;
    [SerializeField] private int numOfFlashes;


    [SerializeField] private float damage;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isTriggered;
    private bool isActive;

    private float blinkWaitSeconds;


    
    [Header("Audio")]
    [SerializeField] private AudioClip fireTrapSound;
    [SerializeField] private AudioClip blinkSound;

    
    private void Awake() {

     blinkWaitSeconds = activationDelay / (2 * numOfFlashes);   
    
    }
    private void Update() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
        {
            
            // if the trap is not triggered, then trigger and activate it
            if (!isTriggered)
            {
                StartCoroutine(ActivateFiretrap());
            }
            // if active, hurt the player
            if (isActive)
            {
                other.GetComponent<Health>().TakeDamage(damage);
            }
        }
    }


    private IEnumerator ActivateFiretrap()
    {
        isTriggered = true;
        
        for (int i = 0; i < numOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1,0,0, 0.5f);
            SoundManager.instance.PlaySound(blinkSound);
            yield return new WaitForSeconds(blinkWaitSeconds);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkWaitSeconds);
        }
        isActive = true;
        animator.SetBool("isActive", isActive);
        SoundManager.instance.PlaySound(fireTrapSound);
        yield return new WaitForSeconds(activeTime);
        isActive = false;
        animator.SetBool("isActive", isActive);
        isTriggered = false;

    }

}
