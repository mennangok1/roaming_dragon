using UnityEngine;

public class EnemySidewayMovement : MonoBehaviour
{

    [SerializeField] private float damage;
    [SerializeField] private float movementDistance;
    [SerializeField] private float speed;

    private bool isMovingLeft;
    private float leftEdge;
    private float rightEdge;


    private void Awake() {
        leftEdge = transform.position.x - movementDistance;
        rightEdge = transform.position.x + movementDistance;
    }


    private void Update() {
        if (isMovingLeft)
        {
            if (transform.position.x > leftEdge)
            {
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                isMovingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightEdge)
            {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            else
            {
                isMovingLeft = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
