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
        
        [SyncVar(hook = nameof(OnPlayerReadyChanged))]
        public bool ready;
        
        [SyncVar] public GameObject playerModel;
        
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
        
        public PlayerMove _controller;

        private NetworkAnimator anim;
        
        
        public override void OnStartAuthority()
        {
            DontDestroyOnLoad(this);
            CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
            gameObject.name = "LocalGamePlayer";
        
            FizzyLobbyController.Instance.FindLocalPlayer();
            FizzyLobbyController.Instance.UpdateLobbyName();
        }
        
        public override void OnStartClient()
        {
            Manager.GamePlayer.Add(this);
            FizzyLobbyController.Instance.UpdateLobbyName();
            FizzyLobbyController.Instance.UpdatePlayerList();
        }
        
        public override void OnStopClient()
        {
            Manager.GamePlayer.Remove(this);
            FizzyLobbyController.Instance.UpdatePlayerList();
        }
        
        public void CanStartGame(string sceneName)
        {
            CmdStartGame(sceneName);
        }

        public void ChangeReady()
        {
            if (isLocalPlayer)
            {
                CmdSetPlayerReady();
            }
        }
        
        [Command]
        void CmdStartGame(string sceneName)
        {
            _manager.StartGame(sceneName);
            
            if(!isLocalPlayer) return;
            
            _controller.enabled = true;
            
            playerModel.SetActive(true);
        }

        [Command]
        void CmdSetPlayerReady()
        {
            OnPlayerReadyChanged(ready,!ready);
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
                FizzyLobbyController.Instance.UpdatePlayerList();
            }
        }

        void OnPlayerReadyChanged(bool oldValue, bool newValue)
        {
            if (isServer)
            {
                this.ready = newValue;
            }
            if(isClient)
            {
                FizzyLobbyController.Instance.UpdatePlayerList();
            }
        }
    }
}
