using UnityEngine;
using System.Collections;

public static class SkillManager
{

    public static IEnumerator UseNormalAttack(Player player) 
    {
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
    public static IEnumerator UseAnASkill1(Player player) // 예: 뒤로 순간이동
    {
        Debug.Log("used AnASkill1");
        Vector3 target = player.transform.position - player.transform.right * 3f;
        player.transform.position = target;
        player.anim.SetTrigger("AnASkill1");
        yield return null;
    }

    public static IEnumerator UseAnASkill2(Player player) // 무적 대쉬
    {
        Debug.Log("used AnASkill2");
        player.anim.SetTrigger("AnASkill2");
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
        Debug.Log("used TapieSkill1");
        player.anim.SetTrigger("TapieSkill1");

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
        Debug.Log("used TapieSkill2");
        player.anim.SetTrigger("TapieSkill2");
        int random = Random.Range(1, 5);

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
                player.attackDamage *= 2;
                break;
        }
        // 버프 적용
        yield return new WaitForSeconds(5f);
    }
}
