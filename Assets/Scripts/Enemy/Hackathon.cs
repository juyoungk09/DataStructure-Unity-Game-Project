using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class Hackathon : EnemyBase
{
    [Header("Hackathon Settings")]
    public GameObject lightningPrefab;
    public GameObject warningPrefab; // Prefab for warning indicators
    public GameObject emWavePrefab; // Prefab for EM Wave effect
    public float lightningCooldown = 12f;
    public int lightningCount = 5;
    public float warningDuration = 1f;
    public float strikeRadius = 3f;
    
    private float lastLightningTime;
    private bool isCastingLightning = false;
    private List<GameObject> warningIndicators = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        maxHP = 700;
        currentHP = maxHP;
        attackDamage = 25;
        moveSpeed = 1f;
        chaseRange = 12f;
        attackRange = 8f;
    }

    protected override void Update()
    {
        base.Update();
        
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= chaseRange && !isCastingLightning)
        {
            if (Time.time >= lastLightningTime + lightningCooldown)
            {
                StartCoroutine(CastLightningStorm());
            }
        }
    }

    private IEnumerator CastLightningStorm()
    {
        isCastingLightning = true;
        anim.SetTrigger("Cast");
        
        // Create warning indicators
        for (int i = 0; i < lightningCount; i++)
        {
            // Predict player position with some randomness
            Vector2 targetPos = (Vector2)player.position + Random.insideUnitCircle * 2f;
            
            // Create warning indicator using the prefab
            GameObject warning = Instantiate(warningPrefab, targetPos, Quaternion.identity);
            warning.name = "LightningWarning_" + i;
            
            // Configure warning indicator (optional customization)
            var sr = warning.GetComponent<SpriteRenderer>();
            sr.color = new Color(1f, 1f, 0f, 0.5f); // Yellow semi-transparent
            
            var collider = warning.GetComponent<CircleCollider2D>();
            collider.radius = strikeRadius;
            
            warningIndicators.Add(warning);
            
            yield return new WaitForSeconds(0.5f); // Time between warning indicators
        }
        
        yield return new WaitForSeconds(warningDuration);
        
        foreach (var warning in warningIndicators)
        {
            if (warning != null)
            {
                // Create lightning effect
                Instantiate(lightningPrefab, warning.transform.position, Quaternion.identity);
                
                // Check for player in strike radius
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    warning.transform.position, 
                    strikeRadius, 
                    playerLayer);
                    
                foreach (var hit in hits)
                {
                    Player playerHit = hit.GetComponent<Player>();
                    if (playerHit != null)
                    {
                        playerHit.TakeDamage(attackDamage);
                    }
                }
                
                // Remove warning indicator
                Destroy(warning);
            }
            
            yield return new WaitForSeconds(0.2f); // Small delay between strikes
        }
        
        // Additional EM Wave effect after lightning strikes
        if (emWavePrefab != null)
        {
            Instantiate(emWavePrefab, transform.position, Quaternion.identity);
        }
        
        warningIndicators.Clear();
        lastLightningTime = Time.time;
        isCastingLightning = false;
    }
    
    private void OnDestroy()
    {
        // Clean up any remaining warning indicators
        foreach (var warning in warningIndicators)
        {
            if (warning != null)
                Destroy(warning);
        }
        warningIndicators.Clear();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}