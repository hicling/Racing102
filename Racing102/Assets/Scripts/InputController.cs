using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public string inputSteerAxis = "Horizontal";
    public string inputThrottleAxis = "Vertical";
    public string inputBrakeAxis = "Submit";
    public string inputReset = "Reset";

    public float ThrottleInput { get; private set; }
    public float SteerInput { get; private set; }
    public float BrakeInput { get; private set; }
    public float ResetInput { get; private set; }
    void Start()
    {
        
    }

    void Update()
    {
        SteerInput = Input.GetAxis(inputSteerAxis);
        ThrottleInput = Input.GetAxis(inputThrottleAxis);
        BrakeInput = Input.GetAxis(inputBrakeAxis);
        ResetInput = Input.GetAxis(inputReset);
    }
}
