using System.Collections.Generic;
using System.Linq;
using DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterSelect
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [SerializeField] private CharacterScrollView scrollView;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private TextMeshProUGUI JobNameText;
        private List<CharacterData> characters;

        void Start()
        {
            leftButton.onClick.AddListener(scrollView.SelectPrevCell);
            rightButton.onClick.AddListener(scrollView.SelectNextCell);

            var characterDataList = GameDataManager.Instance.GetTable<PlayerData>("player_data").Values.ToList();

            characters = new List<CharacterData>();
            foreach (var playerData in characterDataList)
            {
                characters.Add(new CharacterData(playerData));
            }

            scrollView.UpdateData(characters);
            scrollView.SelectCell(0);
            UpdateCharacter(0);
            scrollView.OnSelectionChanged(UpdateCharacter);
        }
        
        void UpdateCharacter(int index)
        {
            JobNameText.text = TextManager.Instance != null
                ? TextManager.Instance.GetText(characters[index].data.job_type)
                : characters[index].data.job_type;
            
            CharacterSelectSceneManager.Instance.UpdatePlayerIcon(UserSession.UserId, characters[index].data);
        }
        
        public void SetInteractable(bool enable)
        {
            scrollView.IsLocked = !enable;
            scrollView.SetScrollDragEnabled(enable);

            leftButton.interactable  = enable;
            rightButton.interactable = enable;
        }
    }
}
