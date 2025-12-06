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

    private int currentRound;
    private GameObject currentPortal; 

    void Awake()
    {
        portals = portalsParent.GetComponentsInChildren<Transform>()
                               .Where(t => t != portalsParent)
                               .ToArray();
        
    }

    void Start()
    {
        
        currentRound = GameManager.Instance.getRound();
    }

    // 라운드 종료 시 호출
    public void EndRound()
    {
        currentRound++;

        if (currentRound < portals.Length)
        {
            SpawnNextPortal();
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
        portalComp.stageController = this;
    }

}
