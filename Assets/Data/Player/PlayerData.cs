using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerData",
    menuName = "Shadow Slayer/Data/Player"
)]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    public int maxHealth = 3;

    [Header("Movement")]
    public float speed = 7f;
    public float jumpForce = 12f;
    public float fallLimit = -8f;
    public float groundCheckRadius = 0.2f;

    [Header("Combat")]
    public float attackRadius = 0.7f;
    public float attackCooldown = 0.8f;
    public int attackDamage = 1;
}
