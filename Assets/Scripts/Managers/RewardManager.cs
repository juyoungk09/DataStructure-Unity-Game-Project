using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    [Header("보상 상자 프리팹")]
    public GameObject chestPrefab;   // TreasureChest 프리팹

    void Awake()
    {
        Instance = this;
    }

    public void SpawnReward()
    {
        // 현재 스테이지 번호에 맞게 위치 선택
        int stage = GameManager.Instance.stageCount;

        Transform spawnPos = ChestPositionManager.Instance.GetChestPosition(stage);

        Instantiate(chestPrefab, spawnPos.position, Quaternion.identity);
    }
}
