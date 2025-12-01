using UnityEngine;

using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 전역 접근용
    public Player player;
    public CameraController cameraController;
    public int maxHP;
    public int playerHP = 10;
    public int gold;
    public int stageCount = 0;
    // public int [,] enemyCount = 
    // {
    //     {1,2,3,4,5},
    //     {6,7,8,9,10},
    //     {11,12,13,14,15},
    //     {16,17,18,19,20},
    //     {21,21,22,23,24}
    // };
    [HideInInspector]public bool isAnA = false;
    [HideInInspector]public bool isNotinCircle = false;
    void Awake()
    {
        // 이미 존재하면 자신을 삭제
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴 X
    }
    // void Start()
    // {
    //     StartCoroutine(StageUpRoutine());
    // }
    //   private IEnumerator StageUpRoutine()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(2f); // 10초 대기
    //         StageUp();
    //         Camera.main.GetComponent<CameraController>()?.UpdateCameraPosition();
    //     }
    // }
    public int getRound() {
        return stageCount/5+1;   
    }
    public int getStage() {
        return stageCount%5+1;
    }
    public void OnEnemyDead()
    {
        // enemyCount--;

        // if (enemyCount <= 0)
        // {
        //     StageClear();
        // }
    }
    public void StageClear()
    {
        Debug.Log("스테이지 클리어!");

        stageCount++;

        Camera.main.GetComponent<CameraController>()?.UpdateCameraPosition();
        // EnemyManager.Instance.SpawnStage(stageCount);
        // UIManager.Instance.UpdateStageText(getRound(), getStage());
    }

}
