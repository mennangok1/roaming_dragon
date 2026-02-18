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

    private int thinPlatformLayerIndex;
    private int playerLayerIndex;
    private void Awake() {
        playerBody = player.GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<BoxCollider2D>();

        playerLayerIndex = LayerMask.NameToLayer("Player");
        thinPlatformLayerIndex = LayerMask.NameToLayer("ThinPlatform");
    }

    private void Update() {
        if (isInactive) return;
        if (playerBody.linearVelocity.y > 0.1)
        {
            Physics2D.IgnoreLayerCollision(playerLayerIndex, thinPlatformLayerIndex, true);
        }
        else if (playerBody.linearVelocity.y < -0.1)
        {
            Physics2D.IgnoreLayerCollision(playerLayerIndex, thinPlatformLayerIndex, false);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(JumpFromPlatform());
        }
    }

    private IEnumerator JumpFromPlatform()
    {
        Physics2D.IgnoreLayerCollision(playerLayerIndex, thinPlatformLayerIndex, true);
        isInactive = true;
        yield return new WaitForSeconds(inactiveDurationAfterJumpingFromPlatform);
        isInactive = false;
        Physics2D.IgnoreLayerCollision(playerLayerIndex, thinPlatformLayerIndex, false);
    }
}
