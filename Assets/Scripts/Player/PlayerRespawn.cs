using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;

    private UIManager uiManager;
    private Transform lastCheckpoint;

    private Vector3 initialPosition;

    private Health playerHealth;
    

    [SerializeField] private Transform firstRoom;

    private void Awake() {
        playerHealth = GetComponent<Health>();
        uiManager = FindObjectOfType<UIManager>(); // use this when you are sure only one object of this type exists. Don't call this method repeatedly e.g. in Update()
    }

    public void CheckRespawn()

    {
        if ( playerHealth.GetCurrentGlobalHealth() < 1)
        {
            uiManager.GameOver();
            return;
        }
        if (lastCheckpoint == null)
        {
            transform.position = initialPosition;
            playerHealth.Respawn();
<<<<<<< Updated upstream
=======
            cameraController.MoveToNewRoom(firstRoom);
>>>>>>> Stashed changes
            return;
        }
        //Move the player to last checkpoint
        transform.position = lastCheckpoint.position;
        
        //Restore player's health
        playerHealth.Respawn();

        //Move the camera to the room where lastCheckpoint is located (transform.parent directs to the room if we set the checkpoint object as the child of the room it belongs to)
        Camera.main.GetComponent<CameraController>().MoveToNewRoom(lastCheckpoint.parent);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.tag == "Checkpoint")
        {
            lastCheckpoint = other.transform;
            SoundManager.instance.PlaySound(checkpointSound);
            other.GetComponent<Collider2D>().enabled = false;
            other.GetComponent<Animator>().SetBool("isChecked", true);
        }
    }

    public void SetInitialPoisition(Vector3 _initialPosition)
    {
        initialPosition = _initialPosition;
    }
}
