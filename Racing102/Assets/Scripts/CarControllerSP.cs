using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControllerSP : MonoBehaviour
{
    
    [Header("Car Parameters")]
    [SerializeField] AnimationCurve motorTorque = new AnimationCurve(new Keyframe(0, 200), new Keyframe(50, 300), new Keyframe(200, 0));
    [Range(2, 16)]
    [SerializeField] float diffGearing = 4.0f;
    public float DiffGearing { get { return diffGearing; } set { diffGearing = value; } }
    [SerializeField] AnimationCurve steerInputCurve = AnimationCurve.Linear(-1.0f, -1.0f, 1.0f, 1.0f);
    [Range(0f, 50f)]
    [SerializeField] float maxSteer = 20f;
    public float MaxSteer { get { return maxSteer; } set { maxSteer = Mathf.Clamp(value, 0.0f, 50.0f); } }
    [Range(0.001f, 1.0f)]
    [SerializeField] float steerSpeed = 0.2f;
    public float SteerSpeed { get { return steerSpeed; } set { steerSpeed = Mathf.Clamp(value, 0.001f, 1.0f); } }
    [SerializeField] float brakeForce = 100f;
    public float BrakeForce { get { return brakeForce; } set { brakeForce = value; } }
    [SerializeField] bool handbrake;
    public bool Handbrake { get { return handbrake; } set { handbrake = value; } }

    [Range(0.5f, 10f)]
    [SerializeField] float downforce = 1.0f;
    [SerializeField] Transform centerOfMass;

    [Header("CurrentSpeed")]
    [SerializeField] float speed = 0.0f;
    public float Speed { get { return speed; } }

    


    public float Steer { get; set; }
    public float Throttle { get; set; }
    public float Brake { get; set; }
    public float Reset { get; set; }
    private Rigidbody rb;
    private Wheel[] wheels;
    private Quaternion target;
    private Vector3 movePosition;
    private float steering;

    void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
        foreach(var wheel in wheels)
        {
            wheel.Torque = 0.0001f;
        }
    }

    void Update()
    {
        foreach (var wheel in wheels)
        {
            steering = steerInputCurve.Evaluate(Steer) * maxSteer;
            wheel.SteerAngle = Mathf.Lerp(wheel.SteerAngle, steering, steerSpeed);
            wheel.BrakeTorque = 0;
            if (handbrake)
            {
                wheel.Torque = 0.0001f;
                wheel.BrakeTorque = brakeForce;
            }
            else if (Mathf.Abs(speed) < 4 || Mathf.Sign(speed) == Mathf.Sign(Throttle))
            {
                wheel.Torque = Throttle * motorTorque.Evaluate(speed) * diffGearing;
            }
            else
            {
                wheel.Torque = 0;
                wheel.BrakeTorque = Mathf.Abs(Throttle) * brakeForce;
            }
            
            
        }
        if(Reset == 1)
        {
            movePosition = rb.position;
            movePosition.y = 5;
            target = rb.rotation;
            target[2] = 0;
            target[0] = 0;
            target = target.normalized;
            rb.MovePosition(movePosition);
            rb.MoveRotation(target);
        }
    
    }

    private void FixedUpdate()
    {
        speed = transform.InverseTransformDirection(rb.velocity).z * 3.6f;
        rb.AddForce(-transform.up * speed * downforce);
    }

}
