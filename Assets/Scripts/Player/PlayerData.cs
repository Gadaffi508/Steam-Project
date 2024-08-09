using System;
using UnityEngine;
using UnityEngine.UI;

namespace Steamworks
{
    [CreateAssetMenu(menuName = "ScriptalObject/Player/Data", fileName = "Player Data")]
    public class SelectPlayer : ScriptableObject
    {
        public GameObject playerModel;

        public Sprite playerImage;
    }
        
    public class PlayerData : MonoBehaviour
    {
        public SelectPlayer playerData;

        private Image buttonImage;

        private void Start()
        {
            buttonImage = GetComponent<Image>();

            buttonImage.sprite = playerData.playerImage;
        }
    }
}
