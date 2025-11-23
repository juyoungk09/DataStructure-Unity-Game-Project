using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    public Transform target;     
    public float smoothSpeed = 0f;    
    public float offsetX = -3f;   
    
    void Start()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");   
        target = Player.GetComponent<Transform>();
    }
    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(target.position.x + offsetX, transform.position.y, transform.position.z);
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
