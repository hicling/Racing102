using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
    [SerializeField] private CinemachineVirtualCamera virtualCameraFinnish = null;
    [SerializeField] private CarLapController carLapController;

    public override void OnStartAuthority()
    {
        virtualCamera.gameObject.SetActive(true);
        enabled = true;
    }

    private void Update()
    {
        if (!carLapController.Finnished) { return; }
        virtualCamera.gameObject.SetActive(false);
        virtualCameraFinnish.gameObject.SetActive(true);
    }
}
