using DataModels;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelect
{
    public class CharacterData
    {
        public PlayerData data;
        public CharacterData(PlayerData data)
        {
            this.data = data;
        }
    }
}