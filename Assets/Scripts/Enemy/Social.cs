using UnityEngine;

public class Social : EnemyBase
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;  // 발사체 프리팹
    public float projectileSpeed = 1f;   // 발사체 속도
    private float lastAttackTime;

    protected override void Update()
    {
        base.Update();

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // 플레이어가 사정거리 내에 있고 쿨다운이 지났으면 공격
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
    }

    protected override void Attack()
    {
        if (projectilePrefab == null || player == null) return;

        // 공격 애니메이션 트리거 (필요시)
        anim?.SetTrigger("Attack");

        // 플레이어 방향 계산
        Vector2 direction = (player.position - transform.position).normalized;
        
        // 발사체 생성 및 발사
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        
        Debug.Log("프리팹 생성 시도");
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
            
            // 발사체가 플레이어를 향하도록 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // 공격 쿨다운 적용
        lastAttackTime = Time.time;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

}