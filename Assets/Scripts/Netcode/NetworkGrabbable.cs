using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkGrabbable : XRGrabInteractable
{
    private Dictionary<IXRSelectInteractor, NetworkPlayer> networkPlayerCache = new Dictionary<IXRSelectInteractor, NetworkPlayer>();
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        NetworkPlayer networkPlayer;

        if (!networkPlayerCache.TryGetValue(args.interactorObject, out networkPlayer))
        {
            networkPlayer = args.interactorObject.transform.GetComponentInParent<NetworkPlayer>();
            networkPlayerCache[args.interactorObject] = networkPlayer;
        }

        networkPlayer.OnSelectGrabbable(args);
    }

    private void Update()
    {
        rb.isKinematic = false;
    }
}