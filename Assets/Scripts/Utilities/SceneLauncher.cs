using System;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLauncher : MonoBehaviour
{
    public static SceneLauncher Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }

    public void StartHost()
    {
        // NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(GetLocalIPAddress(),7777);
        SceneTransitionHandler.Instance.InitializeAsHost = true;
        // SceneManager.LoadScene("Scenes/XRMultiplayerBasketball",LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.LoadScene("Scenes/XRMultiplayerBasketball", LoadSceneMode.Single);
    }

    public void StartClient()
    {
        // NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("23.233.245.159",7777);
        NetworkManager.Singleton.StartClient();
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
