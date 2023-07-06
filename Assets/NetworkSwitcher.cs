using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkSwitcher : MonoBehaviour
{
    public void InitLocal()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1",7777);
    }
    public void InitNetwork()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("23.233.245.159",7777);
    }
}
