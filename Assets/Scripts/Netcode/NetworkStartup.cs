using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;

public class NetworkStartup : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;
    private async void Start()
    {
        NetworkObject obj = Instantiate(playerRef).GetComponent<NetworkObject>();
        obj.Spawn();
        if (SceneTransitionHandler.Instance.InitializeAsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        try
        {
            var lobbyIds = await LobbyService.Instance.GetJoinedLobbiesAsync();
            foreach (var lobbyId in lobbyIds)
            {
                Debug.LogError(lobbyId);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
