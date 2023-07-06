using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionManager : MonoBehaviour, IPointerClickHandler
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
    }
}
