using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Steamworks
{
    public class FizzySteamLobby : MonoBehaviour
    {
        public static FizzySteamLobby Instance;

        private void Awake() => Instance = this;
        
        [Header("Lobby")]
        public ulong CurrentLobbyID;
        
        public List<CSteamID> lobbyID = new List<CSteamID>();

        public string lobbyName;

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> joinRequest;
        protected Callback<LobbyEnter_t> lobbyEntered;

        protected Callback<LobbyMatchList_t> lobbyList;
        protected Callback<LobbyDataUpdate_t> lobbyData;

        protected Callback<LobbyChatMsg_t> lobbyChatMsg;

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

        private void Start()
        {
            if (!SteamManager.Initialized) return;

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

            lobbyList = Callback<LobbyMatchList_t>.Create(MatchLobby);
            lobbyData = Callback<LobbyDataUpdate_t>.Create(GetLobbyData);

            lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessage);
        }

        public void HostLobby(string lobbyNames)
        {
            lobbyName = lobbyNames;
            
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, Manager.maxConnections);
        }

        public void LeaveGame(CSteamID lobbyID)
        {
            SteamMatchmaking.LeaveLobby(lobbyID);
        }

        public void JoinLobby(CSteamID _lobbyID)
        {
            SteamMatchmaking.JoinLobby(_lobbyID);
        }

        public void FindLobbies()
        {
            if (lobbyID.Count > 0) lobbyID.Clear();

            SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
            SteamMatchmaking.RequestLobbyList();
        }

        void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK) return;

            Debug.Log("Lobby created");

            _manager.StartHost();

            CSteamID ulSteamID = new CSteamID(callback.m_ulSteamIDLobby);

            SteamMatchmaking.SetLobbyData(ulSteamID, "HostAddress",
                SteamUser.GetSteamID().ToString());

            SteamMatchmaking.SetLobbyData(ulSteamID, "name",
                lobbyName + " Lobby");
        }

        void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Requested");
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
            FizzyChat.Instance.OpenChat();
        }

        void OnLobbyEntered(LobbyEnter_t callback)
        {
            CurrentLobbyID = callback.m_ulSteamIDLobby;
            if (NetworkServer.active) return;

            CSteamID ulSteamID = new CSteamID(callback.m_ulSteamIDLobby);

            Debug.Log("Newtork Adress : ------------------" + _manager.networkAddress);

            _manager.networkAddress = SteamMatchmaking.GetLobbyData(ulSteamID, "HostAddress");
            _manager.StartClient();
        }

        void MatchLobby(LobbyMatchList_t callback)
        {
            if (FizzyLobbies.Instance.lobbyList.Count > 0) FizzyLobbies.Instance.ClearLobby();

            for (int i = 0; i < callback.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
                lobbyID.Add(lobbyId);
                SteamMatchmaking.RequestLobbyData(lobbyId);
            }
        }

        void OnLobbyChatMessage(LobbyChatMsg_t callback)
        {
            byte[] data = new byte[4096];

            CSteamID steamIDUser;
            EChatEntryType chatEntryType = EChatEntryType.k_EChatEntryTypeChatMsg;

            SteamMatchmaking.GetLobbyChatEntry((CSteamID)callback.m_ulSteamIDLobby, (int)callback.m_iChatID,
                out steamIDUser, data, data.Length, out chatEntryType);

            string message = System.Text.Encoding.UTF8.GetString(data);

            FizzyChat.Instance.DisplayChatMessage(SteamFriends.GetFriendPersonaName(steamIDUser), message);
        }

        void GetLobbyData(LobbyDataUpdate_t callback)
        {
            FizzyLobbies.Instance.DisplayLobby(lobbyID, callback);
        }
    }
}