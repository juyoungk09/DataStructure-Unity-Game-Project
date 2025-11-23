using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Player : MonoBehaviour
{
    // State Machine
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkState walkState;
    [HideInInspector] public RunState runState;
    [HideInInspector] public JumpState jumpState;

    [Header("Stats")]
    public int maxHP = 10;
    public int HP;
    public float moveSpeed = 5f;
    public float runMultiplier = 1.5f;
    public float jumpForce = 12f;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Components")]
    public Slider hpSlider;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer sr;

    [HideInInspector] public float moveInput;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isRunning;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        HP = maxHP;

        // Initialize State Machine
        stateMachine = new StateMachine();
        idleState = new IdleState(this, stateMachine);
        walkState = new WalkState(this, stateMachine);
        runState = new RunState(this, stateMachine);
        jumpState = new JumpState(this, stateMachine);
        // attackState = new AttackState(this, stateMachine);
        // AnAAttackState = new AnAAttackState(this, stateMachine);
        // AnASkill1State = new AnASkill1State(this, stateMachine);
        // AnASkill2State = new AnASkill2State(this, stateMachine);
        // TapieAttackState = new TapieAttackState(this, stateMachine);
        // TapieSkill1State = new TapieSkill1State(this, stateMachine);
        // TapieSkill2State = new TapieSkill2State(this, stateMachine);
        stateMachine.Initialize(idleState);

        // Rigidbody2D 설정
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전 방지
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Physics Material 2D 적용 (마찰 0)
        var col = GetComponent<Collider2D>();
        PhysicsMaterial2D mat = new PhysicsMaterial2D();
        mat.friction = 0f;
        col.sharedMaterial = mat;
    }

    void Start()
    {
        if (hpSlider == null)
        {
            hpSlider = GameObject.Find("HPBar").GetComponent<Slider>();
        }
        hpSlider.maxValue = maxHP;
        hpSlider.value = HP;
    }

    void Update()
    {
        // 입력 처리
        moveInput = Input.GetAxisRaw("Horizontal");

        // FlipX 처리
        if (moveInput != 0)
            sr.flipX = moveInput < 0;

        // 상태 머신 업데이트
        stateMachine.Update();

        // 애니메이션 파라미터 업데이트
        anim.SetBool("isGrounded", isGrounded);

        // HPBar 업데이트
        hpSlider.value = HP;
    }

    void FixedUpdate()
    {
        CheckGround();
        stateMachine.FixedUpdate();
    }

    // 상태 머신에서 호출할 수 있도록 public으로 변경

    public void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0) Die();
    }

    void Die()
    {
        // 죽는 애니메이션 재생 가능
        anim.SetTrigger("die");
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 0.5f); // 0.5초 후 제거
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
