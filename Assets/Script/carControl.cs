using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

public class CarController : MonoBehaviour
{
    // Inputs
    private float horizontalInput;
    private float currentSteerAngle, currentBrakeForce;
    private bool isDrifting;

    public Vector3 startPosition;
    private Quaternion startRotation;

    public int score;
    public TMP_Text scoreText;

    // Settings
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;  // 🚗 braking power
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float downforce = 100f;

    // Drift settings
    [SerializeField] private float driftStiffness = 0.5f;
    [SerializeField] private float normalStiffness = 2f;
    [SerializeField] private float driftTorque = 500f;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheel Meshes
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    [SerializeField] private float accelerationRate = 5f;
    [SerializeField] private float decelerationRate = 5f;
    [SerializeField] private float maxSpeed = 200f;

    private float currentTorque = 0f;

    // ✅ Mobile button states
    [HideInInspector] public bool isAccelerating = false;
    [HideInInspector] public bool isBraking = false;
    [HideInInspector] public bool isTurningLeft = false;
    [HideInInspector] public bool isTurningRight = false;

    // Rigidbody reference
    private Rigidbody rb;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // stability
        AdjustFriction(normalStiffness);

        // Save starting transform
        startPosition = transform.position;
        startRotation = transform.rotation;

        Application.targetFrameRate = 60;
    }


    private void FixedUpdate()
    {
        GetInputFromButtons();
        HandleMotor();
        HandleSteering();
        HandleDrift();
        UpdateWheels();

        // Add downforce
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
        

        if (transform.position.y < -5f)
        {
            ResetCar();
        }
    }

    private void GetInputFromButtons()
    {
        // Reset brake each frame
        currentBrakeForce = 0f;

        // Vertical input (accelerate)
        float verticalInput = isAccelerating ? 1f : 0f;

        // Smooth acceleration
        float targetTorque = verticalInput * motorForce;
        if (verticalInput > 0.01f)
            currentTorque = Mathf.MoveTowards(currentTorque, targetTorque, accelerationRate * Time.fixedDeltaTime * motorForce);
        else
            currentTorque = Mathf.MoveTowards(currentTorque, 0, decelerationRate * Time.fixedDeltaTime * motorForce);

        // Clamp based on max speed
        float speed = rb.velocity.magnitude * 3.6f; // km/h
        if (speed >= maxSpeed && verticalInput > 0)
            currentTorque = 0f;

        // Braking 🚨 or Reverse 🔄
        if (isBraking)
        {
            if (speed > 1f)
            {
                // if moving → apply brakes
                currentBrakeForce = brakeForce;
            }
            else
            {
                // if stopped → reverse
                currentBrakeForce = 0f;
                currentTorque = -motorForce * 1; // reverse torque (slightly weaker than forward)
            }
        }

        // Apply torque
        ApplyMotorAndBrakes();

        // Horizontal input (steering)
        if (isTurningLeft) horizontalInput = -1f;
        else if (isTurningRight) horizontalInput = 1f;
        else horizontalInput = 0f;
    }


    private void ApplyMotorAndBrakes()
    {
        // Apply motor torque
        frontLeftWheelCollider.motorTorque = currentTorque;
        frontRightWheelCollider.motorTorque = currentTorque;
        rearLeftWheelCollider.motorTorque = currentTorque;
        rearRightWheelCollider.motorTorque = currentTorque;

        // Apply brake torque
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleMotor() { }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void HandleDrift()
    {
        if (isDrifting)
        {
            AdjustFriction(driftStiffness);
            rb.AddTorque(transform.up * horizontalInput * driftTorque);
        }
        else
        {
            AdjustFriction(normalStiffness);
        }
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void AdjustFriction(float stiffness)
    {
        WheelFrictionCurve forward = frontLeftWheelCollider.forwardFriction;
        WheelFrictionCurve sideways = frontLeftWheelCollider.sidewaysFriction;

        forward.stiffness = stiffness;
        sideways.stiffness = stiffness;

        frontLeftWheelCollider.forwardFriction = forward;
        frontLeftWheelCollider.sidewaysFriction = sideways;

        frontRightWheelCollider.forwardFriction = forward;
        frontRightWheelCollider.sidewaysFriction = sideways;

        rearLeftWheelCollider.forwardFriction = forward;
        rearLeftWheelCollider.sidewaysFriction = sideways;

        rearRightWheelCollider.forwardFriction = forward;
        rearRightWheelCollider.sidewaysFriction = sideways;
    }

    //if can position.y < -5 then reset position
    public void ResetCar()
    {
        rb.velocity = Vector3.zero;       // stop movement
        rb.angularVelocity = Vector3.zero; // stop spinning
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Point"))
        {
            score += Random.Range(5, 10);
            scoreText.text = score.ToString();
            PlayerPrefs.SetInt("Score", score);
            //Debug.Log("Points: " + points);
            Destroy(other.gameObject);
        }
    }



    // 🚀 Mobile Button Functions 🚀
    public void AcceleratePressed() => isAccelerating = true;
    public void AccelerateReleased() => isAccelerating = false;

    public void BrakePressed() => isBraking = true;
    public void BrakeReleased() => isBraking = false;

    public void TurnLeftPressed() => isTurningLeft = true;
    public void TurnLeftReleased() => isTurningLeft = false;

    public void TurnRightPressed() => isTurningRight = true;
    public void TurnRightReleased() => isTurningRight = false;

    public void DriftPressed() => isDrifting = true;
    public void DriftReleased() => isDrifting = false;
}
