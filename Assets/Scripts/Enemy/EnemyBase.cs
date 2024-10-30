using TheDamage;
using TheEnemy;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageSource
{
    private EnemyAttack enemyAttack;
    private EnemyMove enemyMove;

    public LayerMask layerMask { get; set; }
    public float maxHealth { get; set; }
    public float attackDamage { get; set; }
    public float attackCooldown { get; set; }
    public float nextTimeAttack { get; set; }
    public Pathfinding pathfinding { get; set; }
    public DamageDealer damageDealer { get; set; }
    public float attackRange { get; set; }

    [SerializeField] private SphereCollider aggroRange;

    protected virtual void Start()
    {
        enemyAttack = new EnemyAttack(attackCooldown, attackRange, layerMask, damageDealer);
        enemyMove = new EnemyMove(pathfinding);
    }

    protected void Update()
    {
        // Move and attack behavior handled per frame
        enemyMove.HandleMoving(enemyMove.target, attackRange, transform);
        enemyAttack.HandleAttack(transform.position);
    }

    //Chase player if player enters the aggro range
    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Damageable"))
        {
            enemyMove.SetTarget(other);
        }
    }

    //Stop chasing when player or damageable object exits the aggro range
    protected void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Damageable"))
        {
            enemyMove.SetTarget(null);
        }
    }

    float IDamageSource.GetAttackDamage() => attackDamage;
}
