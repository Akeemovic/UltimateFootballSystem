using TMPro;
using UltimateFootballSystem.Common.Scripts.Utils;
using UltimateFootballSystem.Gameplay.Common.Widgets.StarRating;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Drag_and_Drop_Support;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.ViewModes;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.ViewModes.Options;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player
{
    public class PlayerItemViewMain : MonoBehaviour
    {
        [SerializeField]
        private PlayerItemViewModeOption _viewMode = PlayerItemViewModeOption.General;
    
        public PlayerItemViewModeOption ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                if (_viewMode == PlayerItemViewModeOption.General)
                {
                    playerItemGeneralViewMode.Show();
                    // Hide the rest
                    playerItemRolesViewMode.Hide();
                }
                else if (_viewMode == PlayerItemViewModeOption.Roles)
                {
                    playerItemRolesViewMode.Show();
                    // Hide the rest
                    playerItemGeneralViewMode.Hide();
                
                    // Disabled dragging
                    _playerItemView.GetComponent<PlayerItemDragSupport>().AllowDrag = false;
                    return;
                } 
            
                // Enable Dragging
                _playerItemView.GetComponent<PlayerItemDragSupport>().AllowDrag = true;
            }
        }

        [SerializeField]
        private PlayerItemView _playerItemView;
    
        /// <summary>
        /// CanvasGroup for manipulating opacity (Alpha) value.
        /// </summary>
        [SerializeField]
        private CanvasGroup canvasGroup;
    
        /// <summary>
        /// Name view.
        /// </summary>
        public TextMeshProUGUI NameText;
    
        /// <summary>
        /// Star Rating View.
        /// </summary>
        [SerializeField]
        private StarRatingView StarRatingView;

        /// <summary>
        /// Positions Text Background Image view.
        /// </summary>
        public Image PositionsTextBgImage;
    
        /// <summary>
        /// Positions view.
        /// </summary>
        public TextMeshProUGUI PositionsText;
    
        [Header("View Model UGUI Elements")]
        [SerializeField]
        private PlayerItemGeneralViewMode playerItemGeneralViewMode;
    
        [SerializeField]
        private PlayerItemRolesViewMode playerItemRolesViewMode;
    
        /// <summary>
        /// Other indicators (bookings, substitution status etc.) container.
        /// </summary>
        [SerializeField]
        private PlayerItemOtherIndicators playerItemOtherIndicators;
    
        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.G)) ViewMode = PlayerItemViewModeOption.General;
            // if (Input.GetKeyDown(KeyCode.R)) ViewMode = PlayerItemViewModeOption.Roles;
        }

        private void Start()
        {
        }

        /// <summary>
        /// Update view.
        /// </summary>
        public virtual void UpdateView()
        {
            // if (_playerItemView.Profile == null || _playerItemView.Profile.Id == 0 || string.IsNullOrEmpty(_playerItemView.Profile.Name) || _playerItemView.Profile.CurrentAbility == 0)
            // {
            //     _playerItemView.HasPlayerItem = false;
            //     SetDefaultView();
            // }
            // else
            // {
            //     _playerItemView.HasPlayerItem = true;
            //     NameText.text = _playerItemView.Profile.Name;
            //     StarRatingView.SetRating(_playerItemView.Profile.CurrentAbility);
            //     
            //     playerItemGeneralViewMode.UpdateView(_playerItemView);
            //     // playerItemRolesViewMode.UpdateView(_playerItemView);
            // }
        
            if (_playerItemView.HasPlayerItem)
            {
                NameText.text = _playerItemView.Profile.Name;
                StarRatingView.SetRating(_playerItemView.Profile.CurrentAbility);

                if (_playerItemView.InUseForFormation)
                {
                    // PositionsText.text = _playerItemView.Profile
                    // PositionsTextBgImage.color = _playerItemView.Profile.(_playerItemView.TacticalPositionOption);
                    // PositionsTextBgImage.color = NumToVisualHelper.GetColorFromValue(50);
                    PositionsTextBgImage.color = NumToVisualHelper.GetColorFromValue(80);
                }
            
            }
            else
            {
                SetDefaultView();
            }
        
            playerItemOtherIndicators.UpdateView(_playerItemView);
            playerItemGeneralViewMode.UpdateView(_playerItemView);
            // playerItemRolesViewMode.UpdateView(_playerItemView); 
        }
    
        public void FadeView()
        {
            canvasGroup.alpha = 0.5f;
        }
    
        public void BrightenView()
        {
            canvasGroup.alpha = 1;
        }
    
        public void Show()
        {
            gameObject.SetActive(true);
        }
    
        public void ToggleShow(bool show)
        {
            gameObject.SetActive(show);
        }
    
        public void Hide()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("mainView is not assigned in PlayerItemViewMain");
            }
        }
    
        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            SetDefaultView();
        }
    
        private void SetDefaultView()
        {
            NameText.text = "";
            StarRatingView.SetRating(0);
            PositionsText.text = "UNAVAILABLE";
        }
    }
}
