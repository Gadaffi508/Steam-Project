using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Steamworks
{
    public class FizzyStop : MonoBehaviour
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
            if(lobbyID != (CSteamID)0)
                FizzySteamLobby.Instance.LeaveGame(lobbyID);
            else
                Debug.Log("Lobby ID : " + lobbyID);
        
            Manager.StopHost();

            Manager.networkAddress = "HostAddress";
        
            Manager.StopClient();

            SceneManager.LoadScene(sceneID);
        }
    }
}
