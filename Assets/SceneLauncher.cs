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
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(GetLocalIPAddress(),7777);
        SceneTransitionHandler.Instance.InitializeAsHost = true;
        LoadScene("Scenes/XRMultiplayerBasketball");
    }

    public void StartClient()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("23.233.245.159",7777);
        SceneTransitionHandler.Instance.InitializeAsHost = false;
        LoadScene("Scenes/XRMultiplayerBasketball");
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
