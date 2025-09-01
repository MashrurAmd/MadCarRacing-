using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -8);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float zoomOutAmount = 5f;
    [SerializeField] private float maxSpeed = 200f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float resetDelay = 10f;

    private Rigidbody carRb;
    private Transform target;
    private Vector3 currentOffset;
    private Vector3 velocity = Vector3.zero;

    private float yaw = 0f;
    private float lastMouseInputTime = 0f;

    private void Start()
    {
        currentOffset = offset;
        // Find the active car at start
        SetTarget(FindObjectOfType<CarManager>().GetActiveCar());
    }

    private void LateUpdate()
    {
        if (target == null || carRb == null) return;

        float speed = carRb.velocity.magnitude * 3.6f; // km/h
        float zoomFactor = Mathf.Clamp01(speed / maxSpeed);
        Vector3 desiredOffset = offset + new Vector3(0, 0, -zoomOutAmount * zoomFactor);

        // --- Mouse / Touch input tracking ---
        float mouseX = 0f;

#if UNITY_EDITOR || UNITY_STANDALONE
        mouseX = Input.GetAxis("Mouse X");
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            mouseX = Input.GetTouch(0).deltaPosition.x * 0.05f;
#endif

        if (Mathf.Abs(mouseX) > 0.01f)
        {
            yaw += mouseX * mouseSensitivity;
            lastMouseInputTime = Time.time;
        }

        bool shouldReset = (Time.time - lastMouseInputTime) > resetDelay;
        if (shouldReset)
        {
            float carYaw = target.eulerAngles.y;
            yaw = Mathf.LerpAngle(yaw, carYaw, Time.deltaTime * smoothSpeed);
        }

        Quaternion rotation = Quaternion.Euler(15f, yaw, 0f);
        currentOffset = Vector3.Lerp(currentOffset, desiredOffset, Time.deltaTime * smoothSpeed);

        Vector3 desiredPosition = target.position + rotation * currentOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.1f);

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    // Called by CarManager whenever car changes
    public void SetTarget(CarController car)
    {
        if (car == null) return;
        target = car.transform;
        carRb = car.GetComponent<Rigidbody>();
        yaw = target.eulerAngles.y;
    }
}
