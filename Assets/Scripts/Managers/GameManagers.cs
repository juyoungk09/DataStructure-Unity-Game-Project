using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 전역 접근용
    public Player player;
    public int maxHP;
    public int playerHP = 10;
    public int gold;
    public int roundCount = 0;
    
    
    void Awake()
    {
        // 이미 존재하면 자신을 삭제하는 거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 싱글톤으로 설정하는 거
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 되어도 파괴되지 않음
    }
    public void RoundUp() {
        roundCount++;
    }
    public int getStage() {
        return roundCount/5+1;   
    }
    public int getRound() {
        return roundCount%5+1;
    }
}
