using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    
    [Header("References")]
    public Transform destination;  // 목적지 위치 (인스펙터에서 설정)
    public float teleportDelay = 0.5f;
    
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.F;
    public float interactionRange = 2f;
    
    private bool isPlayerInRange = false;
    private bool isTeleporting = false;
    private Player player;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey) && !isTeleporting)
        {
            StartCoroutine(Teleport());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.GetComponent<Player>();
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            
        }
    }

    private IEnumerator Teleport()
    {
        if (player == null || destination == null) yield break;
        
        isTeleporting = true;
        Debug.Log("player.position: " + player.transform.position + "  destination.position: " + destination.position);
        
        player.transform.position = destination.position;
        
        GameManager.Instance.StageUp();
        
        isTeleporting = false;
    }

    // 에디터에서 목적지 위치 시각화
    private void OnDrawGizmos()
    {
        if (destination != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destination.position);
            Gizmos.DrawWireSphere(destination.position, 0.5f);
        }
    }
}