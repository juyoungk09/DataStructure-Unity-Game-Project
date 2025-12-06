// using UnityEngine;

// public class Boss : EnemyBase
// {
//     [Header("Boss Settings")]
//     public int phase = 1;  // 보스 페이즈
//     public float[] phaseThresholds = { 0.7f, 0.3f };  // 페이즈 전환 체력 비율
//     public float[] skillCooldowns = { 5f, 3f, 7f };  // 각 스킬의 쿨다운
//     private float[] nextSkillTime;
//     public GameObject[] skillEffects;  // 스킬 이펙트 프리팹들

//     protected override void Start()
//     {
//         base.Start();
//         nextSkillTime = new float[skillCooldowns.Length];
//         maxHP = 1000;  // 기본 체력 설정
//         currentHP = maxHP;
//     }

//     protected override void Update()
//     {
//         base.Update();
        
//         // 페이즈 체크
//         CheckPhase();
        
//         // 스킬 사용
//         if (player != null && Time.time >= nextSkillTime[0])
//         {
//             UseRandomSkill();
//         }
//     }

//     void CheckPhase()
//     {
//         float hpRatio = (float)currentHP / maxHP;
        
//         if (phase == 1 && hpRatio <= phaseThresholds[0])
//         {
//             phase = 2;
//             EnterPhase2();
//         }
//         else if (phase == 2 && hpRatio <= phaseThresholds[1])
//         {
//             phase = 3;
//             EnterPhase3();
//         }
//     }

//     void EnterPhase2()
//     {
//         // 페이즈 2 전용 로직
//         moveSpeed *= 1.3f;
//         attackDamage = (int)(attackDamage * 1.5f);
//         anim.SetTrigger("Phase2");
//     }

//     void EnterPhase3()
//     {
//         // 페이즈 3 전용 로직
//         moveSpeed *= 1.2f;
//         attackCooldown *= 0.7f;
//         anim.SetTrigger("Phase3");
//     }

//     void UseRandomSkill()
//     {
//         int skillIndex = Random.Range(0, skillCooldowns.Length);
        
//         switch(skillIndex)
//         {
//             case 0:
//                 StartCoroutine(UseSkill1());
//                 break;
//             case 1:
//                 StartCoroutine(UseSkill2());
//                 break;
//             case 2:
//                 StartCoroutine(UseSkill3());
//                 break;
//         }
        
//         nextSkillTime[skillIndex] = Time.time + skillCooldowns[skillIndex] * (1f / phase);  // 페이즈에 따라 쿨다운 감소
//     }

//     IEnumerator UseSkill1()
//     {
//         // 예시: 원형 범위 공격
//         anim.SetTrigger("Skill1");
//         yield return new WaitForSeconds(0.5f);
        
//         Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f, playerLayer);
//         foreach (var hit in hits)
//         {
//             Player player = hit.GetComponent<Player>();
//             if (player != null)
//             {
//                 player.TakeDamage(attackDamage);
//             }
//         }
        
//         if (skillEffects.Length > 0)
//             Instantiate(skillEffects[0], transform.position, Quaternion.identity);
//     }

//     IEnumerator UseSkill2()
//     {
//         // 예시: 돌진 공격
//         anim.SetTrigger("Skill2");
//         Vector2 dashDirection = (player.position - transform.position).normalized;
//         float dashSpeed = moveSpeed * 3f;
        
//         rb.velocity = dashDirection * dashSpeed;
//         yield return new WaitForSeconds(0.5f);
//         rb.velocity = Vector2.zero;
        
//         if (skillEffects.Length > 1)
//             Instantiate(skillEffects[1], transform.position, Quaternion.identity);
//     }

//     IEnumerator UseSkill3()
//     {
//         // 예시: 발사체 생성
//         anim.SetTrigger("Skill3");
        
//         for (int i = 0; i < 8; i++)
//         {
//             float angle = i * 45f;
//             Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            
//             GameObject projectile = Instantiate(skillEffects[2], transform.position, Quaternion.identity);
//             Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
//             if (rb != null)
//             {
//                 rb.velocity = direction * 5f;
//             }
            
//             yield return new WaitForSeconds(0.2f);
//         }
//     }

//     public override void TakeDamage(int damage)
//     {
//         base.TakeDamage(damage);
        
//         // 보스 피격 시 추가 효과 (예: 분노 게이지 증가 등)
//         if (currentHP <= 0)
//         {
//             // 보스 처치 시 이벤트
//             GameManager.Instance.OnBossDefeated();
//         }
//     }
// }