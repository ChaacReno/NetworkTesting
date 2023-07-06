using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionManager : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var playerId = eventData.pointerPress.GetComponent<NetworkPlayer>().OwnerClientId;
        NetworkManager.Singleton.DisconnectClient(playerId);
    }
}
