
using UnityEngine;

public class SkeletonPatrol : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        WindUp,
        Attack,
        Cooldown
    }

    [Header("Movement")]
    public float speed = 1.5f;
    public float chaseSpeed = 2f;
    public float patrolDistance = 0.7f;

    [Header("Vision")]
    public Transform player;
    public float visionRange = 4f;

    [Range(0f, 180f)]
    public float visionAngle = 140f;

    public LayerMask obstacleMask;
    public Vector2 eyeOffset = new Vector2(0f, 0.3f);

    [Header("Attack")]
    public float attackRange = 0.9f;
    public float attackHeight = 0.8f;
    public int attackDamage = 1;

    [Range(0.3f, 0.8f)]
    public float windUpTime = 0.5f;

    public float attackDuration = 0.75f;
    public float hitDelay = 0.375f;
    public float cooldownTime = 1.2f;

    [Header("Platform Safety")]
    public LayerMask groundMask;
    public float edgeCheckForward = 0.35f;
    public float edgeCheckDistance = 1f;

    [Header("Debug")]
    [SerializeField]
    private EnemyState currentState = EnemyState.Patrol;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Camera mainCamera;

    private Color normalColor;
    private float startX;
    private float stateTimer;
    private float attackElapsed;

    private bool damageApplied;

    private int patrolDirection = 1;
    private int facingDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        startX = transform.position.x;
        normalColor = sr.color;

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (obstacleMask.value == 0)
        {
            Debug.LogWarning(
                "Skeleton: Assign Obstacle Mask."
            );
        }
    }

    void Update()
    {
        if (player == null)
            return;

        switch (currentState)
        {
            case EnemyState.Patrol:

                if (CanSeePlayer())
                    ChangeState(EnemyState.Chase);

                break;

            case EnemyState.Chase:

                FacePlayer();

                if (!CanSeePlayer())
                {
                    ChangeState(EnemyState.Patrol);
                }
                else if (IsPlayerInAttackRange())
                {
                    ChangeState(EnemyState.WindUp);
                }

                break;

            case EnemyState.WindUp:

                FacePlayer();

                if (!IsPlayerInAttackRange() ||
                    !HasLineOfSight())
                {
                    ChangeState(EnemyState.Chase);
                    break;
                }

                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                    ChangeState(EnemyState.Attack);

                break;

            case EnemyState.Attack:

                // Count time from the start of the animation
                attackElapsed += Time.deltaTime;

                // Apply damage only once, during the sword swing
                if (!damageApplied &&
                    attackElapsed >= hitDelay)
                {
                    PerformAttack();
                    damageApplied = true;
                }

                // Wait until the attack animation finishes
                if (attackElapsed >= attackDuration)
                {
                    ChangeState(EnemyState.Cooldown);
                }

                break;

            case EnemyState.Cooldown:

                stateTimer -= Time.deltaTime;

                if (stateTimer <= 0f)
                {
                    ChangeState(
                        CanSeePlayer()
                            ? EnemyState.Chase
                            : EnemyState.Patrol
                    );
                }

                break;
        }
    }

    void FixedUpdate()
    {
        if (currentState == EnemyState.Patrol)
        {
            Patrol();
        }
        else if (currentState == EnemyState.Chase)
        {
            Chase();
        }
    }

    void Patrol()
    {
        float targetX =
            startX + patrolDirection * patrolDistance;

        float newX = Mathf.MoveTowards(
            rb.position.x,
            targetX,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(
            new Vector2(newX, rb.position.y)
        );

        FaceDirection(patrolDirection);

        if (Mathf.Abs(newX - targetX) < 0.01f)
            patrolDirection = -patrolDirection;
    }

    void Chase()
    {
        float difference =
            player.position.x - transform.position.x;

        if (Mathf.Abs(difference) <= attackRange * 0.8f)
            return;

        int direction = difference > 0f ? 1 : -1;

        FaceDirection(direction);

        if (groundMask.value != 0)
        {
            Vector2 checkPoint = rb.position +
                new Vector2(direction * edgeCheckForward, 0f);

            RaycastHit2D groundHit = Physics2D.Raycast(
                checkPoint,
                Vector2.down,
                edgeCheckDistance,
                groundMask
            );

            if (groundHit.collider == null)
                return;
        }

        float newX = Mathf.MoveTowards(
            rb.position.x,
            player.position.x,
            chaseSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(
            new Vector2(newX, rb.position.y)
        );
    }

    bool CanSeePlayer()
    {
        if (!IsOnScreen())
            return false;

        Vector2 origin =
            (Vector2)transform.position + eyeOffset;

        Vector2 target =
            (Vector2)player.position + eyeOffset;

        Vector2 direction = target - origin;

        if (direction.magnitude > visionRange)
            return false;

        Vector2 forward =
            new Vector2(facingDirection, 0f);

        float angle = Vector2.Angle(
            forward,
            direction
        );

        if (angle > visionAngle / 2f)
            return false;

        return HasLineOfSight();
    }

    bool HasLineOfSight()
    {
        if (obstacleMask.value == 0)
            return false;

        Vector2 origin =
            (Vector2)transform.position + eyeOffset;

        Vector2 target =
            (Vector2)player.position + eyeOffset;

        RaycastHit2D hit = Physics2D.Linecast(
            origin,
            target,
            obstacleMask
        );

        return hit.collider == null;
    }

    bool IsPlayerInAttackRange()
    {
        float horizontalDistance =
            Mathf.Abs(
                player.position.x - transform.position.x
            );

        float verticalDistance =
            Mathf.Abs(
                player.position.y - transform.position.y
            );

        return horizontalDistance <= attackRange &&
               verticalDistance <= attackHeight &&
               IsOnScreen();
    }

    bool IsOnScreen()
    {
        if (mainCamera == null)
            return false;

        Vector3 viewportPoint =
            mainCamera.WorldToViewportPoint(
                transform.position
            );

        return viewportPoint.z > 0f &&
               viewportPoint.x >= 0f &&
               viewportPoint.x <= 1f &&
               viewportPoint.y >= 0f &&
               viewportPoint.y <= 1f;
    }

    void PerformAttack()
    {
        if (!IsPlayerInAttackRange() ||
            !HasLineOfSight())
            return;

        PlayerHealth health =
            player.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(attackDamage);
        }
    }

    void ChangeState(EnemyState newState)
    {
        currentState = newState;

        sr.color = normalColor;

        if (newState == EnemyState.WindUp)
        {
            stateTimer = windUpTime;
            sr.color = Color.yellow;
        }
        else if (newState == EnemyState.Attack)
        {
            attackElapsed = 0f;
            damageApplied = false;

            sr.color = Color.red;

            if (animator != null)
            {
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Attack");
            }
        }
        else if (newState == EnemyState.Cooldown)
        {
            stateTimer = cooldownTime;
        }
    }

    void FacePlayer()
    {
        float difference =
            player.position.x - transform.position.x;

        if (Mathf.Abs(difference) > 0.05f)
        {
            FaceDirection(
                difference > 0f ? 1 : -1
            );
        }
    }

    void FaceDirection(int direction)
    {
        facingDirection = direction;
        sr.flipX = direction < 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            visionRange
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}
