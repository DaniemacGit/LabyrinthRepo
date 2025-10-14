using UnityEngine;

public class GyroControl : MonoBehaviour
{
    
    public float maxTilt = 20;
    public float smoothness = 5;

    private Quaternion targetRotation;

    void Update()
    {
        // Get accelerometer input
        Vector3 tilt = Input.acceleration;

        // --- Orientation Mapping ---
        // Assuming landscape mode with screen facing up
        // and home button (or camera notch) on the RIGHT.
        float tiltX = -tilt.x; // Left-right tilt
        float tiltZ = tilt.y; // Forward-backward tilt

        // Map tilt to rotation angles
        float angleX = Mathf.Clamp(tiltZ * maxTilt, -maxTilt, maxTilt); // forward/back tilt
        float angleZ = Mathf.Clamp(tiltX * maxTilt, -maxTilt, maxTilt); // left/right tilt

        // Create target rotation
        targetRotation = Quaternion.Euler(angleX, 0f, angleZ);

        // Smoothly rotate the board
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothness);
    }
}
