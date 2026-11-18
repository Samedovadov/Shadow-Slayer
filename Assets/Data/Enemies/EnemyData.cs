
using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyData",
    menuName = "Shadow Slayer/Data/Enemy"
)]
public class EnemyData : ScriptableObject
{
    [Header("Health")]
    public int maxHealth = 3;

    [Header("Movement")]
    public float speed = 1.5f;
    public float chaseSpeed = 2f;
    public float patrolDistance = 0.7f;

    [Header("Detection")]
    public float detectionRange = 4f;
    public float maxHeightDifference = 2f;
    public float visionAngle = 140f;

    [Header("Combat")]
    public float attackRange = 0.9f;
    public float attackHeight = 0.8f;
    public int attackDamage = 1;
    public float attackCooldown = 1.2f;

    [Header("Projectile")]
    public float projectileSpeed = 6f;
    public int projectileDamage = 1;
    public float projectileLifetime = 5f;

    [Header("Charger Movement")]
    public float chargeSpeed = 9f;
    public float waypointTolerance = 0.15f;

    [Header("Charger Detection")]
    public float detectionHeight = 1.5f;

    [Header("Charger Attack")]
    public float windUpTime = 0.6f;
    public float chargeDuration = 0.35f;
    public float recoveryTime = 1.5f;

    [Header("Wizard Attack")]
    public float aimTime = 0.3f;
    public float attackDuration = 0.8f;
    public float shotDelay = 0.4f;
    public float cooldownTime = 1.2f;

    [Header("Skeleton Attack")]
    public float skeletonWindUpTime = 0.5f;
    public float skeletonAttackDuration = 0.75f;
    public float hitDelay = 0.375f;
}
