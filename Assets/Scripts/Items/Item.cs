// Item.cs
using UnityEngine;

public enum ItemType
{
    Doritos,    // 도리토스 (체력 회복)
    Monster1,   // 몬스터 에너지드링크 (공격력 증가)
    Monster3    // 몬스터 에너지드링크 (이동속도 증가)
}

[System.Serializable]
public class ItemEffect
{
    public ItemType type;
    public string itemName = "아이템";
    [TextArea] public string description = "아이템 설명";
    public Sprite icon;
    public int price = 100;
    public float duration = 10f;
    public float effectValue = 1f;
}

public class Item : MonoBehaviour
{
    [Header("아이템 효과 설정")]
    [SerializeField] private ItemEffect itemEffect;
    
    private bool isUsed = false;
    private bool isPlayerInRange = false;
    private Player player;
    
    public ItemEffect Effect => itemEffect;

    private void Start()
    {
        // 아이템 타입에 따라 기본값 설정
        if (itemEffect == null) 
        {
            Debug.LogWarning($"{name} : ItemEffect가 할당되지 않아 기본값으로 생성됩니다.");
            itemEffect = new ItemEffect();
        }

        // type에 맞게 기본값 자동 세팅 (Inspector에서 값 덮어쓰기 방지)
        switch (itemEffect.type)
        {
            case ItemType.Doritos:
                if (string.IsNullOrEmpty(itemEffect.itemName))
                    itemEffect.itemName = "도리토스";
                if (string.IsNullOrEmpty(itemEffect.description))
                    itemEffect.description = "체력을 회복시켜주는 간식";
                if (itemEffect.price == 0) itemEffect.price = 150;
                if (itemEffect.effectValue == 0) itemEffect.effectValue = 30f;
                if (itemEffect.duration == 0) itemEffect.duration = 0; 
                break;

            case ItemType.Monster1:
                if (string.IsNullOrEmpty(itemEffect.itemName))
                    itemEffect.itemName = "몬스터 에너지드링크 1";
                if (string.IsNullOrEmpty(itemEffect.description))
                    itemEffect.description = "일정 시간 동안 공격력 증가";
                if (itemEffect.price == 0) itemEffect.price = 200;
                if (itemEffect.effectValue == 0) itemEffect.effectValue = 1.5f;
                if (itemEffect.duration == 0) itemEffect.duration = 10f;
                break;

            case ItemType.Monster3:
                if (string.IsNullOrEmpty(itemEffect.itemName))
                    itemEffect.itemName = "몬스터 에너지드링크 3";
                if (string.IsNullOrEmpty(itemEffect.description))
                    itemEffect.description = "일정 시간 동안 이동속도 증가";
                if (itemEffect.price == 0) itemEffect.price = 250;
                if (itemEffect.effectValue == 0) itemEffect.effectValue = 1.3f;
                if (itemEffect.duration == 0) itemEffect.duration = 8f;
                break;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !isUsed)
        {
            TryBuyItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isUsed) return;

        player = collision.GetComponent<Player>();
        if (player != null)
        {
            isPlayerInRange = true;
            // 상점 UI에 아이템 정보 표시
            // GameManager.Instance?.ShowShopItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            isPlayerInRange = false;
            // GameManager.Instance?.HideShopItem();
            GameManager.Instance.ShowMessage("");
        }
    }

    private void TryBuyItem()
    {
        if (GameManager.Instance.gold >= itemEffect.price)
        {
            GameManager.Instance.gold -= itemEffect.price;
            GameManager.Instance.UpdateGameUI();
            ApplyEffect();
            DisableItem();

        }
        else
        {
            GameManager.Instance?.ShowMessage("골드가 부족합니다!");
        }
    }

    private void ApplyEffect()
    {
        switch (itemEffect.type)
        {
            case ItemType.Monster1:
                player.HP = Mathf.Min(player.HP + (int)itemEffect.effectValue, player.maxHP);
                break;
                
            case ItemType.Doritos:
                float originalDamage = player.attackDamage;
                player.attackDamage = Mathf.RoundToInt(originalDamage * itemEffect.effectValue);
                
                break;
                
            case ItemType.Monster3:
                SkillManager.NORMAL_ATTACK_CD-=0.1f;
            
                break;
        }
    }

    // private void ShowItemEffectText()
    // {
    //     string message = itemEffect.type switch
    //     {
    //         ItemType.Doritos => $"체력 +{itemEffect.effectValue} 회복!",
    //         ItemType.Monster1 => $"공격력 {itemEffect.effectValue}x 증가! ({itemEffect.duration}초)",
    //         ItemType.Monster3 => $"이동속도 {itemEffect.effectValue}x 증가! ({itemEffect.duration}초)",
    //         _ => string.Empty
    //     };
        
    //     UIManager.Instance?.ShowMessage(message);
    // }

    private void DisableItem()
    {
        isUsed = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
    }
}