using Unity.Netcode;
using UnityEngine;

public class NetworkStartup : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;
    private void Start()
    {
        NetworkObject obj = Instantiate(playerRef).GetComponent<NetworkObject>();
        obj.Spawn();
        if (SceneTransitionHandler.Instance.InitializeAsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
    }
}
