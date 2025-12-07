using System.Collections;
using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    public float scale = 1.5f;
    public float lifeTime = 0.8f;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.2f;
    public float fadeTime = 0.4f;
    private SpriteRenderer spriteRenderer;
    private float elapsed;

    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(scale, scale, 1f);
        Destroy(gameObject, lifeTime);

        
        // if (AudioManager.Instance != null)
        // {
        //     AudioManager.Instance.Play("Thunder");
        // }
        
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraController controller = mainCamera.GetComponent<CameraController>();
            if (controller != null)
            {
                controller.StartShake(shakeDuration, shakeMagnitude);
            }
        }

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        if (spriteRenderer == null || fadeTime <= 0f) yield break;

        Color startColor = spriteRenderer.color;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            yield return null;
        }
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }
}