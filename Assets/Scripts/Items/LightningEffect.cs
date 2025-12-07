using UnityEngine;
public class LightningEffect : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 1f); // 1초 후 자동 제거
    }
}