using UnityEngine;
using System.Collections;

public class ComSys : EnemyBase
{
    [Header("ComSys Settings")]
    public GameObject orGatePrefab;
    public GameObject andGatePrefab;
    public GameObject notGatePrefab;
    public GameObject pulseWavePrefab;
    public float projectileSpeed = 6f;
    public Transform shootPoint;
    public float pulseWaveCooldown = 7f;
    public float pulseRadius = 4f;

    [Header("Attack Pattern")]
    public int volleyCount = 3;
    public float volleyDelay = 0.4f;
    public float telegraphDuration = 0.5f;
    public float predictionDistance = 1.5f;
    public float spreadAngle = 8f;

    [Header("Telegraph Visuals")]
    public Sprite telegraphSprite;
    public Color telegraphColor = new Color(0.3f, 0.8f, 1f, 0.35f);
    public float telegraphScale = 1.75f;

    private float lastAttackTime;
    private float lastPulseTime;
    private bool isCastingPulse = false;

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
        if (!isCastingPulse && Time.time >= lastPulseTime + pulseWaveCooldown)
        {
            StartCoroutine(CastPulseWave());
            return;
        }

        if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        Player playerComponent = player != null ? player.GetComponent<Player>() : null;

        for (int i = 0; i < volleyCount; i++)
        {
            if (player == null) break;

            int gateType = Random.Range(0, 3);
            Vector3 spawnPosition = shootPoint != null ? shootPoint.position : transform.position;

            Vector3 predictedTarget = player.position;
            if (playerComponent != null)
            {
                predictedTarget += (Vector3)playerComponent.GetMoveDirection() * predictionDistance;
            }

            GameObject telegraph = CreateTelegraph(predictedTarget);

            anim?.SetTrigger("Attack");
            yield return new WaitForSeconds(telegraphDuration);

            if (telegraph != null)
            {
                Destroy(telegraph);
            }

            FireGate(gateType, spawnPosition, predictedTarget);

            if (i < volleyCount - 1)
            {
                yield return new WaitForSeconds(volleyDelay);
            }
        }

        isAttacking = false;
        lastAttackTime = Time.time;
    }

    private GameObject CreateTelegraph(Vector3 position)
    {
        if (telegraphSprite == null) return null;

        GameObject telegraph = new GameObject("GateTelegraph");
        telegraph.transform.position = position;
        telegraph.transform.localScale = Vector3.one * telegraphScale;

        SpriteRenderer sr = telegraph.AddComponent<SpriteRenderer>();
        sr.sprite = telegraphSprite;
        sr.color = telegraphColor;
        sr.sortingOrder = 20;

        return telegraph;
    }

    private IEnumerator CastPulseWave()
    {
        if (isCastingPulse) yield break;

        isCastingPulse = true;
        isAttacking = true;

        GameObject pulseTelegraph = CreatePulseTelegraph();
        anim?.SetTrigger("Attack");
        yield return new WaitForSeconds(telegraphDuration);

        if (pulseTelegraph != null)
        {
            Destroy(pulseTelegraph);
        }

        if (pulseWavePrefab != null)
        {
            GameObject pulseFX = Instantiate(pulseWavePrefab, transform.position, Quaternion.identity);
            pulseFX.transform.localScale = Vector3.one * (pulseRadius * 2f);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pulseRadius, playerLayer);
        foreach (var hit in hits)
        {
            Player playerHit = hit.GetComponent<Player>();
            if (playerHit != null)
            {
                playerHit.TakeDamage(attackDamage + 10);
            }
        }

        lastPulseTime = Time.time;
        isCastingPulse = false;
        isAttacking = false;
    }

    private GameObject CreatePulseTelegraph()
    {
        if (telegraphSprite == null) return null;

        GameObject telegraph = new GameObject("PulseTelegraph");
        telegraph.transform.position = transform.position;
        telegraph.transform.localScale = Vector3.one * (pulseRadius * 2f);

        SpriteRenderer sr = telegraph.AddComponent<SpriteRenderer>();
        sr.sprite = telegraphSprite;
        sr.color = new Color(telegraphColor.r, telegraphColor.g, telegraphColor.b, 0.45f);
        sr.sortingOrder = 18;

        return telegraph;
    }

    private void FireGate(int gateType, Vector3 spawnPosition, Vector3 targetPosition)
    {
        GameObject prefab = gateType switch
        {
            0 => orGatePrefab,
            1 => andGatePrefab,
            _ => notGatePrefab
        };

        if (prefab == null) return;

        GameObject gate = Instantiate(prefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = gate.GetComponent<Rigidbody2D>();
        LogicGateProjectile gateProjectile = gate.GetComponent<LogicGateProjectile>();

        Vector2 direction = (targetPosition - spawnPosition).normalized;
        if (direction == Vector2.zero)
        {
            direction = Vector2.right * Mathf.Sign(transform.localScale.x);
        }

        float spread = gateType switch
        {
            1 => spreadAngle,
            2 => spreadAngle * 1.5f,
            _ => spreadAngle * 0.5f
        };
        direction = Quaternion.Euler(0f, 0f, Random.Range(-spread, spread)) * direction;

        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        gate.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

        if (gateProjectile != null)
        {
            gateProjectile.gateType = gateType switch
            {
                0 => LogicGateProjectile.GateType.OR,
                1 => LogicGateProjectile.GateType.AND,
                _ => LogicGateProjectile.GateType.NOT
            };
        }
    }
}