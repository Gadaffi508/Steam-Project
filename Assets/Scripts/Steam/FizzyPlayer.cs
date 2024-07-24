using Mirror;
using UnityEngine;

namespace Steamworks
{
    public class FizzyPlayer : NetworkBehaviour
    {
        [Header("Player Connection Values")]
        [SyncVar] public int connectionID;
        [SyncVar] public int playerIdNumber;
        
        [Header("Player Object Values")]
        public ulong playerSteamId;
        
        [SyncVar(hook = nameof(OnPlayerNameChanged))]
        public string playerName;
        
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
        
        public override void OnStartAuthority()
        {
            DontDestroyOnLoad(this);
            CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
            gameObject.name = "LocalGamePlayer";
        
            FizzyLobbyController.Instance.FindLocalPlayer();
            FizzyLobbyController.Instance.UpdateLobbyName();
        }
        
        [Command]
        void CmdSetPlayerName(string _playerName)
        {
            OnPlayerNameChanged(playerName,_playerName);
        }
        
        void OnPlayerNameChanged(string oldValue, string newValue)
        {
            if (isServer)
            {
                this.playerName = newValue;
            }
            if(isClient)
            {
                //SteamLobbyController.Instance.UpdatePlayerList();
            }
        }
    }
}
