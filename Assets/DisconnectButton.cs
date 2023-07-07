using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class DisconnectButton : NetworkBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private GameObject player;
    public void OnPointerClick(PointerEventData eventData)
    {
        DisconnectServerRpc();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        var trackedDeviceEventData = eventData as TrackedDeviceEventData;

        if (trackedDeviceEventData != null)
        {
            //var controller = trackedDeviceEventData.rayInteractor.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRController>();
            /*if (controller)
            {
                GameObject player = controller.transform.root.gameObject;
                Debug.Log(player.name + " is pointing at " + gameObject.name);
            }*/
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisconnectServerRpc()
    {
        for (var i = NetworkManager.Singleton.ConnectedClientsIds.Count - 1; i >= 0; i--)
        {
            var id = NetworkManager.Singleton.ConnectedClientsIds[i];
            if (id != NetworkManager.Singleton.LocalClientId)
            {
                NetworkManager.Singleton.DisconnectClient(id);
            }
        }
    }
}
