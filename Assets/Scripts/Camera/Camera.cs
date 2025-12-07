using UnityEngine;
using System.Collections;
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

    private Coroutine shakeRoutine;
    private bool isShaking = false;

    public void StartShake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        isShaking = true;
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            transform.position = new Vector3(
                originalPos.x + offsetX,
                originalPos.y + offsetY,
                originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        isShaking = false;
        shakeRoutine = null;
    }
}