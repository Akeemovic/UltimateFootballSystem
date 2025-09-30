using TMPro;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemRolesViewMode : MonoBehaviour, IPlayerItemViewMode
    //, IPointerClickHandler
    {
        [SerializeField]
        public TextMeshProUGUI SelectedRoleNameTextView;
    
        [SerializeField]
        public TextMeshProUGUI SelectedDutyTextView;
    
        [SerializeField] private PlayerItemView playerItemView;
    
        private void Awake()
        {
        }

        private void Start()
        {
            if (playerItemView == null)
            {
                playerItemView = GetComponentInParent<PlayerItemView>();
            }

            // Subscribe to role/duty change events
            if (playerItemView != null)
            {
                playerItemView.OnRoleChanged += OnRoleChanged;
                playerItemView.OnDutyChanged += OnDutyChanged;
            }

            // Initialize with placeholder text - will be updated when UpdateView is called
            SelectedRoleNameTextView.text = "Role";
            SelectedDutyTextView.text = "Duty";
            UpdateRoleDisplay();
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

            UpdateRoleDisplay();
        }

        private void UpdateRoleDisplay()
        {
            if (playerItemView?.TacticalPosition?.SelectedRole != null)
            {
                SelectedRoleNameTextView.text = playerItemView.TacticalPosition.SelectedRole.RoleName;
                SelectedDutyTextView.text = playerItemView.TacticalPosition.SelectedRole.SelectedDuty.ToString();
            }
            else
            {
                SelectedRoleNameTextView.text = "No Role";
                SelectedDutyTextView.text = "No Duty";
            }
        }

        // Event handlers for role/duty changes
        private void OnRoleChanged(TacticalRoleOption newRole)
        {
            UpdateRoleDisplay();
        }

        private void OnDutyChanged(TacticalDutyOption newDuty)
        {
            UpdateRoleDisplay();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (playerItemView != null)
            {
                playerItemView.OnRoleChanged -= OnRoleChanged;
                playerItemView.OnDutyChanged -= OnDutyChanged;
            }
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