using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private Transform lastCheckpoint;

    private Health playerHealth;

    private void Awake() {
        playerHealth = GetComponent<Health>();

    }

    public void CheckRespawn()
    
    {
        
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
}
