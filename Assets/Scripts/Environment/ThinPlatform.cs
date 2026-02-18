using UnityEngine;
using System.Collections;

public class ThinPlatform : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Rigidbody2D playerBody;
    private BoxCollider2D platformCollider;
    private float platformTopYPos;
    private bool isInactive = false;
    [SerializeField] private float inactiveDurationAfterJumpingFromPlatform = 1f;
    private void Awake() {
        playerBody = player.GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<BoxCollider2D>();
    }

    private void Update() {
        if (isInactive) return;
        if (playerBody.linearVelocity.y > 0.1)
        {
            platformCollider.enabled = false;
        }
        else if (playerBody.linearVelocity.y < -0.1)
        {
            platformCollider.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(JumpFromPlatform());
        }
    }

    private IEnumerator JumpFromPlatform()
    {
        platformCollider.enabled = false;
        isInactive = true;
        yield return new WaitForSeconds(inactiveDurationAfterJumpingFromPlatform);
        isInactive = false;
    }
}
