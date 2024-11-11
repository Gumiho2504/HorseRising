using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform player; // The player's transform to follow
    public Vector3 offset; // The offset distance between the player and camera
    public float smoothSpeed = 0.125f; // The smooth speed for camera movement

    void LateUpdate()
    {
        
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Optional: Make the camera look at the player
        transform.LookAt(player);
    }
}
