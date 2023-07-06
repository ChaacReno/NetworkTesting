using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OwnershipManager : MonoBehaviour
{
    public static OwnershipManager Instance;

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

    [ServerRpc]
    public void RequestOwnershipServerRpc(NetworkObjectReference obj,ulong clientId )
    {
        if (obj.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(clientId);
        }
        Debug.LogError("Request Change ownership");
    }

    [ServerRpc]
    public void RemoveOwnershipServerRpc(SelectExitEventArgs args)
    {
        args.interactableObject.transform.GetComponent<NetworkObject>().RemoveOwnership();
    }
}
