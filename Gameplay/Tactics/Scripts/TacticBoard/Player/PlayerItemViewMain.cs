using TMPro;
using UltimateFootballSystem.Common.Scripts.Utils;
using UltimateFootballSystem.Gameplay.Common.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemViewMain : MonoBehaviour
    {
        [SerializeField]
        private PlayerItemViewModeOption _viewMode = PlayerItemViewModeOption.General;
    
        /// <summary>
        /// Container for all view modes - expected to switch children based on active view mode.
        /// </summary>
        [SerializeField] public GameObject topContainer;
        /// <summary>
        /// Container for player name, star rating & positions - children views remain the same (but w/ dynamic data) 
        /// </summary>
        [SerializeField] public GameObject bottomContainer;
        
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
        /// Name Text Background Image tacticsPitch.
        /// </summary>
        [Header("Name UGUI Elements")]
        public Image NameTextBgImage;
        
        /// <summary>
        /// Name tacticsPitch.
        /// </summary>
        public TextMeshProUGUI NameText;
    
        /// <summary>
        /// Star Rating View.
        /// </summary>
        [Header("Stars")]
        [SerializeField]
        private StarRatingView StarRatingView;
        
        /// <summary>
        /// Positions Text Background Image tacticsPitch.
        /// </summary>
        [Header("Positions UGUI Elements")]
        public Image PositionsTextBgImage;
    
        /// <summary>
        /// Positions tacticsPitch.
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

        private readonly Color defaultGrey = new Color(0.22f, 0.22f, 0.22f);
        private readonly Color defaultNameTextColor = new Color(0.09f, 0.02f, 0.44f);
        
        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.G)) ViewMode = PlayerItemViewModeOption.General;
            // if (Input.GetKeyDown(KeyCode.R)) ViewMode = PlayerItemViewModeOption.Roles;
        }

        private void Start()
        {
        }

        /// <summary>
        /// Update tacticsPitch.
        /// </summary>
        // public virtual void UpdateView()
        // {
        //     // if (_playerItemView.Profile == null || _playerItemView.Profile.Id == 0 || string.IsNullOrEmpty(_playerItemView.Profile.Name) || _playerItemView.Profile.CurrentAbility == 0)
        //     // {
        //     //     _playerItemView.HasPlayerItem = false;
        //     //     SetDefaultView();
        //     // }
        //     // else
        //     // {
        //     //     _playerItemView.HasPlayerItem = true;
        //     //     NameText.text = _playerItemView.Profile.Name;
        //     //     StarRatingView.SetRating(_playerItemView.Profile.CurrentAbility);
        //     //     
        //     //     playerItemGeneralViewMode.UpdateView(_playerItemView);
        //     //     // playerItemRolesViewMode.UpdateView(_playerItemView);
        //     // }
        //
        //     // update bottom container contents
        //     if (_playerItemView.HasPlayerItem)
        //     {
        //         // Activate Name, Star and Positions container if not in hierachy
        //         if(!bottomContainer.activeInHierarchy) bottomContainer.SetActive(true);
        //         NameTextBgImage.color = defaultNameTextColor;
        //         NameText.text = _playerItemView.Profile.Name;
        //         StarRatingView.SetRating(_playerItemView.Profile.CurrentAbility);
        //
        //         // if (_playerItemView.InUseForFormation)
        //         // {
        //         //     // PositionsText.text = _playerItemView.Profile
        //         //     // PositionsTextBgImage.color = _playerItemView.Profile.(_playerItemView.TacticalPositionOption);
        //         //     // PositionsTextBgImage.color = NumToVisualHelper.GetColorFromValue(50);
        //         //     PositionsTextBgImage.color = NumToVisualHelper.GetColorFromValue(80);
        //         // }
        //         
        //         PositionsText.SetText(_playerItemView.Profile.GetLearnedPositionsTypeString());
        //         // Update color to match position compatibility for player
        //         if (_playerItemView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
        //         {
        //             var playerPositionTypeRating =
        //                 _playerItemView.Profile.GetPositionRating(_playerItemView.TacticalPosition.PositionType);
        //             PositionsTextBgImage.color = NumToVisualHelper.GetColorFromValue(playerPositionTypeRating);
        //         }
        //     }
        //     else
        //     {
        //         // Hide container
        //         if(bottomContainer.activeInHierarchy) bottomContainer.SetActive(false);
        //         // SetDefaultView();
        //     }
        //
        //     // update top container contents
        //     playerItemOtherIndicators.UpdateView(_playerItemView);
        //     playerItemGeneralViewMode.UpdateView(_playerItemView);
        //     // playerItemRolesViewMode.UpdateView(_playerItemView); 
        // }
    
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
    
        // private void SetDefaultView()
        // {
        //     NameTextBgImage.color = defaultGrey;
        //     NameText.text = "";
        //     StarRatingView.SetRating(0);
        //     PositionsTextBgImage.color = defaultGrey;
        //     // PositionsText.text = "NONE";
        //     PositionsText.text = "-";
        // }
        
        public virtual void UpdateView()
        {
            if (_playerItemView.HasPlayerItem)
            {
                bottomContainer.SetActive(true); // Always activate if player exists
                NameTextBgImage.color = defaultNameTextColor;
                NameText.text = _playerItemView.Profile.Name;
                StarRatingView.SetRating(_playerItemView.Profile.CurrentAbility);

                PositionsText.SetText(_playerItemView.Profile.GetLearnedPositionsTypeString());
                if (_playerItemView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    var playerPositionTypeRating =
                        _playerItemView.Profile.GetPositionRating(_playerItemView.TacticalPosition.PositionType);
                    PositionsTextBgImage.color = NumToVisualHelper.GetColorFromValue(playerPositionTypeRating);
                }
            }
            else // No player item
            {
                bottomContainer.SetActive(false); // Always deactivate if no player exists
                SetDefaultView(); // Ensure visual elements are reset to default
            }

            playerItemOtherIndicators.UpdateView(_playerItemView);
            playerItemGeneralViewMode.UpdateView(_playerItemView); // This call triggers all placeholder logic
        }

        private void SetDefaultView() // This method's purpose is to reset the _contents_ of bottomContainer
        {
            NameTextBgImage.color = defaultGrey;
            NameText.text = "";
            StarRatingView.SetRating(0);
            PositionsTextBgImage.color = defaultGrey;
            PositionsText.text = "-";
        }
    }
}
