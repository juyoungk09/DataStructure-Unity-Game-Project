using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float chaseRange = 5f;     // 플레이어 감지 거리
    public float attackRange = 1f;    // 공격 가능 거리
    public float moveSpeed = 3f;      // 이동 속도
    public int attackDamage = 10;     // 공격 데미지
    public float attackCooldown = 1.5f; // 공격 쿨타임
    public float attackDelay = 0.3f;   // 공격 애니메이션 후 데미지 적용까지의 딜레이
    public LayerMask playerLayer;     
    
    [Header("References")]
    public Animator anim;
    public Transform attackPoint;     
    public Slider hpSlider;
    public TMP_Text hpText;
    [Header("Debug")]
    public bool showGizmos = true;    
    
    [Header("Stats")]
    public int maxHP = 100;
    private Vector3 originalScale;
    public Transform player;
    public bool isAttacking = false;
    [HideInInspector]
    protected int currentHP;
    [HideInInspector]
    private Rigidbody2D rb;
    [HideInInspector]
    public Collider2D col;
    [HideInInspector]
    public Collider2D playerCol;
    protected virtual void Start()
    {
        StartCoroutine(InitPlayer());

        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null)
                Debug.LogError($"{name}에 Animator가 없습니다!");
        }
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true; // Z축 회전 고정
        }
        col = GetComponent<Collider2D>();
        playerCol = GameManager.Instance.player.GetComponent<Collider2D>();
        currentHP = maxHP;
        
        if (col != null && playerCol != null) {
            Physics2D.IgnoreCollision(col, playerCol);
            originalScale = transform.localScale;
        }
    }

    private IEnumerator InitPlayer()
    {
        while (GameManager.Instance == null || GameManager.Instance.player == null)
            yield return null; // 한 프레임씩 기다리면서 체크

        player = GameManager.Instance.player.transform;
    }

    protected virtual void Update()
    {
        if (player == null) return;
        if (hpSlider != null && hpText != null)
        {
            hpSlider.value = currentHP / (float)maxHP;
            hpText.text = $"{currentHP}/{maxHP}";
        }
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            if(distance <= attackRange){
                if (this is not Social) {
                    Attack();
                }
            }
            else {
                ChasePlayer();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        // 공격 범위 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            attackPoint != null ? attackPoint.position : transform.position, 
            attackRange);
        
        // 추적 범위 표시
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.2f); // 연한 노란색
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        // 공격 방향 표시
        if (player != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(
                attackPoint != null ? attackPoint.position : transform.position,
                player.position);
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

        
        if (dir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir.x) * originalScale.x, originalScale.y, originalScale.z);
    }

    protected virtual void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
            
            // 공격 애니메이션 후 데미지 적용
            Invoke(nameof(DealDamage), attackDelay);
            // 공격 쿨타임
            Invoke(nameof(ResetAttack), attackCooldown);
            Invoke(nameof(AttackEnd), 0.5f);
        }
    }
    void AttackEnd()
    {
        anim.SetTrigger("AttackEnd");
    }
    void DealDamage()
    {
        if (player == null) return;
    
        // 공격 범위 내의 플레이어 감지
    
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(
            attackPoint != null ? attackPoint.position : transform.position, 
            attackRange, 
            playerLayer);
        foreach (Collider2D playerCollider in hitPlayers)
        {
            if (playerCollider.CompareTag("Player"))
            {
                Player player = playerCollider.GetComponent<Player>();
                if (player != null && player.canTakeDamage)
                {
                    player.TakeDamage(attackDamage);
                    // 공격 방향으로 넉백 효과 (선택사항)
                    Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                    player.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
                }
            }
        }
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        StartCoroutine(DamageFlash());
        Debug.Log($"{gameObject.name}이(가) {amount}의 데미지를 받음. 현재 체력: {currentHP} -> {currentHP - amount}");
   
        if (currentHP <= 0)
            Die();
    }

    public void Die()
    {
        anim.SetTrigger("Die");
        GameManager.Instance.OnEnemyDead();
        Destroy(gameObject, 0.5f);
    }

    private IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }
}
