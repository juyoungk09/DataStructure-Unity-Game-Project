using UnityEngine;

public class EMWave : MonoBehaviour
{
    // 발사체 속성
    public float speed;
    public int damage;
    private Vector2 direction;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    // Visual effects
    private float lifetime = 0f;
    private float maxLifetime = 6f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, maxLifetime);
    }

    // 초기화 메서드
    public void Initialize(Vector2 dir, float spd, int dmg)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        
        // Rotate to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        
        // Fade out over time
        if (spriteRenderer != null && lifetime > maxLifetime * 0.5f)
        {
            float fadeProgress = (lifetime - maxLifetime * 0.5f) / (maxLifetime * 0.5f);
            Color color = spriteRenderer.color;
            color.a = 1f - fadeProgress;
            spriteRenderer.color = color;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    // 플레이어 충돌 시 데미지
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit player
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<Player>(out Player player))
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
        
        // Destroy on collision with walls (only check if tag exists)
        try
        {
            if (collision.CompareTag("Wall"))
            {
                Destroy(gameObject);
                return;
            }
        }
        catch (UnityException)
        {
            // Wall tag doesn't exist, ignore
        }
        
        // Alternative: check by layer name
        int wallLayer = LayerMask.NameToLayer("Wall");
        if (wallLayer != -1 && collision.gameObject.layer == wallLayer)
        {
            Destroy(gameObject);
        }
    }
}