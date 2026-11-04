
using UnityEngine;

public class BossShooter : MonoBehaviour
{
    [Header("Attack")]
    public Transform player;
    public Transform firePoint;
    public GameObject fireballPrefab;
    public float attackCooldown = 2.5f;

    private Animator animator;
    private SpriteRenderer sr;
    private BossHealth bossHealth;
    private float nextAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        bossHealth = GetComponentInParent<BossHealth>();

        nextAttackTime = Time.time + 1.5f;
    }

    void Update()
    {
        if (bossHealth == null || bossHealth.IsDead)
            return;

        if (player == null)
            return;

        FacePlayer();

        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void FacePlayer()
    {
        bool playerOnLeft =
            player.position.x < transform.position.x;

        sr.flipX = playerOnLeft;

        if (firePoint != null)
        {
            Vector3 pos = firePoint.localPosition;

            pos.x = Mathf.Abs(pos.x) *
                (playerOnLeft ? -1f : 1f);

            firePoint.localPosition = pos;
        }
    }

    public void SpawnFireball()
    {
        if (bossHealth == null || bossHealth.IsDead)
            return;

        if (player == null || firePoint == null ||
            fireballPrefab == null)
            return;

        Vector2 direction =
            (player.position + Vector3.up * 0.5f -
             firePoint.position).normalized;

        GameObject fireball = Instantiate(
            fireballPrefab,
            firePoint.position,
            Quaternion.identity
        );

        BossProjectails projectile =
            fireball.GetComponent<BossProjectails>();

        if (projectile != null)
        {
            projectile.SetDirection(direction);
        }
    }
}
