using UnityEngine;
using System.Collections;

public class InfoT : EnemyBase
{
    // Boss AI States (FSM)
    private enum BossState
    {
        Idle,
        Chase,
        Attack,
        Teleport,
        Dead
    }

    [Header("InfoT Boss Settings")]
    public GameObject emWavePrefab;
    public GameObject teleportEffectPrefab; // 순간이동 이펙트 (선택사항)
    
    [Header("Attack Settings")]
    public float waveCooldown = 3f;
    public int waveCount = 3;
    public float waveInterval = 0.3f;
    public float waveSpeed = 7f;
    public float waveSpreadAngle = 30f; // 파동 확산 각도
    
    [Header("Teleport Settings")]
    public float teleportCooldown = 5f;
    public float teleportDistance = 5f;
    public int damageThresholdForTeleport = 100; // 이 데미지를 받으면 순간이동
    public int maxTeleportsPerPhase = 3; // 페이즈당 최대 순간이동 횟수
    
    [Header("Boss Phases")]
    public float phase2HPThreshold = 0.6f; // 60% 이하
    public float phase3HPThreshold = 0.3f; // 30% 이하
    
    // FSM State
    private BossState currentState = BossState.Idle;
    
    // Timers
    private float lastWaveTime;
    private float lastTeleportTime;
    private float stateTimer;
    
    // Tracking
    private int accumulatedDamage = 0;
    private int currentPhase = 1;
    private int teleportsThisPhase = 0;
    private bool isDying = false;
    
