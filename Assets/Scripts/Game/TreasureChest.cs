using UnityEngine;
using System.Collections;

public class TreasureChest : MonoBehaviour
{
    public enum ChestRarity { Common, Rare, Epic, Legendary }
    
    [Header("Chest Settings")]
    public ChestRarity rarity = ChestRarity.Common;
    public int minGold = 10;
    public int maxGold = 50;
    public GameObject[] possibleItems; // 드롭 가능한 아이템 프리팹 배열
    public float itemDropChance = 0.5f; // 아이템 드롭 확률 (0~1)
    
    private bool isOpened = false;
    private Animator animator;
    
    void Start()
    {
        // animator = GetComponent<Animator>();
    }
    
    public void OpenChest()
    {
        if (isOpened) return;
        
        isOpened = true;
        
        int goldAmount = Random.Range(minGold, maxGold + 1);
        GameManager.Instance.AddGold(goldAmount);
        
        // if (Random.value <= itemDropChance && possibleItems.Length > 0)
        // {
        //     int randomIndex = Random.Range(0, possibleItems.Length);
        //     Instantiate(possibleItems[randomIndex], transform.position, Quaternion.identity);
        // }
        
        // 효과음 재생
        // AudioManager.Instance.PlaySFX("ChestOpen");
        
        // 일정 시간 후 제거
        StartCoroutine(DestroyAfterDelay(1f));
    }
    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            OpenChest();
        }
    }
}