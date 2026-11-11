
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
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Detection")]
    public float detectionRange = 5f;
    public float maxHeightDifference = 2f;

    [Header("Attack Settings")]
    public float aimTime = 0.3f;
    public float windUpTime = 0.5f;
    public float attackDuration = 0.8f;
    public float shotDelay = 0.4f;
    public float cooldownTime = 1.2f;
    public float projectileSpeed = 6f;

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
                else if (stateTimer >= aimTime)
                {
                    ChangeState(WizardState.WindUp);
                }
                break;

            case WizardState.WindUp:
                if (!CanDetectPlayer())
                {
                    ChangeState(WizardState.Idle);
                }
                else if (stateTimer >= windUpTime)
                {
                    ChangeState(WizardState.Attack);
                }
                break;

            case WizardState.Attack:
                if (!hasShot && stateTimer >= shotDelay)
                {
                    Shoot();
                    hasShot = true;
                }

                if (stateTimer >= attackDuration)
                    ChangeState(WizardState.Cooldown);
                break;

            case WizardState.Cooldown:
                if (stateTimer >= cooldownTime)
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

        if (distance > detectionRange)
            return false;

        if (heightDifference > maxHeightDifference)
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

        Vector2 direction =
            ((Vector2)player.position + Vector2.up * 0.2f
            - (Vector2)firePoint.position).normalized;

        projectile.transform.up = direction;

        Rigidbody2D projectileRb =
            projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            projectileRb.linearVelocity =
                direction * projectileSpeed;
        }

        Debug.Log("Wizard fired a projectile!");
    }
}
