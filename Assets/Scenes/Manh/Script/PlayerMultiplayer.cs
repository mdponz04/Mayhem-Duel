using TheDamage;
using TheHealth;
using Unity.Netcode;
using UnityEngine;

public class PlayerMultiplayer : NetworkBehaviour, IDamageSource
{
    private PlayerInputAction inputActions;
    private HealthSystem healthSystem;
    public float speed = 5f;
    private DamageDealer damageDealer;
    private float maxHealth = 100f;
    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.SetUp(maxHealth);
        damageDealer = GetComponent<DamageDealer>();
        damageDealer.SetUp();
        inputActions = new PlayerInputAction();
        inputActions.Enable();  // Enable the input actions
    }
    private void Update()
    {
        //Debug purpose
        HandleMove();
        HandleAttack();
    }
    private void HandleMove()
    {
        if (!IsOwner) return;
        Vector2 moveDirInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveDirInput.x, 0, moveDirInput.y);
        transform.position += moveDir * speed * Time.deltaTime;
    }
    private void HandleAttack()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0))
        {
            float sphereRadius = 5f;
            Vector3 sphereCenter = transform.position;  // Center of the sphere at the player's position

            // Use OverlapSphere to detect colliders within the sphere radius
            Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, sphereRadius, LayerMask.GetMask("Enemy"));

            // Iterate over all the colliders that were detected in the sphere
            foreach (Collider collider in hitColliders)
            {
                // Check if the detected object has the "Enemy" tag
                if (collider.CompareTag("Enemy"))
                {
                    /*Debug.Log("Enemy detected: " + collider.name);*/

                    // Attempt to deal damage (or perform other actions)
                    Vulnerable vulnerableComponent = collider.GetComponent<Vulnerable>();
                    if (vulnerableComponent != null)
                    {
                        /*Debug.Log("Enemy is vulnerable. Attempting to deal damage.");*/
                        damageDealer.TryDoDamage(vulnerableComponent);
                    }
                }
            }
        }
    }
    float IDamageSource.GetAttackDamage() => 10f;
}
