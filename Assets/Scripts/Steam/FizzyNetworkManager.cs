using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Steamworks
{
    public class FizzyNetworkManager : NetworkManager
    {
        public FizzyPlayer gamePlayerPrefabs;
        public List<FizzyPlayer> GamePlayer = new List<FizzyPlayer>();
        
        [Scene] public string gameScene = String.Empty;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if(SceneManager.GetActiveScene().name != gameScene) return;
            
            FizzyPlayer gamePlayerInstance = Instantiate(gamePlayerPrefabs);
        
            gamePlayerInstance.connectionID = conn.connectionId;
            gamePlayerInstance.playerIdNumber = GamePlayer.Count + 1;

            CSteamID _currentLobbID = (CSteamID)FizzySteamLobby.Instance.CurrentLobbyID;
            gamePlayerInstance.playerSteamId =
                (ulong)SteamMatchmaking.GetLobbyMemberByIndex(_currentLobbID, GamePlayer.Count);
            
            NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
        }

        public void StartGame(string sceneName)
        {
            ServerChangeScene(sceneName);
        }
    }

}