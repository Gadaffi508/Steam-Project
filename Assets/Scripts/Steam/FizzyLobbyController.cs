using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Steamworks
{
    public class FizzyLobbyController : MonoBehaviour
    {
        public static FizzyLobbyController Instance;
        private void Awake() => Instance = this;

        public TMP_InputField lobbyName;
        public TMP_Text lobbyNameText;
        
        [Header("Player Item")] public GameObject playerListItemViewContent;
        public GameObject playerListItemPrefab;
        public GameObject localPlayerObject;

        [Header("Lobby")] public ulong currentLobbyID;
        public bool playerItemCreated = false;

        [Header("Ready")] public Button startGameButton;
        public TMP_Text readyButtonText;

        private List<PlayerItem> _playerListItems = new List<PlayerItem>();
        public FizzyPlayer localObject;

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

        public void HostLobby()
        {
            if (lobbyName.text == "")
            {
                Debug.Log("Lobby name is empty");
            }
            else 
                Debug.Log("Lobby name is  " + lobbyName.text);
            
            FizzySteamLobby.Instance.HostLobby(lobbyName.text);

            lobbyNameText.text = lobbyName.text + "'s Lobby";
        }

        public void ReadyPlayer()
        {
            localObject.ChangeReady();
        }

        public void UpdateButton()
        {
            if (localObject.ready)
            {
                readyButtonText.text = "UnReady";
            }
            else
            {
                readyButtonText.text = "Ready";
            }
        }

        public void CheckIfAllReady()
        {
            bool allReady = false;

            foreach (FizzyPlayer player in Manager.GamePlayer)
            {
                if (player.ready)
                {
                    allReady = true;
                }
                else
                {
                    allReady = false;
                    break;
                }
            }

            if (allReady == true)
            {
                if (localObject.playerIdNumber == 1)
                {
                    startGameButton.interactable = true;
                }
                else
                {
                    startGameButton.interactable = false;
                }
            }
            else
            {
                startGameButton.interactable = false;
            }
        }

        public void UpdateLobbyName()
        {
            currentLobbyID = FizzySteamLobby.Instance.CurrentLobbyID;
        }

        public void UpdatePlayerList()
        {
            if (!playerItemCreated) CreateHostPlayerItem();
            if (_playerListItems.Count < Manager.GamePlayer.Count) CreateClientPlayerItem();
            if (_playerListItems.Count > Manager.GamePlayer.Count) RemovePlayerItem();
            if (_playerListItems.Count == Manager.GamePlayer.Count) UpdatePlayerItem();
        }

        #region UpdatePlayerFunc

        public void CreateHostPlayerItem()
        {
            foreach (FizzyPlayer player in Manager.GamePlayer)
            {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab, playerListItemViewContent.transform);
                PlayerItem newPlayerItemComponent = newPlayerItem.GetComponent<PlayerItem>();
                
                newPlayerItemComponent.playerName = player.playerName;
                newPlayerItemComponent.connectionID = player.connectionID;
                newPlayerItemComponent.playerSteamID = player.playerSteamId;
                newPlayerItemComponent.ready = player.ready;
                newPlayerItemComponent.SetPlayerValues();

                _playerListItems.Add(newPlayerItemComponent);
            }

            playerItemCreated = true;
        }

        public void CreateClientPlayerItem()
        {
            foreach (FizzyPlayer player in _manager.GamePlayer)
            {
                if (!_playerListItems.Any(item => item.connectionID == player.connectionID))
                {
                    GameObject newPlayerItem = Instantiate(playerListItemPrefab, playerListItemViewContent.transform);
                    PlayerItem newPlayerItemComponent = newPlayerItem.GetComponent<PlayerItem>();
                    newPlayerItemComponent.playerName = player.playerName;
                    newPlayerItemComponent.connectionID = player.connectionID;
                    newPlayerItemComponent.playerSteamID = player.playerSteamId;
                    newPlayerItemComponent.ready = player.ready;
                    newPlayerItemComponent.SetPlayerValues();

                    _playerListItems.Add(newPlayerItemComponent);
                }
            }
        }

        public void RemovePlayerItem()
        {
            _playerListItems.RemoveAll(item =>
                !_manager.GamePlayer.Any(player => player.connectionID == item.connectionID));
        }

        public void UpdatePlayerItem()
        {
            foreach (FizzyPlayer player in Manager.GamePlayer)
            {
                foreach (PlayerItem playerList in _playerListItems)
                {
                    if (playerList.connectionID == player.connectionID)
                    {
                        playerList.playerName = player.playerName;
                        playerList.ready = player.ready;
                        playerList.SetPlayerValues();
                        if (player == localObject)
                        {
                            UpdateButton();
                        }
                    }
                }
            }
            CheckIfAllReady();
        }

        #endregion

        public void FindLocalPlayer()
        {
            localPlayerObject = GameObject.Find("LocalGamePlayer");
            localObject = localPlayerObject.GetComponent<FizzyPlayer>();
        }

        public void StartGame(string sceneName)
        {
            localObject.CanStartGame(sceneName);
        }
    }
}