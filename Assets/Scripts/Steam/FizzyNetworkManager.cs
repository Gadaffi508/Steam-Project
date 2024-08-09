using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Steamworks
{
    public class FizzyNetworkManager : NetworkManager
    {
        public FizzyPlayer gamePlayerPrefabs;
        public FizzyPlayer gamePlayerLobby;
        
        public List<FizzyPlayer> GamePlayer = new List<FizzyPlayer>();

        public GameObject playerSpawnSystem = null;

        public NetworkConnectionToClient _conn;
        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnection> OnServerReadied; 
        
        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("Assets/Prefabs/Resources/").ToList();

        public override void OnStartClient()
        {
            var spawnablePrefab = Resources.LoadAll<GameObject>("Assets/Prefabs/Resources/");

            foreach (var prefab in spawnablePrefab)
            {
                ClientScene.RegisterPrefab(prefab);
            }
        }
        
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            
            Debug.Log("Client Connect");
            
            OnClientConnected?.Invoke();
        }
        
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            
            Debug.Log("Client Disconnect");
            
            OnClientDisconnected?.Invoke();
        }
        
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.Log("OnServerConnect -----------------Connect");
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            FizzyPlayer gamePlayerInstance = Instantiate(gamePlayerPrefabs);
        
            gamePlayerInstance.connectionID = conn.connectionId;
            gamePlayerInstance.playerIdNumber = GamePlayer.Count + 1;

            CSteamID _currentLobbID = (CSteamID)FizzySteamLobby.Instance.CurrentLobbyID;
            gamePlayerInstance.playerSteamId =
                (ulong)SteamMatchmaking.GetLobbyMemberByIndex(_currentLobbID, GamePlayer.Count);
            
            NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
        }
        
        public override void ServerChangeScene(string newSceneName)
        {
            if (SceneManager.GetActiveScene().name == "LobbyScene" && newSceneName.StartsWith("Game"))
            {
                /*for (int i = GamePlayer.Count - 1; i >= 0; i--)
                {
                    var conn = GamePlayer[i].connectionToClient;
                    var gamePlayerInstance = Instantiate(gamePlayerLobby);
                    gamePlayerInstance.SetDisplayName(GamePlayer[i].name);
                    
                    NetworkServer.Destroy(conn.identity.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
                }
                Debug.Log("Server changed");*/
            }
            
            base.ServerChangeScene(newSceneName);
        }
        
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            
            OnServerReadied?.Invoke(conn);
        }
        
        public override void OnServerChangeScene(string newSceneName)
        {
            if (newSceneName.StartsWith("Game"))
            {
                Debug.Log("OnServerChangeScene");
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);
            }
        }

        public void StartGame(string sceneName)
        {
            ServerChangeScene(sceneName);
        }
    }

}