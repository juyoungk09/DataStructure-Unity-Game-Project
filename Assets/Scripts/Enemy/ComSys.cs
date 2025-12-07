using UnityEngine;
using System.Collections;

public class ComSys : EnemyBase
{
    [Header("ComSys Settings")]
    public GameObject orGatePrefab;
    public GameObject andGatePrefab;
    public GameObject notGatePrefab;
    public float projectileSpeed = 5f;
    public Transform shootPoint;  // Assign this in the inspector to set the spawn point

    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();
        maxHP = 500;
        currentHP = maxHP;
        attackDamage = 20;
        moveSpeed = 1.5f;
        chaseRange = 10f;
        attackRange = 8f;
        attackCooldown = 3f;
    }

    protected override void Update()
    {
        base.Update();
        
        if (player == null) return;
        
        // Face the player
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(5, 5, 1);
        else
            transform.localScale = new Vector3(-5, 5, 1);
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        
        // Play attack animation
        if (anim != null)
        {
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(0.3f); // Wait for attack animation to reach the shooting frame
        }

        // Shoot the logic gate
        if (player != null)
        {
            // Choose random logic gate type
            int gateType = Random.Range(0, 3);
            GameObject gatePrefab = gateType switch
            {
                0 => orGatePrefab,
                1 => andGatePrefab,
                _ => notGatePrefab
            };

            // Determine spawn position
            Vector3 spawnPosition = shootPoint != null ? shootPoint.position : transform.position;
            
            // Instantiate the gate projectile
            GameObject gate = Instantiate(gatePrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = gate.GetComponent<Rigidbody2D>();
            LogicGateProjectile gateProjectile = gate.GetComponent<LogicGateProjectile>();
            
            if (rb != null && gateProjectile != null)
            {
                Vector2 direction = (player.position - spawnPosition).normalized;
                rb.linearVelocity = direction * projectileSpeed;
                
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                gate.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                
                // Set the gate type
                gateProjectile.gateType = gateType switch
                {
                    0 => LogicGateProjectile.GateType.OR,
                    1 => LogicGateProjectile.GateType.AND,
                    _ => LogicGateProjectile.GateType.NOT
                };
            }
        }

        // Wait for the rest of the attack animation
        if (anim != null)
        {
            yield return new WaitForSeconds(0.7f);
        }

        isAttacking = false;
        lastAttackTime = Time.time;
    }
}