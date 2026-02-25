using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthbar;
    [SerializeField] private Image currentHealthbar;
    [SerializeField] private float fillSpeed;
    private float targetFill;

    private void Start() {
        totalHealthbar.fillAmount = playerHealth.GetCurrentHealth() / 10;    
    }

    private void Update() {
        targetFill = playerHealth.GetCurrentHealth() / 10;
        currentHealthbar.fillAmount = Mathf.MoveTowards(currentHealthbar.fillAmount,
                                                        targetFill,
                                                        fillSpeed * Time.deltaTime);
    }
}
