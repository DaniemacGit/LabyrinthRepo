using UnityEngine;
using UnityEngine.SceneManagement;

public class GyroControl : MonoBehaviour
{
    // ball physics values
    Rigidbody ballRigidbody;
    [Header("Ball physics settings")]
    public float ballDrag;
    public float ballAngularDrag;
    public PhysicsMaterial ballPhysicsMaterial;
    public float ballFriction;
    public float gravity;
    
    // board tilt values
    float maxTiltAngle = 10;
    float tiltingSmoothness = 2;
    float tiltSens = 50;

    // rotation calculation variables
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
        // accelerometer input
        Vector3 tilt = Input.acceleration;
        float tiltX = -tilt.x; // Left-right tilt
        float tiltZ = tilt.y; // Forward-backward tilt

        // map phone tilt to board rotation
        float angleX = Mathf.Clamp(tiltZ * tiltSens, -maxTiltAngle, maxTiltAngle); // forward/back tilt
        float angleZ = Mathf.Clamp(tiltX * tiltSens, -maxTiltAngle, maxTiltAngle); // left/right tilt

        // create variable for desired rotation and rotate
        targetRotation = Quaternion.Euler(angleX, 0f, angleZ);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltingSmoothness));
    }
}
