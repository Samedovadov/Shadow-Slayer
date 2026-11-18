
using UnityEngine;

public class WizardShooter : MonoBehaviour
{
    public enum WizardState
    {
        Idle,
        Aim,
        WindUp,
        Attack,
        Cooldown
    }

    [Header("References")]
    public EnemyData enemyData;
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Sprite")]
    public bool spriteFacesRight = false;

    [Header("Current State")]
    public WizardState currentState = WizardState.Idle;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private Color normalColor;

    private float stateTimer;
    private bool hasShot;
    private float firePointOffsetX;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        if (enemyData == null)
        {
            Debug.LogError("WizardShooter: EnemyData is not assigned!", this);
            enabled = false;
            return;
        }

        if (spriteRenderer != null)
            normalColor = spriteRenderer.color;

        if (player == null)
        {
            GameObject foundPlayer =
                GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        if (firePoint != null)
            firePointOffsetX =
                Mathf.Abs(firePoint.localPosition.x);

        if (player == null)
            Debug.LogWarning("Wizard: Player not assigned!");

        if (firePoint == null)
            Debug.LogWarning("Wizard: FirePoint not assigned!");
    }

    void Update()
    {
        if (player == null)
            return;

        FacePlayer();

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case WizardState.Idle:
                if (CanDetectPlayer())
                    ChangeState(WizardState.Aim);
                break;

            case WizardState.Aim:
                if (!CanDetectPlayer())
                {
                    ChangeState(WizardState.Idle);
                }
                else if (stateTimer >= enemyData.aimTime)
                {
                    ChangeState(WizardState.WindUp);
                }
                break;

            case WizardState.WindUp:
                if (!CanDetectPlayer())
                {
                    ChangeState(WizardState.Idle);
                }
                else if (stateTimer >= enemyData.windUpTime)
                {
                    ChangeState(WizardState.Attack);
                }
                break;

            case WizardState.Attack:
                if (!hasShot && stateTimer >= enemyData.shotDelay)
                {
                    Shoot();
                    hasShot = true;
                }

                if (stateTimer >= enemyData.attackDuration)
                    ChangeState(WizardState.Cooldown);
                break;

            case WizardState.Cooldown:
                if (stateTimer >= enemyData.cooldownTime)
                    ChangeState(WizardState.Idle);
                break;
        }
    }

    bool CanDetectPlayer()
    {
        float distance =
            Vector2.Distance(transform.position, player.position);

        float heightDifference =
            Mathf.Abs(transform.position.y - player.position.y);

        if (distance > enemyData.detectionRange)
            return false;

        if (heightDifference > enemyData.maxHeightDifference)
            return false;

        if (mainCamera != null)
        {
            Vector3 screenPosition =
                mainCamera.WorldToViewportPoint(transform.position);

            if (screenPosition.z <= 0 ||
                screenPosition.x < 0 || screenPosition.x > 1 ||
                screenPosition.y < 0 || screenPosition.y > 1)
            {
                return false;
            }
        }

        return true;
    }

    void FacePlayer()
    {
        bool facingRight =
            player.position.x > transform.position.x;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX =
                spriteFacesRight ? !facingRight : facingRight;
        }

        if (firePoint != null)
        {
            Vector3 position = firePoint.localPosition;

            position.x = facingRight
                ? firePointOffsetX
                : -firePointOffsetX;

            firePoint.localPosition = position;
        }
    }

    void ChangeState(WizardState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;

        switch (newState)
        {
            case WizardState.WindUp:
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.yellow;
                break;

            case WizardState.Attack:
                hasShot = false;

                if (animator != null)
                    animator.SetTrigger("Attack");
                break;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        WizardProjectile projectileScript =
            projectile.GetComponent<WizardProjectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(
                enemyData.projectileDamage,
                enemyData.projectileLifetime
            );
        }

        Vector2 direction =
            ((Vector2)player.position + Vector2.up * 0.2f
            - (Vector2)firePoint.position).normalized;

        projectile.transform.up = direction;

        Rigidbody2D projectileRb =
            projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            projectileRb.linearVelocity =
                direction * enemyData.projectileSpeed;
        }

        Debug.Log("Wizard fired a projectile!");
    }
}
