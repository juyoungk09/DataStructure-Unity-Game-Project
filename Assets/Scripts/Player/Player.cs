    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine.SceneManagement;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]

    public class Player : MonoBehaviour
    {
        [HideInInspector] public StateMachine stateMachine;
        [HideInInspector] public IdleState idleState;
        [HideInInspector] public WalkState walkState;
        [HideInInspector] public RunState runState;
        [Header("Stats")]
        public int maxHP = 100;
        public int HP;
        public int attackDamage = 50;
        public float moveSpeed = 5f;
        public float runMultiplier = 1.5f;
        [Header("Jump Settings")]
        [Range(1f, 20f)]
        public float jumpForce = 6f;
        public float jumpTime = 0.35f;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;
        public float jumpCooldown = 0.2f;
        [HideInInspector] public bool isJumping = false;
        [HideInInspector] public float jumpTimeCounter;
        [HideInInspector] public float lastJumpTime = -1f;
        public bool canMove = true;
        public bool canTakeDamage = true;
        [Header("GroundCheck")]
        public Transform groundCheck;
        public float groundRadius = 0.2f;
        public LayerMask groundLayer;
        [Header("Components")]
        public Slider hpSlider;
        [HideInInspector] public Rigidbody2D rb;
        [HideInInspector] public Animator anim;
        [HideInInspector] public SpriteRenderer sr;
        [HideInInspector] public Vector3 animBaseScale;
        [HideInInspector] public int shield = 0;  // 남은 쉴드 수
        [HideInInspector] public GameObject[] activeShields = new GameObject[3]; // 현재 쉴드 오브젝트
        [HideInInspector] public float moveInput;
        [HideInInspector] public bool isGrounded;
        [HideInInspector] public bool isRunning;
        [HideInInspector] public bool isAttacking = false;

        public Sprite htmlSprite;
        public Sprite cssSprite;
        public Sprite jsSprite;
        public Sprite buffSprite;
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
            animBaseScale = anim != null ? anim.transform.localScale : Vector3.one;

            HP = maxHP;

            stateMachine = new StateMachine();
            idleState = new IdleState(this, stateMachine);
            walkState = new WalkState(this, stateMachine);
            runState = new RunState(this, stateMachine);
            stateMachine.Initialize(idleState);

            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전 방지
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

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
            if (canMove)
            {
                if (Input.GetKeyDown(KeyCode.C) && isGrounded)
                {
                    Jump();
                }
                moveInput = Input.GetAxisRaw("Horizontal");
            }
            else
            {
                moveInput = 0; 
            }
            if (moveInput != 0)
                sr.flipX = moveInput < 0;
        
            CheckSkillInput();
            stateMachine.Update();
            anim.SetBool("isGrounded", isGrounded);

            hpSlider.value = HP;
            
            // Update skill cooldown UI if needed
            // Example: UpdateCooldownUI();
        }

        void FixedUpdate()
        {
            CheckGround();
            stateMachine.FixedUpdate();
            HandleJump();
        }

        public void CheckGround()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
            
            // Reset jump when landing
            if (!wasGrounded && isGrounded) {
                isJumping = false;
            }
        }

        public void ReduceShield()
        {
            if (shield > 0)
            {
                shield--;  
                
                if (activeShields[shield] != null)
                {
                    Destroy(activeShields[shield]);
                    activeShields[shield] = null;
                }

                if (shield <= 0)
                {
                    Debug.Log("쉴드가 모두 소진되었습니다.");
                }
            }
        }

        private Coroutine damageFlashRoutine;
        private IEnumerator DamageFlash() {
            // Store the original color
            Color originalColor = sr.color;
            
            // Set to red
            sr.color = Color.red;
            
            // Wait for the flash duration
            yield return new WaitForSeconds(0.1f);
            
            // Only reset if we're still in the damaged state
            if (sr.color == Color.red) {
                sr.color = originalColor;
            }
            
            // Clear the routine reference
            damageFlashRoutine = null;
        }   
        
        public void TakeDamage(int damage)
        {
            if(shield > 0) {
                ReduceShield();
                Debug.Log(shield);
                return;
            }
            Debug.Log("민기 죽는다");
            
            // Stop any existing flash routine
            if (damageFlashRoutine != null) {
                StopCoroutine(damageFlashRoutine);
                sr.color = Color.white; // Reset to default color
            }
            
            // Start new flash routine
            damageFlashRoutine = StartCoroutine(DamageFlash());
            
            HP -= damage;
            if (HP <= 0) Die();
        }
        void Jump() {
            if (isGrounded && Time.time > lastJumpTime + jumpCooldown) {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isJumping = true;
                jumpTimeCounter = jumpTime;
                lastJumpTime = Time.time;
            }
        }
        
        void HandleJump() {
        
            if (rb.linearVelocity.y < 0) {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            } else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.C)) {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            
            if (isJumping) {
                if (jumpTimeCounter > 0 && Input.GetKey(KeyCode.C)) {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Sqrt(jumpForce * 2f * Mathf.Abs(Physics2D.gravity.y)));
                    jumpTimeCounter -= Time.deltaTime;
                } else {
                    isJumping = false;
                }
            }
            if (isGrounded && !isJumping) {
                jumpTimeCounter = 0;
            }
        }
        void Die()
        {
            anim.SetTrigger("die");
            rb.linearVelocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            SceneManager.LoadScene("GameOver");
            Destroy(gameObject, 0.5f); 
        }

        void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
            }
        }
        private void CheckSkillInput()
        {
            
            if(Input.GetKeyDown(KeyCode.Space)) {
                if(GameManager.Instance.canModeChange) {
                    GameManager.Instance.isAnA = !GameManager.Instance.isAnA;  
                    Debug.Log("Mode Changed : isAnA? ->" + GameManager.Instance.isAnA);
                }
            }

            if(Input.GetKeyDown(KeyCode.X)) {
                if (SkillManager.IsSkillReady(SkillManager.SkillType.NormalAttack))
                {
                    AudioManager.Instance.PlayAttackSound();
                    StartCoroutine(SkillManager.UseNormalAttack(this));
                }
                else
                {
                    // Optional: Play a sound or show feedback that skill is on cooldown
                    Debug.Log("Normal Attack on cooldown!");
                }
                return;
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("used skill1");
                Debug.Log("not in circle: " + GameManager.Instance.isNotinCircle + ", isAnA: " + GameManager.Instance.isAnA);
                if(!GameManager.Instance.isNotinCircle) {
                    Debug.Log("in circle");
                    if (GameManager.Instance.isAnA) {
                        if(SkillManager.IsSkillReady(SkillManager.SkillType.AnASkill1)) {
                            StartCoroutine(SkillManager.UseAnASkill1(this));
                        } else {
                            Debug.Log("Ana Skill 1 on cooldown!");
                        }
                    } else {
                        if(SkillManager.IsSkillReady(SkillManager.SkillType.TapieSkill1)) {
                            StartCoroutine(SkillManager.UseTapieSkill1(this));
                        } else {
                            Debug.Log("Tapie Skill 1 on cooldown!");
                        }
                    }
                }
            }
            
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                Debug.Log("used skill2");
                if(!GameManager.Instance.isNotinCircle) {
                    Debug.Log("in circle");
                    if (GameManager.Instance.isAnA) {
                        if(SkillManager.IsSkillReady(SkillManager.SkillType.AnASkill2)) {
                            StartCoroutine(SkillManager.UseAnASkill2(this));
                        } else {
                            Debug.Log("Ana Skill 2 on cooldown!");
                        }
                    } else {
                        if(SkillManager.IsSkillReady(SkillManager.SkillType.TapieSkill2)) {
                            StartCoroutine(SkillManager.UseTapieSkill2(this));
                        } else {
                            Debug.Log("Tapie Skill 2 on cooldown!");
                        }
                    }
                }
            }
        }

        public Vector2 GetMoveDirection()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 direction = new Vector2(horizontal, vertical);
            return direction.sqrMagnitude > 1f ? direction.normalized : direction;
        }
    }
