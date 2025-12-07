using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class Hackathon : EnemyBase
{
    [Header("Hackathon Settings")]
    public GameObject lightningPrefab;
    [Header("Lightning Storm")]
    public float lightningCooldown = 10f;
    public int lightningCount = 9;
    public float warningDuration = 0.35f;
    public float strikeRadius = 5.5f;
    public float warningInterval = 0.2f;
    public float strikeInterval = 0.1f;
    public float targetPredictDistance = 3f;
    public Sprite warningSprite;
    public Color warningColor = new Color(1f, 0.85f, 0.1f, 0.8f);
    public float lightningWarningScaleMultiplier = 2f;
    public float lightningEffectScale = 1.5f;

    [Header("Shockwave Burst")]
    public GameObject shockwavePrefab;
    public float shockwaveCooldown = 8f;
    public float shockwaveWarningDuration = 0.8f;
    public float shockwaveRadius = 6.5f;
    public int shockwaveDamage = 35;
    public Color shockwaveWarningColor = new Color(1f, 0.3f, 0.3f, 0.6f);
    public float shockwaveWarningScaleMultiplier = 2.2f;
    public float shockwaveEffectScale = 2.5f;

    private float lastLightningTime;
    private float lastShockwaveTime;
    private bool isCastingLightning = false;
    private bool isCastingShockwave = false;
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

        if (distanceToPlayer <= chaseRange)
        {
            if (!isCastingLightning && Time.time >= lastLightningTime + lightningCooldown)
            {
                StartCoroutine(CastLightningStorm());
            }

            if (!isCastingShockwave && Time.time >= lastShockwaveTime + shockwaveCooldown)
            {
                StartCoroutine(CastShockwaveBurst());
            }
        }
    }

    private IEnumerator CastLightningStorm()
    {
        isCastingLightning = true;
        anim.SetTrigger("Attack");
        Player playerController = player.GetComponent<Player>();

        // Create warning indicators
        for (int i = 0; i < lightningCount; i++)
        {
            Vector2 moveDir = playerController != null ? playerController.GetMoveDirection() : Vector2.zero;
            Vector2 predictedPos = (Vector2)player.position + moveDir * targetPredictDistance + Random.insideUnitCircle * 1.5f;
            Vector2 targetPos = Vector2.Lerp(player.position, predictedPos, 0.7f);

            // Create warning indicator
            GameObject warning = new GameObject("LightningWarning");
            warning.transform.position = targetPos;
            warning.transform.localScale = Vector3.one * (strikeRadius * lightningWarningScaleMultiplier);

            // Add sprite renderer for warning effect
            var sr = warning.AddComponent<SpriteRenderer>();
            sr.sprite = warningSprite;
            sr.color = warningColor;
            sr.sortingOrder = 5;

            // Particle feedback
            var ps = warning.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startSpeed = 4f;
            main.startSize = 0.4f;
            main.startLifetime = 0.4f;
            main.loop = true;
            main.playOnAwake = true;
            var emission = ps.emission;
            emission.rateOverTime = 20f;
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = strikeRadius;

            // Add collider for the warning area
            var collider = warning.AddComponent<CircleCollider2D>();
            collider.radius = strikeRadius;
            collider.isTrigger = true;

            warningIndicators.Add(warning);

            yield return new WaitForSeconds(warningInterval);
        }

        yield return new WaitForSeconds(warningDuration);

        foreach (var warning in warningIndicators)
        {
            if (warning != null)
            {
                // Create lightning effect
                GameObject lightningFX = Instantiate(lightningPrefab, warning.transform.position, Quaternion.identity);
                if (lightningFX != null)
                {
                    LightningEffect effect = lightningFX.GetComponent<LightningEffect>();
                    if (effect != null)
                    {
                        effect.scale = strikeRadius * lightningEffectScale;
                    }
                    else
                    {
                        lightningFX.transform.localScale = Vector3.one * strikeRadius * lightningEffectScale;
                    }
                }

                // Check for player in strike radius
                bool hitSomething = false;
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    warning.transform.position,
                    strikeRadius + 0.5f,
                    playerLayer);

                foreach (var hit in hits)
                {
                    Player playerHit = hit.GetComponent<Player>();
                    if (playerHit != null)
                    {
                        playerHit.TakeDamage(attackDamage);
                        hitSomething = true;
                    }
                }

                if (!hitSomething)
                {
                    DamagePlayerDirect(warning.transform.position, strikeRadius + 0.5f, attackDamage);
                }

                // Remove warning indicator
                Destroy(warning);
            }

            yield return new WaitForSeconds(strikeInterval);
        }

        warningIndicators.Clear();
        lastLightningTime = Time.time;
        isCastingLightning = false;
    }

    private IEnumerator CastShockwaveBurst()
    {
        isCastingShockwave = true;
        anim.SetTrigger("Attack");

        GameObject warning = new GameObject("ShockwaveWarning");
        warning.transform.position = transform.position;
        warning.transform.localScale = Vector3.one * (shockwaveRadius * shockwaveWarningScaleMultiplier);

        var sr = warning.AddComponent<SpriteRenderer>();
        sr.sprite = warningSprite;
        sr.color = shockwaveWarningColor;
        sr.sortingOrder = 4;

        yield return new WaitForSeconds(shockwaveWarningDuration);

        Destroy(warning);

        if (shockwavePrefab != null)
        {
            GameObject shockwaveFX = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
            if (shockwaveFX != null)
            {
                LightningEffect effect = shockwaveFX.GetComponent<LightningEffect>();
                if (effect != null)
                {
                    effect.scale = shockwaveRadius * shockwaveEffectScale;
                }
                else
                {
                    shockwaveFX.transform.localScale = Vector3.one * shockwaveRadius * shockwaveEffectScale;
                }
            }
        }

        bool shockwaveHit = false;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, shockwaveRadius, playerLayer);
        foreach (var hit in hits)
        {
            Player playerHit = hit.GetComponent<Player>();
            if (playerHit != null)
            {
                playerHit.TakeDamage(shockwaveDamage);
                shockwaveHit = true;
            }
        }

        if (!shockwaveHit)
        {
            DamagePlayerDirect(transform.position, shockwaveRadius, shockwaveDamage);
        }

        lastShockwaveTime = Time.time;
        isCastingShockwave = false;
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

    private void DamagePlayerDirect(Vector2 center, float radius, int damage)
    {
        if (player == null) return;

        float distance = Vector2.Distance(center, player.position);
        if (distance <= radius)
        {
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(damage);
            }
        }
    }
}