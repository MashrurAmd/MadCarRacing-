using UnityEngine;
using System.Collections.Generic;

public class CarCarousel : MonoBehaviour
{
    public List<Transform> cars;
    public Vector3 centerPos = Vector3.zero;
    public Vector3 leftPos = new Vector3(-10f, 0f, 0f);
    public Vector3 rightPos = new Vector3(10f, 0f, 0f);
    public float slideSpeed = 5f;

    [Header("Rotation (degrees/sec)")]
    public Vector3 rotationAxis = Vector3.up;      // axis to rotate around (usually Vector3.up)
    public float centerRotationSpeed = 45f;       // speed when car is in center
    public float sideRotationSpeed = 0f;          // speed for offscreen/side cars

    private int currentIndex = 0;
    private int targetIndex = 0;
    private bool isMoving = false;
    private Vector3 currentTargetPos;

    const float centerEpsilon = 0.05f; // how close to center counts as "centered"

    void Start()
    {
        if (cars == null || cars.Count == 0) return;

        // place all cars offscreen right then put first car in center
        foreach (var car in cars)
            if (car != null) car.position = rightPos;

        if (cars[0] != null) cars[0].position = centerPos;
    }

    void Update()
    {
        // Sliding logic (unchanged)
        if (isMoving)
        {
            if (cars[currentIndex] != null)
                cars[currentIndex].position = Vector3.MoveTowards(cars[currentIndex].position, currentTargetPos, slideSpeed * Time.deltaTime);

            if (cars[targetIndex] != null)
                cars[targetIndex].position = Vector3.MoveTowards(cars[targetIndex].position, centerPos, slideSpeed * Time.deltaTime);

            bool curReached = cars[currentIndex] != null && Vector3.Distance(cars[currentIndex].position, currentTargetPos) < 0.001f;
            bool tgtReached = cars[targetIndex] != null && Vector3.Distance(cars[targetIndex].position, centerPos) < 0.001f;

            if (curReached && tgtReached)
            {
                currentIndex = targetIndex;
                isMoving = false;
            }
        }

        // Rotation: centered car rotates with centerRotationSpeed, others with sideRotationSpeed
        if (cars != null)
        {
            for (int i = 0; i < cars.Count; i++)
            {
                var car = cars[i];
                if (car == null) continue;

                float dist = Vector3.Distance(car.position, centerPos);
                float speed = (dist <= centerEpsilon) ? centerRotationSpeed : sideRotationSpeed;

                if (Mathf.Abs(speed) > 0.0001f)
                    car.Rotate(rotationAxis * speed * Time.deltaTime, Space.Self);
            }
        }
    }

    // UI -> Left button (shows next car: current goes left, next comes from right)
    public void ShowNextCar()
    {
        if (isMoving || cars == null || cars.Count < 2) return;

        targetIndex = (currentIndex + 1) % cars.Count;
        if (cars[targetIndex] != null) cars[targetIndex].position = rightPos;
        currentTargetPos = leftPos;
        isMoving = true;

        //print index for debugging
        //Debug.Log("index: " + targetIndex);
    }

    // UI -> Right button (shows previous car: current goes right, prev comes from left)
    public void ShowPreviousCar()
    {
        if (isMoving || cars == null || cars.Count < 2) return;

        targetIndex = (currentIndex - 1 + cars.Count) % cars.Count;
        if (cars[targetIndex] != null) cars[targetIndex].position = leftPos;
        currentTargetPos = rightPos;
        isMoving = true;
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }
}
