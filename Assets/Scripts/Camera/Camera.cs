using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform[] stageCameraPositions; // 스테이지별 카메라 위치
    private Transform targetPosition;

    void Start()
    {
        UpdateCameraPosition(); // 시작 시 위치 맞춤
    }

    public void UpdateCameraPosition()
    {
        int stageIndex = GameManager.Instance.getStage() - 1; // 0부터 시작
        if (stageIndex >= 0 && stageIndex < stageCameraPositions.Length)
        {
            targetPosition = stageCameraPositions[stageIndex];
            // 순간 이동
            transform.position = targetPosition.position;
        }
    }
}
