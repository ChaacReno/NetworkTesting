using Unity.Netcode;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ConnectionManager : NetworkBehaviour, IPointerClickHandler
{
    
    public void OnPointerClick(PointerEventData eventData)
    {
        DisconnectServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DisconnectServerRpc()
    {
        var playerId = NetworkManager.Singleton.LocalClientId;
        NetworkManager.Singleton.DisconnectClient(playerId);
        SceneManager.LoadScene("Scenes/XRMultiplayerSetup");
    }
}
