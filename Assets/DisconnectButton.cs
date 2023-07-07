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
        Disconnect();
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
    
    
    public void Disconnect()
    {
        SceneManager.LoadSceneAsync("XRMultiplayerSetup");
        NetworkManager.Singleton.Shutdown();
    }
}
