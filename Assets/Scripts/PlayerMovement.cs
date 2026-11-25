
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Data")]
    public PlayerData playerData;
    public LevelData levelData;

    [Header("Movement")]
    public float speed = 7f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Combat")]
    public Transform attackPoint;
    public float attackRadius = 0.7f;
    public LayerMask enemyLayer;
    public float attackCooldown = 0.8f;
    public int attackDamage = 1;

    [Header("Collectibles")]
    public TMP_Text crystalText;

    [Header("Audio")]
    public AudioClip swordSound;

    private int crystals = 0;
    public int Crystals => crystals;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private Vector2 move;
    private bool grounded;
    private float nextAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (playerData != null)
        {
            speed = playerData.speed;
            jumpForce = playerData.jumpForce;
            groundCheckRadius = playerData.groundCheckRadius;
            attackRadius = playerData.attackRadius;
            attackCooldown = playerData.attackCooldown;
            attackDamage = playerData.attackDamage;
        }
        else
        {
            Debug.LogError("PlayerData is not assigned!");
        }

        if (levelData != null)
        {
            transform.position = levelData.playerSpawnPosition;
        }
        else
        {
            Debug.LogWarning("LevelData is not assigned to Player!");
        }

        SaveData data = SaveSystem.Load();
        SetCrystals(data.savedCrystals);
    }

    void Update()
    {
        grounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        animator.SetFloat("Speed", Mathf.Abs(move.x));
        animator.SetBool("Grounded", grounded);
        animator.SetFloat("YSpeed", rb.linearVelocity.y);

        if (Mathf.Abs(move.x) > 0.1f)
        {
            bool facingRight = move.x > 0f;
            spriteRenderer.flipX = !facingRight;

            if (attackPoint != null)
            {
                Vector3 pos = attackPoint.localPosition;

                pos.x = Mathf.Abs(pos.x) *
                    (facingRight ? 1f : -1f);

                attackPoint.localPosition = pos;
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            move.x * speed,
            rb.linearVelocity.y
        );
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if (grounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            grounded = false;
            SoundEffects.Play(SoundEffects.Cue.Jump);
        }
    }

    public void OnFire(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        animator.SetTrigger("Attack");

        if (audioSource != null && swordSound != null)
        {
            SoundEffects.PlayVaried(swordSound, audioSource.volume);
        }

        DealDamage();
    }

    void DealDamage()
    {
        if (attackPoint == null)
            return;

        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius,
                enemyLayer
            );

        foreach (Collider2D enemy in enemies)
        {
            EnemyHealth skeleton =
                enemy.GetComponentInParent<EnemyHealth>();

            if (skeleton != null)
            {
                skeleton.TakeDamage(attackDamage);
            }

            BossHealth boss =
                enemy.GetComponentInParent<BossHealth>();

            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
            }
        }
    }

    public void CollectCrystal()
    {
        crystals++;
        UpdateCrystalUI();
        SavePlayerData();
    }

    public void SetCrystals(int amount)
    {
        crystals = Mathf.Max(0, amount);
        UpdateCrystalUI();
    }

    void SavePlayerData()
    {
        SaveData data = SaveSystem.Load();
        data.savedCrystals = crystals;
        SaveSystem.Save(data);
    }

    void UpdateCrystalUI()
    {
        if (crystalText != null)
        {
            crystalText.text = "Crystals: " + crystals;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}
