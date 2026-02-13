using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private RectTransform[] options;
    [SerializeField] private AudioClip toggleOptionSound;
    [SerializeField] private AudioClip chooseOptionSound;


    private int currentPosition;

    private void Awake() {
        rect = GetComponent<RectTransform>();
    }
    private void Update() {

        // Toggle between options
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(-1);
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(1);
        }

        // Choose an option
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
        {
            ChooseOption();
        }
    }
    private void ChangePosition(int change)
    {
        currentPosition += change;
        if (change != 0)
        {
            SoundManager.instance.PlaySound(toggleOptionSound);
        }
        if (currentPosition < 0 )
        {
            currentPosition = options.Length - 1;
        }
        else if(currentPosition > options.Length - 1)
        {
            currentPosition = 0;
        }
        rect.position = new Vector3(rect.position.x, options[currentPosition].position.y, 0);

    }

    private void ChooseOption()
    {
        SoundManager.instance.PlaySound(chooseOptionSound);

        options[currentPosition].GetComponent<Button>().onClick.Invoke();
    }
}
