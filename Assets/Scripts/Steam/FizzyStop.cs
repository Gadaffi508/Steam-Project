using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Steamworks
{
    public class FizzyStop : NetworkBehaviour
    {
        public int sceneID;

        public CSteamID lobbyID;

        #region Singleton

        private FizzyNetworkManager _manager;

        private FizzyNetworkManager Manager
        {
            get
            {
                if (_manager != null) return _manager;
                return _manager = NetworkManager.singleton as FizzyNetworkManager;
            }
        }

        #endregion

        void Start()
        {
            lobbyID = (CSteamID)FizzySteamLobby.Instance.CurrentLobbyID;
        }

        public void ApplicationQuit()
        {
            Application.Quit();
        }

        public void LeaveGame()
        {
            Manager.offlineScene = "";
            
            // Unregister all lobby callbacks
            /*Callback<LobbyCreated_t>.Unregister();
            Callback<GameLobbyJoinRequested_t>.Unregister();
            Callback<LobbyEnter_t>.Unregister();
            Callback<LobbyMatchList_t>.Unregister();
            Callback<LobbyDataUpdate_t>.Unregister();
            Callback<LobbyChatMsg_t>.Unregister();*/
            
            SceneManager.LoadScene(sceneID);
            
            FizzySteamLobby.Instance.LeaveGame(lobbyID);

            Manager.networkAddress = "localhost";

            if (isLocalPlayer)
            {
                if (isServer)
                {
                    Manager.StopHost();
                }
                else
                {
                    Manager.StopClient();
                }
            }else
            {
                Manager.StopClient();
            }
            
            Manager.StopHost();
        }
    }
}