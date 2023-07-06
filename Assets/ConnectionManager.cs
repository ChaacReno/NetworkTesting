using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionManager : MonoBehaviour
{
    public void OnPointerClick(PointerEventData eventData)
    {
        NetworkManager.Singleton.DisconnectClient();
    }
}
