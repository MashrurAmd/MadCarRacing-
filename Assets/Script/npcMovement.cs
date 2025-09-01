using UnityEngine;

public class NPCRotate : MonoBehaviour
{
    public float rotationAngle = 90f;   // How much to rotate each time
    public float interval = 10f;        // How often to rotate (seconds)
    public float rotationSpeed = 180f;  // Degrees per second for smooth rotation

    private float nextRotationTime;
    private Quaternion targetRotation;
    private bool isRotating = false;

    void Start()
    {
        nextRotationTime = Time.time + interval;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // Check if it's time to start a new rotation
        if (!isRotating && Time.time >= nextRotationTime)
        {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + rotationAngle, 0);
            isRotating = true;
            nextRotationTime = Time.time + interval; // schedule next rotation
        }

        // Smoothly rotate towards target
        if (isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                isRotating = false;
        }
    }
}

