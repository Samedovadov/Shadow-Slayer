
using UnityEngine;

public class SkeletonPatrol : MonoBehaviour
{
    public float speed = 1.5f;
    public float patrolDistance = 0.7f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float startX;
    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        startX = transform.position.x;
    }

    void FixedUpdate()
    {
        float targetX = startX + direction * patrolDistance;

        float newX = Mathf.MoveTowards(
            rb.position.x,
            targetX,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(new Vector2(newX, rb.position.y));

        if (Mathf.Abs(newX - targetX) < 0.01f)
        {
            direction = -direction;
        }

        sr.flipX = direction < 0;
    }
}
