using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkMoveProvider : ActionBasedContinuousMoveProvider
{
    public bool enableInputAction;
    
    [ServerRpc]
    protected override Vector2 ReadInput()
    {
        if (!enableInputAction) return Vector2.zero;
        return base.ReadInput();
    }
}
