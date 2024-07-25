using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Steamworks
{
    public class LobbyData : MonoBehaviour
    {
        public CSteamID lobbyID;

        public string lobbyName;
        public int lobbyMembers;

        public TMP_Text lobbyNameText;
        public TMP_Text lobbyMembersText;

        public void SetLobbyData()
        {
            if (lobbyName == "")
                lobbyNameText.text = "Empty ' lobby";
            else
                lobbyNameText.text = lobbyName;

            lobbyMembersText.text = lobbyMembers + " / 11";
        }

        public void JoinLobby()
        {
            FizzySteamLobby.Instance.JoinLobby(lobbyID);
            FizzyChat.Instance.OpenChat();
        }
    }
}