    protected override void Start()
    {
        // Ensure Animator exists BEFORE calling base.Start()
        if (GetComponent<Animator>() == null)
        {
            anim = gameObject.AddComponent<Animator>();
            Debug.LogWarning($"InfoT: Animator가 없어서 자동으로 추가했습니다. Animator Controller를 할당해주세요.");
        }
        
        base.Start();
        
        // Boss Stats
        maxHP = 800;
        currentHP = maxHP;
        attackDamage = 20;
        moveSpeed = 2f;
        chaseRange = 12f;
        attackRange = 8f;
        
        // Ensure Rigidbody2D exists and configure properly
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                Debug.LogWarning($"InfoT: Rigidbody2D가 없어서 자동으로 추가했습니다.");
            }
        }
        
        // Configure Rigidbody2D for top-down 2D game
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0; // No gravity for top-down
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
        
        // Ensure Collider exists
        if (col == null)
        {
            col = GetComponent<Collider2D>();
            if (col == null)
            {
                CircleCollider2D circleCol = gameObject.AddComponent<CircleCollider2D>();
                circleCol.radius = 0.5f; // Adjust size as needed
                col = circleCol;
                Debug.LogWarning($"InfoT: Collider2D가 없어서 자동으로 추가했습니다.");
            }
        }
        
        ChangeState(BossState.Idle);
    }

    protected override void Update()
    {
        if (isDying) return;
        
        // Don't call base.Update() to override default behavior
        UpdateHealthUI();
        
        if (player == null) return;
        
        // Debug: Check if InfoT is floating
        if (rb != null && Mathf.Abs(rb.linearVelocity.y) > 0.1f)
        {
            Debug.LogWarning($"InfoT is moving vertically! Velocity: {rb.linearVelocity}, Position: {transform.position}");
        }
        
        // Update boss phase based on HP
        UpdatePhase();
        
        // FSM Update
        switch (currentState)
        {
            case BossState.Idle:
                UpdateIdleState();
                break;
            case BossState.Chase:
                UpdateChaseState();
                break;
            case BossState.Attack:
                UpdateAttackState();
                break;
            case BossState.Teleport:
                UpdateTeleportState();
                break;
        }
    }

    private void UpdateHealthUI()
    {
        if (hpSlider != null && hpText != null)
        {
            hpSlider.value = currentHP / (float)maxHP;
            hpText.text = $"{currentHP}/{maxHP}";
        }
    }

    private void UpdatePhase()
    {
        float hpPercent = currentHP / (float)maxHP;
        
        if (hpPercent <= phase3HPThreshold && currentPhase < 3)
        {
            currentPhase = 3;
            teleportsThisPhase = 0;
            waveCooldown = 2f;
            waveCount = 5;
            Debug.Log("Info_T Boss entered Phase 3!");
        }
        else if (hpPercent <= phase2HPThreshold && currentPhase < 2)
        {
            currentPhase = 2;
            teleportsThisPhase = 0;
            waveCooldown = 2.5f;
            waveCount = 4;
            Debug.Log("Info_T Boss entered Phase 2!");
        }
    }

    private void ChangeState(BossState newState)
    {
        currentState = newState;
        stateTimer = 0f;
        
        // State entry actions
        if (rb != null)
        {
            switch (newState)
            {
                case BossState.Idle:
                case BossState.Attack:
                case BossState.Teleport:
                    rb.linearVelocity = Vector2.zero;
                    break;
            }
        }
        
        // Animation state
        if (anim != null)
        {
            switch (newState)
            {
                case BossState.Idle:
                    anim.SetBool("IsMoving", false);
                    break;
                case BossState.Chase:
                    anim.SetBool("IsMoving", true);
                    break;
            }
        }
    }

    #region State Updates
    
    private void UpdateIdleState()
    {
        stateTimer += Time.deltaTime;
        
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= chaseRange)
        {
            ChangeState(BossState.Chase);
        }
    }

    private void UpdateChaseState()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        // Chase player
        Vector2 direction = (player.position - transform.position).normalized;
        
        if (rb != null)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        
        // Flip sprite based on direction
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
            transform.localScale = scale;
        }
        
        // Transition to attack if in range
        if (distance <= attackRange && Time.time >= lastWaveTime + waveCooldown)
        {
            ChangeState(BossState.Attack);
        }
        
        // Too far away, return to idle
        if (distance > chaseRange * 1.5f)
        {
            ChangeState(BossState.Idle);
        }
    }

    private void UpdateAttackState()
    {
        stateTimer += Time.deltaTime;
        
        if (stateTimer < 0.5f) return; // Wait a bit before attacking
        
        StartCoroutine(CastEMWave());
        ChangeState(BossState.Chase);
    }

    private void UpdateTeleportState()
    {
        // Teleport is handled by coroutine
        // This state just waits for the teleport to complete
    }
    
    #endregion

    private IEnumerator CastEMWave()
    {
        isAttacking = true;
        
        if (anim != null)
            anim.SetTrigger("Attack");
        
        yield return new WaitForSeconds(0.5f); // Cast time
        
        // Fire multiple waves in a spread pattern
        for (int i = 0; i < waveCount; i++)
        {
            if (emWavePrefab != null && player != null)
            {
                Vector2 baseDirection = (player.position - transform.position).normalized;
                
                // Calculate spread angle for this wave
                float angleOffset = 0f;
                if (waveCount > 1)
                {
                    angleOffset = Mathf.Lerp(-waveSpreadAngle, waveSpreadAngle, i / (float)(waveCount - 1));
                }
                
                // Rotate direction by angle offset
                float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
                float finalAngle = baseAngle + angleOffset;
                Vector2 finalDirection = new(
                    Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                    Mathf.Sin(finalAngle * Mathf.Deg2Rad)
                );
                
                GameObject wave = Instantiate(emWavePrefab, transform.position, Quaternion.identity);
                EMWave waveScript = wave.GetComponent<EMWave>();
                
                if (waveScript != null)
                {
                    waveScript.Initialize(finalDirection, waveSpeed, attackDamage);
                }
            }
            
            yield return new WaitForSeconds(waveInterval);
        }
        
        lastWaveTime = Time.time;
        isAttacking = false;
        
        if (anim != null)
            anim.SetTrigger("AttackEnd");
    }

    public new void TakeDamage(int amount)
    {
        if (isDying) return;
        
        currentHP -= amount;
        accumulatedDamage += amount;
        
        StartCoroutine(DamageFlash());
        
        Debug.Log($"Info_T Boss took {amount} damage. HP: {currentHP}/{maxHP}");
        
        // Check if should teleport to evade
        bool shouldTeleport = accumulatedDamage >= damageThresholdForTeleport && 
                             teleportsThisPhase < maxTeleportsPerPhase &&
                             Time.time >= lastTeleportTime + teleportCooldown;
        
        if (shouldTeleport && currentState != BossState.Teleport)
        {
            accumulatedDamage = 0;
            teleportsThisPhase++;
            StartCoroutine(PerformTeleport());
        }
        
        if (currentHP <= 0)
        {
            BossDie();
        }
    }

    private IEnumerator PerformTeleport()
    {
        ChangeState(BossState.Teleport);

        if (anim != null)
            anim.SetTrigger("Teleport");

        // Teleport effect
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }

        // Fade out or visual effect
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr != null ? sr.color : Color.white;
        if (sr != null)
        {
            sr.color = new(sr.color.r, sr.color.g, sr.color.b, 0.3f);

            yield return new WaitForSeconds(0.3f);

            // Calculate teleport position (away from player, but not too far)
            Vector2 awayFromPlayer = (transform.position - player.position).normalized;
            Vector2 teleportPos = (Vector2)transform.position + awayFromPlayer * teleportDistance;

            // Make sure position is valid (you might want to add bounds checking)
            transform.position = teleportPos;

            // Effect at new position
            if (teleportEffectPrefab != null)
            {
                Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(0.2f);

            sr.color = originalColor;
        }

        lastTeleportTime = Time.time;
        ChangeState(BossState.Idle);

        Debug.Log($"Info_T Boss teleported! ({teleportsThisPhase}/{maxTeleportsPerPhase} this phase)");
    }

    private void BossDie()
    {
        if (isDying) return;
        
        isDying = true;
        currentState = BossState.Dead;
        
        if (anim != null)
            anim.SetTrigger("Die");
        
        // Disable collider
        if (col != null)
            col.enabled = false;
        
        // Stop movement
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation()
    {
        // Boss death animation with rotation and scale
        float deathDuration = 2f;
        float elapsed = 0f;

        Vector3 originalScale = transform.localScale;
        Vector3 originalPosition = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr != null ? sr.color : Color.white;

        while (elapsed < deathDuration) 
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / deathDuration;

            // Rotate while dying
            transform.Rotate(0, 0, 720 * Time.deltaTime); // 720 degrees per second

            // Scale down
            float scaleMultiplier = 1f - (progress * 0.7f);
            transform.localScale = originalScale * scaleMultiplier;

            // Float up slightly
            transform.position = originalPosition + Vector3.up * (progress * 0.5f);

            // Fade out
            if (sr != null)
            {
                sr.color = new(
                    originalColor.r,
                    originalColor.g,
                    originalColor.b,
                    1f - progress
                );
            }

            yield return null;
        }

        // Notify game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyDead();
        }

        Destroy(gameObject);
    }

    private IEnumerator DamageFlash()
    {
        if (!TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
            yield break;

        Color originalColor = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    // Override the base Die method to prevent default behavior
    public new void Die()
    {
        BossDie();
    }
}