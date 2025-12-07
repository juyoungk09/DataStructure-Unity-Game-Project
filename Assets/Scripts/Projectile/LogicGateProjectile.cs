using UnityEngine;

public class LogicGateProjectile : MonoBehaviour
{
    public enum GateType { OR, AND, NOT }
    
    [Header("Gate Settings")]
    public GateType gateType = GateType.OR;
    public float lifetime = 5f;
    
    [Header("Damage Settings")]
    public int orGateDamage = 15;
    public int andGateDamage = 20;
    public int notGateDamage = 25;
    
    public int Damage {
        get {
            return gateType switch
            {
                GateType.OR => orGateDamage,
                GateType.AND => andGateDamage,
                GateType.NOT => notGateDamage,
                _ => orGateDamage
            };
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(Damage);
            }
            Destroy(gameObject);
        }
    }
}