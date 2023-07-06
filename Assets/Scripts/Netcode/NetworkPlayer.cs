using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject[] eyes;
    [SerializeField] private Vector2 placementArea = new Vector2(-10, 10);
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        DisableClientInput();
    }

    private void DisableClientInput()
    {
        if (IsOwner)
        {
            HideEyes();
        }
        else
        {
            var clientMoveProvider = GetComponent<NetworkMoveProvider>();
            var clientControllers = GetComponentsInChildren<ActionBasedController>();
            var clientTurnProvider = GetComponent<ActionBasedSnapTurnProvider>();
            var clientHead = GetComponentInChildren<TrackedPoseDriver>();
            var clientCamera = GetComponentInChildren<Camera>();

            clientCamera.enabled = false;
            clientMoveProvider.enableInputAction = false;
            clientTurnProvider.enableTurnLeftRight = false;
            clientTurnProvider.enableTurnAround = false;
            clientHead.enabled = false;

            foreach (var controller in clientControllers)
            {
                controller.enableInputActions = false;
                controller.enableInputTracking = false;
            }
        }
    }

    private void HideEyes()
    {
        foreach (var eye in eyes)
        {
            eye.SetActive(false);
        }
    }

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position =
                new Vector3(Random.Range(placementArea.x, placementArea.y), transform.position.y,
                    Random.Range(placementArea.x, placementArea.y));
        }
    }

    public void OnSelectGrabbable(SelectEnterEventArgs eventArgs)
    {
        if (eventArgs.interactableObject.transform.CompareTag("Grabbable"))
        {
            if (!IsClient || !IsOwner) return;
            NetworkObject networkObjectSelected =
                eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
            if (networkObjectSelected != null)
            {
                //Request ownership from the server
                RequestGrabbableOwnershipServerRpc(OwnerClientId, networkObjectSelected);
            }
        }
    }

    

    public void OnReleaseGrabbable(SelectExitEventArgs eventArgs)
    {
        if (eventArgs.interactableObject.transform.CompareTag("Grabbable"))
        {
            if (!IsClient || !IsOwner) return;
            NetworkObject networkObjectSelected =
                eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
            if (networkObjectSelected != null)
            {
                Debug.LogError("Remove ownership");
                //Request ownership from the server
                RemoveOwnershipServerRpc(eventArgs.interactableObject.transform.GetComponent<NetworkObject>());
            }
            else
            {
                Debug.LogError("Object null");
            }
        }
    }

    [ServerRpc]
    public void RequestGrabbableOwnershipServerRpc(ulong newOwnerClientId,
        NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(newOwnerClientId);
        }
        else
        {
            Debug.LogWarning($"Unable to change ownership for clientId {newOwnerClientId}");
        }
    }

    [ServerRpc]
    public void RemoveOwnershipServerRpc(NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.RemoveOwnership();
        }
    }

    public override void OnNetworkDespawn()
    {
        SceneManager.LoadSceneAsync("Scenes/XRMultiplayerSetup");
    }
}