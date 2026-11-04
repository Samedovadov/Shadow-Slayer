
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 1f;
    public float distance = 2f;

    private Rigidbody2D rb;
    private Collider2D platformCollider;
    private Vector2 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();
        startPosition = rb.position;
    }

    void FixedUpdate()
    {
        float x = Mathf.PingPong(
            Time.fixedTime * speed,
            distance
        );

        Vector2 target = startPosition + Vector2.right * x;

        rb.MovePosition(target);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            collision.transform.position.y >
            platformCollider.bounds.max.y - 0.05f)
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") &&
            collision.transform.parent == transform)
        {
            collision.transform.SetParent(null);
        }
    }
}
