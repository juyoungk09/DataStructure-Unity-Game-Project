using UnityEngine;
using System.Collections;
using System.Linq;

public static class SkillManager
{
    // Cooldown durations (in seconds)
    public static float NORMAL_ATTACK_CD = 0.5f;
    public static float ANASKILL1_CD = 3f;
    public static float ANASKILL2_CD = 5f;
    public static float TAPIE_SKILL1_CD = 10f;
    public static float TAPIE_SKILL2_CD = 15f;

    // Cooldown trackers
    private static float normalAttackCooldown = 0f;
    private static float anaSkill1Cooldown = 0f;
    private static float anaSkill2Cooldown = 0f;
    private static float tapieSkill1Cooldown = 0f;
    private static float tapieSkill2Cooldown = 0f;

    public static void UpdateCooldowns()
    {
        if (normalAttackCooldown > 0) normalAttackCooldown -= Time.deltaTime;
        if (anaSkill1Cooldown > 0) anaSkill1Cooldown -= Time.deltaTime;
        if (anaSkill2Cooldown > 0) anaSkill2Cooldown -= Time.deltaTime;
        if (tapieSkill1Cooldown > 0) tapieSkill1Cooldown -= Time.deltaTime;
        if (tapieSkill2Cooldown > 0) tapieSkill2Cooldown -= Time.deltaTime;
    }

