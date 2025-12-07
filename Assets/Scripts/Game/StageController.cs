using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class StageController : MonoBehaviour
{
    [Header("포탈 위치 부모")]
    public Transform portalsParent;
    public Transform teleportPos;
    public Transform[] teleportPosArray;
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
        teleportPosArray = teleportPos.GetComponentsInChildren<Transform>()
                                .Where(t => t != teleportPos)
                                .ToArray();
        
    }

    void Start()
    {
        
        currentRound = GameManager.Instance.stageCount;
    }

    // 라운드 종료 시 호출
    public void EndRound()
    {
        Debug.Log("currentRound: " + currentRound + "  portals.Length: " + portals.Length);
        if (currentRound < portals.Length)
        {
            SpawnNextPortal();
            currentRound++;
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
        portalComp.destination = teleportPosArray[currentRound];
    }

}
