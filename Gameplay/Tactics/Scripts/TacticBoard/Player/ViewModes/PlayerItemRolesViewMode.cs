using TMPro;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.ViewModes
{
    public class PlayerItemRolesViewMode : MonoBehaviour, IPlayerItemViewMode
    //, IPointerClickHandler
    {
        [SerializeField]
        public TextMeshProUGUI SelectedRoleNameTextView;
    
        [SerializeField]
        public TextMeshProUGUI SelectedDutyTextView;
    
        private PlayerItemView playerItemView;
    
        private void Awake()
        {
        }

        private void Start()
        {
            SelectedRoleNameTextView.text = "Box-To-Box Midfielder";
            SelectedDutyTextView.text = "Support";
        }
    
        public void Show()
        {
            gameObject.SetActive(true);
        }
    
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    
        // public void UpdateView(TacticalRole selectedTacticalRole)
        public void UpdateView(PlayerItemView view)
        {
            playerItemView = view;
            Debug.Log("Role: Team Id: " + playerItemView.Controller.teamId);
        
            // SelectedRoleNameTextView.text = selectedTacticalRole.RoleName;
            // SelectedDutyTextView.text = selectedTacticalRole.SelectedDuty.ToString();
        }

        public void SetDefaultView()
        {
            gameObject.SetActive(false);
        }

        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     Debug.Log("OnPointerClick roles mode");
        //     var dialog = playerItemView.Controller.RoleSelectorDialog.Clone();
        //     dialog.Show();
        //     Debug.Log("dialog", dialog);
        // }
    }
}