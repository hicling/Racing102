using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LicoriceMarks : MonoBehaviour
{
    [SerializeField] float SKID_FX_SPEED = 0.5f; 
	[SerializeField] float MAX_SKID_INTENSITY = 20.0f; 
	[SerializeField] float WHEEL_SLIP_MULTIPLIER = 10.0f; 
    [SerializeField] double minSpeed = 40;

	private Rigidbody rb;
	private Skidmarks skidmarksController;
	private WheelCollider wheelCollider;
	private WheelHit wheelHitInfo;
    private new ParticleSystem particleSystem;

    int lastSkid = -1;
	float lastFixedUpdateTime;
    void Awake()
    {
        rb = GetComponentInParent<Rigidbody>(); 
        skidmarksController = FindObjectOfType<Skidmarks>();
        wheelCollider = GetComponent<WheelCollider>();
        particleSystem = GetComponent<ParticleSystem>();
        lastFixedUpdateTime = Time.time;
    }

    protected void FixedUpdate() {
		lastFixedUpdateTime = Time.time;
	}

    void LateUpdate()
    {   
        if (wheelCollider.GetGroundHit(out wheelHitInfo))
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
			float skidTotal = Mathf.Abs(localVelocity.x);

            float wheelAngularVelocity = wheelCollider.radius * ((2 * Mathf.PI * wheelCollider.rpm) / 60);
			float carForwardVel = Vector3.Dot(rb.velocity, transform.forward);
			float wheelSpin = Mathf.Abs(carForwardVel - wheelAngularVelocity) * WHEEL_SLIP_MULTIPLIER;
            
            double speed = rb.velocity.magnitude * 3.6;
            float motorTorque = wheelCollider.motorTorque;

            if (speed < minSpeed & motorTorque == 0)
            {
                wheelSpin = 0;
            }

            wheelSpin = Mathf.Max(0, wheelSpin * (10 - Mathf.Abs(carForwardVel)));

			skidTotal += wheelSpin;

			if (skidTotal > SKID_FX_SPEED) 
            {
				float intensity = Mathf.Clamp01(skidTotal / MAX_SKID_INTENSITY);
				Vector3 skidPoint = wheelHitInfo.point + (rb.velocity * (Time.time - lastFixedUpdateTime));
				lastSkid = skidmarksController.AddSkidMark(skidPoint, wheelHitInfo.normal, intensity, lastSkid);
                if(particleSystem != null && !particleSystem.isPlaying)
                {
                    particleSystem.Play();
                }
            }
			else 
            {
				lastSkid = -1;
                if (particleSystem != null && particleSystem.isPlaying)
                {
                    particleSystem.Stop();
                }
			}
        }
		else 
        {
			lastSkid = -1;
            if (particleSystem != null && particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }
		}
           
    }
}
