using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private CameraController cam;
    [SerializeField] private GameObject player;
    private Player playerScript;
    private Transform currentRoom;
    private Collider2D doorTrigger;

    private void Awake() {
        doorTrigger = GetComponent<Collider2D>();
        playerScript = player.GetComponent<Player>();
    }

    private void Update() {
        if (doorTrigger == null)
        {
            return;
        }
        if (playerScript.GetCurrentRoom() != previousRoom && playerScript.GetCurrentRoom() != nextRoom)
        {
            return;
        }



        // Only auto-correct room state while the player is inside this doorway.
        // This prevents distant doors from changing camera room after a respawn teleport.
/*         if (!doorTrigger.OverlapPoint(player.transform.position))
        {
            return;
        } */

        if (player.transform.position.x > transform.position.x && currentRoom == previousRoom)
        {
            // player is to the right of the door but the current room is set to the previous room, we must be in the next room
            TransitionRooms(currentRoom, nextRoom);
        }
        else if (player.transform.position.x < transform.position.x && currentRoom == nextRoom)
        {
            // player is to the left of the door but the current room is set to the next room, we must be in the previous room
            TransitionRooms(currentRoom, previousRoom);
        }
    }

     private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x)
            {
                //player coming from the left, so move to the next room
                TransitionRooms(previousRoom, nextRoom);
            }
            else
            {
                //player coming from the right, so move to the previous room
                TransitionRooms(nextRoom, previousRoom);
            }
            
        }
    } 


    private void TransitionRooms(Transform fromRoom, Transform toRoom)
    {
        cam.MoveToNewRoom(toRoom);
        toRoom.GetComponent<Room>().ActivateRoom(true);
        fromRoom.GetComponent<Room>().ActivateRoom(false);
        currentRoom = toRoom;
        playerScript.SetCurrentRoom(toRoom);
    }
}
