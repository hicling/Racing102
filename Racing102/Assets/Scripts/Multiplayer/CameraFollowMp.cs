using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowMp : MonoBehaviour
{
    public Transform Target;

    public Vector3 offset;
    public Vector3 eulerRotation;
    public float damper;

    void Start()
    {
        transform.eulerAngles = eulerRotation;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Target == null)
            return;

        transform.position = Vector3.Lerp(transform.position, Target.position + offset, damper * Time.deltaTime);
    }

    public void setTarget(Transform target)
    {
        Target = target;
    }
}
