using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkGrabbable : XRGrabInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        args.interactorObject.transform.parent.transform.parent.GetComponent<NetworkPlayer>().OnSelectGrabbable(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        //args.interactorObject.transform.parent.transform.parent.GetComponent<NetworkPlayer>().OnReleaseGrabbable(args);
    }

    private void Update()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}