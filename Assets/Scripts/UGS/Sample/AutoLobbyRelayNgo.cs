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
        public Button startHost;
        public Button quickJoin;

        public int maxPlayers = 2;
        public UnityEvent handleHostConnected = new(), handleClientConnected = new();
        private LobbyInstance _lobby;
        private RelayInstance _relay;

        private void Start()
        {
            startHost.onClick.AddListener(HandleStartHost);
            quickJoin.onClick.AddListener(HandleQuickJoin);
        }

        private async void HandleStartHost()
        {
            _relay = new RelayInstance();
            await _relay.CreateAllocation(maxPlayers);
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
            handleHostConnected.Invoke();
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
            await _lobby.QuickJoin();
            var code = _lobby.GetLobby().Data["code"].Value;
            _relay = new RelayInstance();
            await _relay.JoinAllocation(code);
            
            NetworkManager.Singleton.StartClient();
            handleClientConnected.Invoke();
        }
        
    }
}