using UnityEngine;
using UnityEngine.SceneManagement;

public class GyroControl : MonoBehaviour
{
    Rigidbody ballRigidbody;
    [Header("Ball physics settings")]
    public float ballDrag;
    public float ballAngularDrag;
    public PhysicsMaterial ballPhysicsMaterial;
    public float ballFriction;
    public float gravity;
    
    float maxTiltAngle = 10;
    float tiltingSmoothness = 2;
    float tiltSens = 50;

    private Quaternion targetRotation;

    Rigidbody rb;

    private void Start()
    {
        // fill ball physics values with the values from the inspector
        ballRigidbody = GameObject.Find("Ball").GetComponent<Rigidbody>();
        ballRigidbody.linearDamping = ballDrag;
        ballRigidbody.angularDamping = ballAngularDrag;
        ballPhysicsMaterial.dynamicFriction = ballFriction;
        ballPhysicsMaterial.staticFriction = ballFriction;
        Physics.gravity = new Vector3(0, gravity, 0);

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get accelerometer input
        Vector3 tilt = Input.acceleration;

        // --- Orientation Mapping ---
        // Assuming landscape mode with screen facing up
        // and home button (or camera notch) on the RIGHT.
        float tiltX = -tilt.x; // Left-right tilt
        float tiltZ = tilt.y; // Forward-backward tilt

        // Map tilt to rotation angles
        float angleX = Mathf.Clamp(tiltZ * tiltSens, -maxTiltAngle, maxTiltAngle); // forward/back tilt
        float angleZ = Mathf.Clamp(tiltX * tiltSens, -maxTiltAngle, maxTiltAngle); // left/right tilt

        // Create target rotation
        targetRotation = Quaternion.Euler(angleX, 0f, angleZ);

        // Smoothly rotate the board
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltingSmoothness));
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 32;

        GUI.Label(new Rect(10, 100, 4000, 400), " " + transform.rotation.eulerAngles.x);
        GUI.Label(new Rect(10, 200, 4000, 400), " " + transform.rotation.eulerAngles.z);
        GUI.Label(new Rect(10, 300, 4000, 400), " " + GameObject.Find("Ball").GetComponent<Rigidbody>().linearVelocity);
    }
}
