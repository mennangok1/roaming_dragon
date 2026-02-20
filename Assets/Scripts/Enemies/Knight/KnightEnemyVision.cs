using UnityEngine;

public class KnightEnemyVision : MonoBehaviour
{
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float visionDistance;
    [SerializeField] [Range(0, 180)] private float viewAngle = 90f; // Total width of vision cone

    private KnightEnemy knightEnemy;
    
    private  GameObject player;
    private bool hasSight = false;
    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        knightEnemy = GetComponent<KnightEnemy>();
    }

    private void Update() {
        if (player == null || knightEnemy == null)
        {
            return;
        }

        if (hasSight)
        {
            ChasePlayer();
        }
        else
        {
            knightEnemy.StopChaseAndResumePatrol();
        }
    }

    private void FixedUpdate() 
    {
        if (player == null)
        {
            hasSight = false;
            return;
        }

        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // 1. Distance Check (optimization)
        if (distanceToPlayer <= visionDistance)
        {
            // 2. Angle Check
            // Vector2.Angle returns a value between 0 and 180
            float angle;
            if (transform.localScale.x > 0)
            {
                angle = Vector2.Angle(transform.right, directionToPlayer);
            }
            else
            {
                angle = Vector2.Angle(-transform.right, directionToPlayer);
            }

            if (angle < viewAngle / 2f) 
            {
                // 3. Raycast Check (Line of Sight)
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionDistance, detectionMask);
                
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    hasSight = true;
                    Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, Color.green);
                    return; // Exit early if found
                }
            }
        }

        hasSight = false;
        if (transform.localScale.x > 0)
        {
        Debug.DrawRay(transform.position, transform.right * visionDistance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, -transform.right * visionDistance, Color.red);
        }
    }


    private void ChasePlayer()
    {
        knightEnemy.StartChase();
    }
}
