using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    public bool isFinalRoundPortal = false;   // 마지막 라운드 포탈 여부
    public StageController stageController;   // StageController 참조
    public float teleportDelay = 0.2f;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTeleporting && other.CompareTag("Player"))
        {
            StartCoroutine(Teleport(other.transform));
        }
    }

    private IEnumerator Teleport(Transform player)
    {
        isTeleporting = true;

        yield return new WaitForSeconds(teleportDelay);

        // 플레이어 이동: 현재 포탈 위치 → 다음 라운드 위치
        if (!isFinalRoundPortal)
        {
            stageController.EndRound(); // 다음 라운드 포탈 생성
        }
        else
        {
            stageController.GoNextStage(); // 마지막 라운드면 씬 전환
        }

        isTeleporting = false;
    }
}
