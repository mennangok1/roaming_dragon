using UnityEngine;
using UnityEngine.UI;

public class GlobalHealthText : MonoBehaviour
{
    private Text txt;
    [SerializeField] private GameObject player;

    private void Awake() {
        txt = GetComponent<Text>();
    }

    private void Update() {
        txt.text = player.GetComponent<Health>().GetCurrentGlobalHealth().ToString();
    }
}
