using UnityEngine;

public class HpBar : MonoBehaviour
{
    public Camera mainCamera;

    void LateUpdate()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // HPBar가 카메라를 향하게 회전
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }
}