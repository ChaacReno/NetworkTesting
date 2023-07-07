using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UGS.Sample
{
    public class AutoLobbyRelayNgo : MonoBehaviour
    {
        public Button StartHost;
        public Button QuickJoin;

        public int MaxPlayers = 2;
        public UnityEvent HandleHostConnected = new(), HandleClientConnected = new();
        private LobbyInstance _lobby;
        private RelayInstance _relay;

        private void Start()
        {
            StartHost.onClick.AddListener(HandleStartHost);
            QuickJoin.onClick.AddListener(HandleQuickJoin);
        }

        private async void HandleStartHost()
        {
            _relay = new RelayInstance();
            await _relay.CreateAllocation(MaxPlayers);
            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        "code", new DataObject(
                            visibility: DataObject.VisibilityOptions.Public,
                            value: _relay.joinCode)
                    }
                },
                IsPrivate = false
            };
            var settings = new LobbySettings("Test Lobby", 2);
            _lobby = new LobbyInstance();
            await _lobby.Host(settings, options);
            StartCoroutine(HeartbeatLobby());
            
            NetworkManager.Singleton.StartHost();
            HandleHostConnected.Invoke();
        }

        private IEnumerator HeartbeatLobby()
        {
            while (isActiveAndEnabled)
            {
                yield return new WaitForSeconds(25);
                yield return _lobby.HeartbeatPing();
            }
        }
        
        private async void HandleQuickJoin()
        {
            _lobby = new LobbyInstance();

            Debug.LogError(LobbyInstance.QueryLobbies());
            await _lobby.QuickJoin();
            var code = _lobby.GetLobby().Data["code"].Value;
            _relay = new RelayInstance();
            await _relay.JoinAllocation(code);
            
            NetworkManager.Singleton.StartClient();
            HandleClientConnected.Invoke();
        }
        
    }
}