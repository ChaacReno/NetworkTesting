#if UNITY_EDITOR
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using ParrelSync;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class AutoStartSession : MonoBehaviour
{
    
    public void Start()
    {
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (ClonesManager.IsClone())
        {
            unityTransport.SetConnectionData("23.233.245.159",7777);
            StartCoroutine(WaitForHost());
        }
        else
        {
            unityTransport.SetConnectionData(GetLocalIPAddress(),7777);
            SceneLauncher.Instance.StartHost();
        }

    }
    
    public IEnumerator WaitForHost()
    {
        yield return new WaitForSeconds(1);
        SceneLauncher.Instance.StartClient();
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
#endif