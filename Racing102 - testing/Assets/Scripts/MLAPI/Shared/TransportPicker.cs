using System;
using MLAPI.Transports;
using MLAPI.Transports.LiteNetLib;
using MLAPI.Transports.PhotonRealtime;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.Assertions;

public class TransportPicker : MonoBehaviour
{
    [SerializeField]
    NetworkTransport m_IpHostTransport;

    [SerializeField]
    NetworkTransport m_RelayTransport;

    /// <summary>
    /// The transport used when hosting the game on an IP address.
    /// </summary>
    public NetworkTransport IpHostTransport => m_IpHostTransport;

    /// <summary>
    /// The transport used when hosting the game over a relay server.
    /// </summary>
    public NetworkTransport RelayTransport => m_RelayTransport;

    void OnValidate()
    {
        Assert.IsTrue(m_IpHostTransport == null || (m_IpHostTransport as UNetTransport || m_IpHostTransport as LiteNetLibTransport),
            "IpHost transport must be either Unet or LiteNetLib transport.");

        Assert.IsTrue(m_RelayTransport == null || (m_RelayTransport as PhotonRealtimeTransport), "" +
            "Relay transport must be PhotonRealtimeTransport");
    }
}
