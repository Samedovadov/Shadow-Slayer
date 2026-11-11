
using UnityEngine;

public class ChargerAI : MonoBehaviour
{
    public enum ChargerState
    {
        Patrol,
        Chase,
        WindUp,
        Charge,
        Recovery
    }

    [Header("References")]
    public Transform player;
    public Transform waypointA;
    public Transform waypointB;
    public Transform groundCheck;

    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 2.5f;
    public float chargeSpeed = 9f;
    public float waypointTolerance = 0.15f;

    [Header("Detection")]
    public float detectionRange = 4f;
    public float detectionHeight = 1.5f;
    public float attackRange = 2f;

    [Header("Attack")]
    public int attackDamage = 1;
    public float windUpTime = 0.6f;
    public float chargeDuration = 0.35f;
    public float recoveryTime = 1.5f;

    [Header("Ground and Walls")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float edgeCheckForward = 0.4f;
    public float edgeCheckDistance = 0.6f;
    public float wallCheckHeight = 0.5f;

    [Header("Sprite")]
    public bool spriteFacesRight = true;

    [Header("Current State")]
    public ChargerState currentState = ChargerState.Patrol;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Camera mainCamera;

    private Color normalColor;
    private float stateTimer;
    private float chargeDirection;
    private int facingDirection = 1;
    private bool goingToB;
    private bool hasHitPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        if (spriteRenderer != null)
            normalColor = spriteRenderer.color;

        if (player == null)
        {
            GameObject foundPlayer =
                GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (rb == null)
        {
            Debug.LogError("Charger needs Rigidbody2D!");
            enabled = false;
            return;
        }

        if (waypointA == null || waypointB == null)
        {
            Debug.LogWarning("Charger waypoints are missing!");
        }

        if (groundCheck == null)
        {
            Debug.LogWarning("Charger GroundCheck is missing!");
        }

        ChangeState(ChargerState.Patrol);
    }

    void Update()
    {
        if (player == null)
            return;

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case ChargerState.Patrol:
                if (CanDetectPlayer())
                    ChangeState(ChargerState.Chase);
                break;

            case ChargerState.Chase:
                if (!CanDetectPlayer())
                {
                    ChangeState(ChargerState.Patrol);
                }
                else if (IsPlayerInAttackRange())
                {
                    ChangeState(ChargerState.WindUp);
                }
                break;

            case ChargerState.WindUp:
                if (!CanDetectPlayer() ||
                    !IsPlayerInAttackRange())
                {
                    ChangeState(ChargerState.Patrol);
                }
                else if (stateTimer >= windUpTime)
                {
                    chargeDirection =
                        player.position.x >= transform.position.x
                        ? 1f : -1f;

                    FaceDirection(chargeDirection);

                    ChangeState(ChargerState.Charge);
                }
                break;

            case ChargerState.Charge:
                if (!IsOnScreen(transform.position) ||
                    stateTimer >= chargeDuration)
                {
                    ChangeState(ChargerState.Recovery);
                }
                break;

            case ChargerState.Recovery:
                if (stateTimer >= recoveryTime)
                    ChangeState(ChargerState.Patrol);
                break;
        }
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case ChargerState.Patrol:
                Patrol();
                break;

            case ChargerState.Chase:
                Chase();
                break;

            case ChargerState.Charge:
                Charge();
                break;
        }
    }

    void Patrol()
    {
        if (waypointA == null || waypointB == null)
            return;

        Transform target = goingToB ? waypointB : waypointA;

        float difference =
            target.position.x - rb.position.x;

        if (Mathf.Abs(difference) <= waypointTolerance)
        {
            goingToB = !goingToB;
            return;
        }

        float direction = Mathf.Sign(difference);

        FaceDirection(direction);
        MoveHorizontal(direction, patrolSpeed);
    }

    void Chase()
    {
        if (player == null)
            return;

        float difference =
            player.position.x - rb.position.x;

        if (Mathf.Abs(difference) < 0.2f)
            return;

        float direction = Mathf.Sign(difference);

        FaceDirection(direction);
        MoveHorizontal(direction, chaseSpeed);
    }

    void Charge()
    {
        FaceDirection(chargeDirection);

        bool moved =
            MoveHorizontal(chargeDirection, chargeSpeed);

        if (!moved)
            ChangeState(ChargerState.Recovery);
    }

    bool MoveHorizontal(float direction, float speed)
    {
        if (!HasGroundAhead(direction))
            return false;

        if (HasWallAhead(direction, speed))
            return false;

        Vector2 nextPosition = rb.position;

        nextPosition.x +=
            direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);

        return true;
    }

    bool HasGroundAhead(float direction)
    {
        if (groundCheck == null || groundLayer.value == 0)
            return false;

        Vector2 origin =
            (Vector2)groundCheck.position +
            Vector2.right * direction * edgeCheckForward +
            Vector2.up * 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.down,
            edgeCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    bool HasWallAhead(float direction, float speed)
    {
        if (groundCheck == null || wallLayer.value == 0)
            return false;

        Vector2 origin =
            (Vector2)groundCheck.position +
            Vector2.up * wallCheckHeight;

        float distance =
            speed * Time.fixedDeltaTime + 0.2f;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.right * direction,
            distance,
            wallLayer
        );

        return hit.collider != null;
    }

    bool CanDetectPlayer()
    {
        if (player == null)
            return false;

        float distance =
            Vector2.Distance(transform.position, player.position);

        float heightDifference =
            Mathf.Abs(transform.position.y - player.position.y);

        if (distance > detectionRange)
            return false;

        if (heightDifference > detectionHeight)
            return false;

        if (!IsOnScreen(transform.position))
            return false;

        if (!IsOnScreen(player.position))
            return false;

        return true;
    }

    bool IsPlayerInAttackRange()
    {
        if (player == null)
            return false;

        float horizontalDistance =
            Mathf.Abs(player.position.x - transform.position.x);

        float verticalDistance =
            Mathf.Abs(player.position.y - transform.position.y);

        return horizontalDistance <= attackRange &&
               verticalDistance <= detectionHeight &&
               IsOnScreen(transform.position) &&
               IsOnScreen(player.position);
    }

    bool IsOnScreen(Vector3 worldPosition)
    {
        if (mainCamera == null)
            return true;

        Vector3 position =
            mainCamera.WorldToViewportPoint(worldPosition);

        return position.z > 0f &&
               position.x >= 0f &&
               position.x <= 1f &&
               position.y >= 0f &&
               position.y <= 1f;
    }

    void FaceDirection(float direction)
    {
        if (direction == 0f)
            return;

        facingDirection = direction > 0f ? 1 : -1;

        if (spriteRenderer == null)
            return;

        bool facingRight = facingDirection > 0;

        spriteRenderer.flipX =
            spriteFacesRight ? !facingRight : facingRight;
    }

    void ChangeState(ChargerState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;

        if (animator != null)
        {
            bool moving =
                newState == ChargerState.Patrol ||
                newState == ChargerState.Chase;

            animator.SetBool("Moving", moving);
        }

        switch (newState)
        {
            case ChargerState.WindUp:
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.red;
                break;

            case ChargerState.Charge:
                hasHitPlayer = false;

                if (animator != null)
                    animator.SetTrigger("Dash");
                break;

            case ChargerState.Recovery:
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.gray;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    void TryDamagePlayer(Collider2D other)
    {
        if (currentState != ChargerState.Charge)
            return;

        if (hasHitPlayer)
            return;

        PlayerHealth health =
            other.GetComponentInParent<PlayerHealth>();

        if (health == null)
            return;

        hasHitPlayer = true;

        health.TakeDamage(attackDamage);

        Debug.Log("Charger hit player!");
    }
}
