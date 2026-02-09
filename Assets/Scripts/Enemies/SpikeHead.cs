using UnityEngine;

public class SpikeHead : EnemyDamage
{
    private Vector3 destination;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;

    [SerializeField] private LayerMask playerLayer;
    private bool isAttacking;
    private float checkTimer;

    private Vector3[] directions = new Vector3[4];

    
    private void Update() {
        if (isAttacking)
            transform.Translate(destination * Time.deltaTime * speed);
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
                CheckForPlayer(); 
            }

    }

    private void CheckForPlayer()
    {
        CalculateDirections();

        for ( int i = 0; i < directions.Length; i++ )
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

            if (hit.collider != null)
            {
                isAttacking = true;
                destination = directions[i];
                checkTimer = 0;
            }
        }
    }
    private void OnEnable() {
        Stop();
    }
    private void CalculateDirections()
    {
        directions[0] = transform.right * range;
        directions[1] = -transform.right * range;
        directions[2] = transform.up * range;
        directions[3] = -transform.up * range;
    }
    private void Stop()
    {
        destination = transform.position;
        isAttacking = false;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);

        // stop the spikehead after hitting something
        Stop();

    }


}
