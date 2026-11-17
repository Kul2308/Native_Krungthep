using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;        
    public float startFollowX = 0f; 

    float startY;
    float startZ;

    void Start()
    {
        startY = transform.position.y;
        startZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            if (player.position.x > startFollowX)
            {
                transform.position = new Vector3(player.position.x, startY, startZ);
            }
        }
    }
}