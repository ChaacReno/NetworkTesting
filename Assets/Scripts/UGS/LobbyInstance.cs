using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace UGS
{
    [Serializable]
    public class LobbySettings
    {
        public string LobbyName = "Lobby";
        public int MaxPlayers = 1;

        public LobbySettings()
        {
        }

        public LobbySettings(string name, int maxPlayers)
        {
            LobbyName = name;
            this.MaxPlayers = maxPlayers;
        }
    }

    public class LobbyInstance
    {
        private Lobby _lobbyInfo;

        private LobbyEventCallbacks _callbacks;

        public async Task Host(
            LobbySettings settings,
            CreateLobbyOptions options = null,
            LobbyEventCallbacks callbacks = null)
        {
            if (settings == null)
            {
                Debug.LogWarning("Unable to host new lobby without LobbySettings");
                return;
            }

            _lobbyInfo = await LobbyService.Instance.CreateLobbyAsync(settings.LobbyName, settings.MaxPlayers, options);

            AssignCallbacks(callbacks);
        }

        public Lobby GetLobby()
        {
            return _lobbyInfo;
        }

        public async Task JoinById(JoinLobbyByIdOptions options = null, LobbyEventCallbacks callbacks = null)
        {
            if (_lobbyInfo == null)
            {
                Debug.LogWarning("Unassigned LobbyInfo reference for this LobbyInstance");
                return;
            }

            _lobbyInfo = await LobbyService.Instance.JoinLobbyByIdAsync(_lobbyInfo.Id, options);

            //AssignCallbacks(callbacks);
        }

        public async Task JoinByCode(JoinLobbyByCodeOptions options = null, LobbyEventCallbacks callbacks = null)
        {
            if (_lobbyInfo == null)
            {
                Debug.LogWarning("Unassigned LobbyInfo reference for this LobbyInstance");
                return;
            }

            _lobbyInfo = await LobbyService.Instance.JoinLobbyByCodeAsync(_lobbyInfo.Id, options);

            AssignCallbacks(callbacks);
        }

        public async Task QuickJoin(QuickJoinLobbyOptions options = null, LobbyEventCallbacks callbacks = null)
        {
            _lobbyInfo = await LobbyService.Instance.QuickJoinLobbyAsync(options);

            AssignCallbacks(callbacks);
        }

        public async Task UpdateLobby(UpdateLobbyOptions options)
        {
            _lobbyInfo = await LobbyService.Instance.UpdateLobbyAsync(_lobbyInfo.Id, options);
        }

        public async Task UpdatePlayer(string playerId, UpdatePlayerOptions options)
        {
            _lobbyInfo = await LobbyService.Instance.UpdatePlayerAsync(_lobbyInfo.Id, playerId, options);
        }

        public async Task HeartbeatPing()
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(_lobbyInfo.Id);
        }

        private async void AssignCallbacks(LobbyEventCallbacks callbacks)
        {
            if (callbacks == null) return;
            _callbacks = callbacks;
            await Lobbies.Instance.SubscribeToLobbyEventsAsync(_lobbyInfo.Id, _callbacks);
        }

        public static async Task<List<Lobby>> QueryLobbies(QueryLobbiesOptions options = null)
        {
            var result = await Lobbies.Instance.QueryLobbiesAsync(options);
            return result.Results;
        }
    }
}