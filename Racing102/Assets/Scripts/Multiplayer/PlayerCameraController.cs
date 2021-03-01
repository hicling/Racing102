using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    //[SerializeField] private Vector3 offset;
    //[SerializeField] private Vector3 eulerRotation;
    //[SerializeField] private float damper;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
    //[SerializeField] private Transform playerTransform = null;

    public override void OnStartAuthority()
    {
        //transform.eulerAngles = eulerRotation;

        virtualCamera.gameObject.SetActive(true);

        enabled = true;
    }

    //void FixedUpdate()
    //{
    //    if (playerTransform == null)
    //        return;

    //    transform.position = Vector3.Lerp(transform.position, playerTransform.position + offset, damper * Time.deltaTime);
    //}
}
