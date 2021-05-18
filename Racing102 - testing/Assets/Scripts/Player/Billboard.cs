using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Billboard : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform playerCamera;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + playerCamera.rotation * Vector3.forward, playerCamera.rotation * Vector3.up);
    }
}
