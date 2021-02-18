using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform centerOfMass;

    public float motorTorque = 1500f;
    public float maxSteer = 20f;
    public float brakeTorque = 100f;

    public float Steer { get; set; }
    public float Throttle { get; set; }
    public float Brake { get; set; }
    public float Reset { get; set; }
    private Rigidbody rb;
    private Wheel[] wheels;
    private Quaternion target;
    private Vector3 movePosition;

    void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }

    void Update()
    {
        foreach (var wheel in wheels)
        {
            wheel.SteerAngle = Steer * maxSteer;
            wheel.Torque = Throttle * motorTorque;
            wheel.BrakeTorque = Brake * brakeTorque;
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

}
