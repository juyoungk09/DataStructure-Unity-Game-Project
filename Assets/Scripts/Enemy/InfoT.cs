using UnityEngine;
using System.Collections;

public class InfoT : EnemyBase
{
    [Header("InfoT Settings")]
    public GameObject emWavePrefab;
    public float waveCooldown = 10f;
    public int waveCount = 5;
    public float waveInterval = 0.5f;
    public float waveSpeed = 5f;
    
    private float lastWaveTime;
    private bool isCastingWave = false;

    protected override void Start()
    {
        base.Start();
        maxHP = 600;
        currentHP = maxHP;
        attackDamage = 15;
        moveSpeed = 1.5f;
        chaseRange = 10f;
        attackRange = 6f;
    }

    protected override void Update()
    {
        base.Update();
        
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= chaseRange && !isCastingWave)
        {
            if (Time.time >= lastWaveTime + waveCooldown)
            {
                StartCoroutine(CastEMWave());
            }
        }
    }

    private IEnumerator CastEMWave()
    {
        isCastingWave = true;
        anim.SetTrigger("Cast");
        yield return new WaitForSeconds(1f); // Casting time
        
        for (int i = 0; i < waveCount; i++)
        {
            // Create wave
            GameObject wave = Instantiate(emWavePrefab, transform.position, Quaternion.identity);
            EMWave waveScript = wave.GetComponent<EMWave>();
            
            if (waveScript != null)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                waveScript.Initialize(direction, waveSpeed, attackDamage);
            }
            
            yield return new WaitForSeconds(waveInterval);
        }
        
        lastWaveTime = Time.time;
        isCastingWave = false;
    }
}

public class EMWave : MonoBehaviour
{
    public float speed;
    public int damage;
    private Vector2 direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f); // Auto-destroy after 5 seconds
    }

    public void Initialize(Vector2 dir, float spd, int dmg)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        
        // Rotate to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}