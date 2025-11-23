using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class StageController : MonoBehaviour
{
    [Header("포탈 위치 부모")]
    public Transform portalsParent;       
    private Transform[] portals;          

    [Header("포탈 프리팹")]
    public GameObject portalPrefab;

    private int currentRound = 0;
    private GameObject currentPortal; 

    void Awake()
    {
        portals = portalsParent.GetComponentsInChildren<Transform>()
                               .Where(t => t != portalsParent)
                               .ToArray();
    }

    void Start()
    {
        SpawnNextPortal();
    }

    // 라운드 종료 시 호출
    public void EndRound()
    {
        currentRound++;

        if (currentRound < portals.Length)
        {
            SpawnNextPortal();
        }
        else
        {
            GoNextStage();
        }
    }

    void SpawnNextPortal(bool isFinal = false)
    {
        // 기존 포탈 제거
        if (currentPortal != null)
            Destroy(currentPortal);

        Transform spawnPos = portals[currentRound];

        currentPortal = Instantiate(portalPrefab, spawnPos.position, Quaternion.identity);

        var portalComp = currentPortal.GetComponent<Portal>();
        portalComp.isFinalRoundPortal = isFinal;
        portalComp.stageController = this;
    }

    public void GoNextStage()
    {
        int nextStageIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextStageIndex);
    }
}
