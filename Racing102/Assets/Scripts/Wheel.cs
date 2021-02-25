using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Wheel Parameter")]
    [SerializeField] bool steer;
    [SerializeField] bool invertSteer;
    [SerializeField] bool power;
    [SerializeField] bool brake;
    [Header("Surface Parameters Grass")]
    [SerializeField] float grassFriction = 0.6f;
    [Range(0.01f, 0.9f)]
    [SerializeField] float grassSStiffness = 0.3f;
    [Range(0.01f, 1f)]
    [SerializeField] float grassDrag = 0.55f;
    [Header("Surface Parameters Gravel")]
    [SerializeField] float gravelFriction = 0.7f;
    [Range(0.01f, 0.9f)]
    [SerializeField] float gravelSStiffness = 0.6f;
    [Range(0.01f, 1f)]
    [SerializeField] float gravelDrag = 0.3f;
    [Header("Surface Parameters Sand")]
    [SerializeField] float sandFriction = 0.5f;
    [Range(0.01f, 0.9f)]
    [SerializeField] float sandSStiffness = 0.25f;
    [Range(0.01f, 1f)]
    [SerializeField] float sandDrag = 0.6f;

    public float SteerAngle { get; set; }
    public float Torque { get; set; }
    public float BrakeTorque { get; set; }

    private WheelCollider wheelCollider;
    private Transform wheelTransform;

    WheelFrictionCurve standardSFriction;
    float standardDrag;
    Rigidbody rb;
    

    void Start()
    {
        wheelCollider = GetComponentInChildren<WheelCollider>();
        wheelTransform = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
        standardSFriction = wheelCollider.sidewaysFriction;
        rb = GetComponentInParent<Rigidbody>();
        standardDrag = rb.drag;
        
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
            WheelFrictionCurve savedSFriction = standardSFriction;
            if (friction == grassFriction)
            {
                savedSFriction.stiffness = grassSStiffness;

                wheelCollider.sidewaysFriction = savedSFriction;
                rb.drag = grassDrag;
            }
            else if (friction == gravelFriction)
            {
                savedSFriction.stiffness = gravelSStiffness;

                wheelCollider.sidewaysFriction = savedSFriction;
                rb.drag = gravelDrag;
            }
            else if (friction == sandFriction)
            {
                savedSFriction.stiffness = sandSStiffness;

                wheelCollider.sidewaysFriction = savedSFriction;
                rb.drag = sandDrag;
            }
            else
            {
            
                wheelCollider.sidewaysFriction = standardSFriction;
                rb.drag = standardDrag;
            }
        }      
    }
}
