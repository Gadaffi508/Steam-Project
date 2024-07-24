using System.Collections.Generic;
using UnityEngine;

namespace Steamworks
{
    public class FizzyLobbies : MonoBehaviour
    {
        public static FizzyLobbies Instance;
        private void Awake() => Instance = this;

        public GameObject lobbyDataPrefab;
        public Transform lobbiesMenuContent;

        public List<GameObject> lobbyList = new List<GameObject>();

        public void FindLobby()
        {
            FizzySteamLobby.Instance.FindLobbies();
        }

        public void DisplayLobby(List<CSteamID> lobbyID, LobbyDataUpdate_t result)
        {
            for (int i = 0; i < lobbyID.Count; i++)
            {
                if (lobbyID[i].m_SteamID == result.m_ulSteamIDLobby)
                {
                    GameObject lobby = Instantiate(lobbyDataPrefab, lobbiesMenuContent);
                    LobbyData data = lobby.GetComponent<LobbyData>();
                
                    data.lobbyID = (CSteamID)lobbyID[i].m_SteamID;
                    data.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyID[i].m_SteamID, "name");
                    data.lobbyMembers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyID[i]);
                    data.SetLobbyData();
                
                    lobbyList.Add(lobby);
                }
            }
        }

        public void ClearLobby()
        {
            foreach (GameObject lobby in lobbyList)
            {
                Destroy(lobby);
            }
            lobbyList.Clear();
        }
    }
}
