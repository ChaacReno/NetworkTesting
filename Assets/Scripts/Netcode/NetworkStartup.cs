using Unity.Netcode;
using UnityEngine;

public class NetworkStartup : MonoBehaviour
{
    private void Start()
    {
        if (SceneTransitionHandler.Instance.InitializeAsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
    }
}
