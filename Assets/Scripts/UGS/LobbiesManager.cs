using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;

public class LobbiesManager : MonoBehaviour
{
    private void Awake()
    {
        UnityServices.InitializeAsync();
    }

    private void Start()
    {
        DeleteAllLobbies();
    }

    private async void DeleteAllLobbies()
    {
        try
        {
            var lobbyIds = await LobbyService.Instance.GetJoinedLobbiesAsync();
            foreach (var lobbyId in lobbyIds)
            {
                Debug.LogError("DELETED LOBBY: " + lobbyId);
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
