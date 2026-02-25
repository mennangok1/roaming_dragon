using UnityEngine;
using UnityEngine.UI;
public class ChargerBar : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] Image totalChargerBar;
    [SerializeField] Image currentChargerBar;
    [SerializeField] private float fillSpeed;
    private float targetFill;
    private float fullChargeCapacity;
    private PlayerAttack playerAttack;
    private Animator animator;

    private void Start()
    {
        playerAttack = player.GetComponent<PlayerAttack>();
        animator = GetComponent<Animator>();
        fullChargeCapacity = playerAttack.GetChargerCapacity();
        targetFill = playerAttack.GetCurrentFireballsAvailable() / fullChargeCapacity;
        totalChargerBar.fillAmount = targetFill;
    }

    private void Update()
    {
        targetFill = playerAttack.GetCurrentFireballsAvailable() / fullChargeCapacity;

        currentChargerBar.fillAmount = Mathf.MoveTowards(currentChargerBar.fillAmount,
                                                        targetFill,
                                                        fillSpeed * Time.deltaTime);
        animator.SetBool("isFull", playerAttack.IsChargerFull());
    }
}
