using UnityEngine;

public class SmoothFollowCanvas : MonoBehaviour
{
    public Transform cameraTransform;

    public float distance = 0.7f;
    public float damping = 0.8f;
    public float followSpeed = 10f;

    public float timeScale = 0.25f; // 👈 ESTE ES EL CONTROL GLOBAL

    Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 targetPosition =
            cameraTransform.position + forward * distance;

        // 👇 engañamos al tiempo
        float scaledDamping = damping / timeScale;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            scaledDamping,
            followSpeed
        );

        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180f, 0);
    }
}
