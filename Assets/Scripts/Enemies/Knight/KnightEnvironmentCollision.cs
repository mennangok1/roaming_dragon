using UnityEngine;

public class KnightEnvironmentCollision : MonoBehaviour {
    
    public bool isColliding;

    private void Awake() {
        isColliding = false;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other != null && other.tag != "Player" && other.tag != "Projectile")
        {
            isColliding = true;
        }
        else
        {
            isColliding = false;
        }
    }

    public void ResetAfterFlip()
    {
        isColliding = false;
    }
}

