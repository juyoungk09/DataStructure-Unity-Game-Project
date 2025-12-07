using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CameraController : MonoBehaviour
{
    [Header("카메라 위치 부모")]
    public Transform cameraPositionsParent;
    private List<Transform> stageCameraPositions = new List<Transform>();
    private Transform targetPosition;

    void Awake()
    {
        // Get all direct children of cameraPositionsParent
        stageCameraPositions = cameraPositionsParent.GetComponentsInChildren<Transform>()
                                                  .Where(t => t != cameraPositionsParent)
                                                  .ToList();
        
        Debug.Log($"Initialized {stageCameraPositions.Count} camera positions");
    }

    void Start()
    {
        UpdateCameraPosition();
    }

    public void UpdateCameraPosition()
    {
        int stageIndex = GameManager.Instance.stageCount;
        if (stageIndex >= 0 && stageIndex < stageCameraPositions.Count)
        {
            targetPosition = stageCameraPositions[stageIndex];
            transform.position = targetPosition.position;
        }
        else
        {
            Debug.LogError($"Invalid stage index: {stageIndex}. Total positions: {stageCameraPositions.Count}");
        }
    }
}