    public static bool IsSkillReady(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.NormalAttack => normalAttackCooldown <= 0,
            SkillType.AnASkill1 => anaSkill1Cooldown <= 0,
            SkillType.AnASkill2 => anaSkill2Cooldown <= 0,
            SkillType.TapieSkill1 => tapieSkill1Cooldown <= 0,
            SkillType.TapieSkill2 => tapieSkill2Cooldown <= 0,
            _ => false
        };
    }

    // Get remaining cooldown
    public static float GetCooldown(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.NormalAttack => normalAttackCooldown,
            SkillType.AnASkill1 => anaSkill1Cooldown,
            SkillType.AnASkill2 => anaSkill2Cooldown,
            SkillType.TapieSkill1 => tapieSkill1Cooldown,
            SkillType.TapieSkill2 => tapieSkill2Cooldown,
            _ => 0f
        };
    }

    public enum SkillType
    {
        NormalAttack,
        AnASkill1,
        AnASkill2,
        TapieSkill1,
        TapieSkill2
    }

    public static IEnumerator UseNormalAttack(Player player) 
    {
        if (normalAttackCooldown > 0) yield break;
        normalAttackCooldown = NORMAL_ATTACK_CD;
        if (player.isAttacking) yield break; 
        
        player.isAttacking = true;
        player.anim.SetTrigger("Attack");
        Debug.Log("Attack Triggered");
        player.anim.transform.localScale = new Vector3(2, 2, 1); 
        Vector2 attackPos = (Vector2)player.transform.position + 
                        (player.sr.flipX ? Vector2.left : Vector2.right) * 1.5f;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, 1.5f, LayerMask.GetMask("Enemy"));
        
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyBase enemyComponent = enemy.GetComponent<EnemyBase>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(player.attackDamage);
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        player.anim.SetTrigger("AttackEnd");
        player.isAttacking = false;
    }
    public static IEnumerator UseAnASkill1(Player player) // 뒤로 순간이동 후 공격
    {
        if (anaSkill1Cooldown > 0) yield break;
        anaSkill1Cooldown = ANASKILL1_CD;

        AudioManager.Instance.PlaySkillSound(1);
        Debug.Log("used AnASkill1 - 뒤로 순간이동 후 공격");
        
        // 1. 가장 가까운 적 찾기
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            player.transform.position, 
            10f,  // 탐지 범위
            LayerMask.GetMask("Enemy")
        );

        Vector3 originalScale = player.anim.transform.localScale;
        player.anim.transform.localScale = new Vector3(2, 2, 1);

        try
        {
            if (enemies.Length > 0)
            {
                // 가장 가까운 적 찾기
                Transform nearestEnemy = enemies[0].transform;
                float minDistance = Vector2.Distance(player.transform.position, nearestEnemy.position);
                
                foreach (var enemy in enemies)
                {
                    float distance = Vector2.Distance(player.transform.position, enemy.transform.position);
                    if (distance < minDistance)
                    {
                        nearestEnemy = enemy.transform;
                        minDistance = distance;
                    }
                }

                // 2. 적의 뒤로 이동 (적의 반대 방향으로 1.5만큼 이동)
                Vector2 direction = (player.transform.position - nearestEnemy.position).normalized;
                Vector2 teleportPos = (Vector2)nearestEnemy.position + direction * 1.5f;
                
                // 이동 애니메이션 재생
                player.anim.SetTrigger("AnASkill1");
                
                // 3. 순간이동
                player.transform.position = teleportPos;
                
                // 4. 공격 실행
                yield return new WaitForSeconds(0.2f); // 약간의 딜레이
                player.StartCoroutine(UseNormalAttack(player));
            }
            else
            {
                Debug.Log("주변에 적이 없습니다.");
            }
        }
        finally
        {
            player.anim.transform.localScale = originalScale;
        }

        yield return null;
    }

    public static IEnumerator UseAnASkill2(Player player) // 무적 대쉬
    {
        if (anaSkill2Cooldown > 0) yield break;
        anaSkill2Cooldown = ANASKILL2_CD;
        Debug.Log("used AnASkill2");
        AnimationClip clip = player.anim.runtimeAnimatorController.animationClips
            .FirstOrDefault(c => c.name == "AnASkill2");
        float animLength = clip != null ? clip.length : 1f;
        Vector3 originalScale = player.anim.transform.localScale;
        player.anim.transform.localScale = new Vector3(2, 2, 1);
        player.anim.SetTrigger("AnASkill2"); // 1.5배 크기로 증가
        try {
            // Wait for the animation to complete
            yield return new WaitForSeconds(animLength);
            
            // Reset to original size after animation
            player.anim.transform.localScale = originalScale;
        } finally {
            // Ensure we always reset the scale even if there's an error
            player.anim.transform.localScale = originalScale;
        }
        float dashTime = 0.3f;
        float speed = player.moveSpeed * 3f;
        float elapsed = 0f;
        while (elapsed < dashTime)
        {
            player.rb.linearVelocity = new Vector2(player.transform.localScale.x * speed, player.rb.linearVelocity.y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.rb.linearVelocity = Vector2.zero;
    }

    public static IEnumerator UseTapieSkill1(Player player) // 삼중쉴드
    {
        if (tapieSkill1Cooldown > 0) yield break;
        tapieSkill1Cooldown = TAPIE_SKILL1_CD;
        Debug.Log("used TapieSkill1");
        AudioManager.Instance.PlaySkillSound(3);
        AnimationClip clip = player.anim.runtimeAnimatorController.animationClips
            .FirstOrDefault(c => c.name == "TapieSkill1");
        float animLength = clip != null ? clip.length : 1f;
        Vector3 originalScale = player.anim.transform.localScale;
        player.anim.transform.localScale = new Vector3(2, 2, 1);
        player.anim.SetTrigger("TapieSkill1"); // 1.5배 크기로 증가
        try {
            // Wait for the animation to complete
            yield return new WaitForSeconds(animLength);
            
            // Reset to original size after animation
            player.anim.transform.localScale = originalScale;
        } finally {
            // Ensure we always reset the scale even if there's an error
            player.anim.transform.localScale = originalScale;
        }
        Sprite[] shieldSprites = new Sprite[] { player.htmlSprite, player.cssSprite, player.jsSprite };
        player.shield = 3;
        
        // 쉴드 생성
        for (int i = 0; i < 3; i++)
        {
            GameObject shieldGO = new GameObject("Shield_" + (i + 1));
            shieldGO.transform.SetParent(player.transform);
            shieldGO.transform.localPosition = Vector3.back * 0.1f;

            var sr = shieldGO.AddComponent<SpriteRenderer>();
            sr.sprite = shieldSprites[i];
            sr.sortingLayerName = "Player";
            sr.sortingOrder = -1;
            sr.color = new Color(1f, 1f, 1f, 0.5f);
            
            ShieldBehavior shieldBehavior = shieldGO.AddComponent<ShieldBehavior>();
            shieldBehavior.player = player;
            shieldBehavior.index = i;

            player.activeShields[i] = shieldGO;
        }

        float elapsed = 0f;
        float duration = 5f; // 최대 유지 시간
        while (elapsed < duration && player.shield > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (player.activeShields[i] != null)
                {
                    float scale = 0.2f; // Adjust the size as needed
                    player.activeShields[i].transform.localScale = new Vector3(scale, scale, 1f);

                    float angle = (Time.time * 100f + i * 120f) % 360f;
                    Vector3 offset = Quaternion.Euler(0, 0, angle) * Vector3.right * 1.5f;
                    player.activeShields[i].transform.position = player.transform.position + offset;
                }
                
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 3; i++)
            if (player.activeShields[i] != null)
                Object.Destroy(player.activeShields[i]);

        player.shield = 0;
    }

    public static IEnumerator UseTapieSkill2(Player player) // 랜덤 버프
    {
        
        AudioManager.Instance.PlaySkillSound(4);
        if (tapieSkill2Cooldown > 0) yield break;
        tapieSkill2Cooldown = TAPIE_SKILL2_CD;
        Debug.Log("used TapieSkill2");
        
        int random = Random.Range(1, 5);
        float buffDuration = 5f; // 버프 지속시간 (초)
         GameObject buffEffect = new GameObject("BuffEffect");
        buffEffect.transform.SetParent(player.transform);
        buffEffect.transform.localPosition = Vector3.zero;
        
        var sr = buffEffect.AddComponent<SpriteRenderer>();
        sr.sprite = player.buffSprite; // Player 클래스에 buffSprite 추가 필요
        sr.sortingLayerName = "Player";
        sr.sortingOrder = -2; // 실드보다 뒤에
        Color buffColor = random switch
        {
            1 => new Color(0.5f, 1f, 0.5f, 0.7f), // Speed - Green
            2 => new Color(0.5f, 0.5f, 1f, 0.7f),  // Jump - Blue
            3 => new Color(1f, 0.5f, 0.5f, 0.7f),  // HP - Red
            4 => new Color(1f, 1f, 0.5f, 0.7f),    // Size - Yellow
            _ => Color.white
        };
        sr.color = buffColor;
        switch (random)
        {
            case 1:
                player.moveSpeed *= 1.5f;
                break;
            case 2:
                player.jumpForce *= 1.5f;
                break;
            case 3:
                player.maxHP += 5;
                break;
            case 4:
                player.transform.localScale = player.transform.localScale * 2f;
                break;
        }


        // 버프 적용 시간 대기
        yield return new WaitForSeconds(buffDuration);

        // 버프 해제
        Object.Destroy(buffEffect);
        switch (random)
        {
            case 1:
                player.moveSpeed /= 1.5f;
                break;
            case 2:
                player.jumpForce /= 1.5f;
                break;
            case 4:
                player.transform.localScale = player.transform.localScale / 2f;
                break;
        }
    }
}
