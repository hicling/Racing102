using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool steer;
    public bool invertSteer;
    public bool power;
    public bool brake;

    public float SteerAngle { get; set; }
    public float Torque { get; set; }
    public float BrakeTorque { get; set; }

    private WheelCollider wheelCollider;
    private Transform wheelTransform;
    WheelFrictionCurve standardFFriction;
    WheelFrictionCurve standardSFriction;

    void Start()
    {
        wheelCollider = GetComponentInChildren<WheelCollider>();
        wheelTransform = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
        standardFFriction = wheelCollider.forwardFriction;
        standardSFriction = wheelCollider.sidewaysFriction;
    }


    void Update()
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    void FixedUpdate()
    {
        if (steer)
        {
            wheelCollider.steerAngle = SteerAngle * (invertSteer ? -1 : 1);
        }

        if (power)
        {
            wheelCollider.motorTorque = Torque;
        }
        if (brake)
        {
            wheelCollider.brakeTorque = BrakeTorque;
        }
        WheelHit hit;
        if (wheelCollider.GetGroundHit(out hit))
        {
            float friction = hit.collider.material.staticFriction;
            WheelFrictionCurve test1 = standardFFriction;
            WheelFrictionCurve test2 = standardSFriction;
            if (friction == 0.6f)
            {
                test1.stiffness = 0.2f;
                test2.stiffness = 0.2f;

                wheelCollider.forwardFriction = test1;
                wheelCollider.sidewaysFriction = test2;
            }
            else
            {
                wheelCollider.forwardFriction = standardFFriction;
                wheelCollider.sidewaysFriction = standardSFriction;
            }
            Debug.Log(wheelCollider.forwardFriction.stiffness + "  " + hit.collider.material.staticFriction);
        }
    }
}
