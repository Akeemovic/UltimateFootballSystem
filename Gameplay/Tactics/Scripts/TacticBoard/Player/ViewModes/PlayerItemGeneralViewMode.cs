// using TMPro;
// using UltimateFootballSystem.Common.Scripts.Utils;
// using UltimateFootballSystem.Core.TacticsEngine.Utils;
// using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// namespace UltimateFootballSystem.Gameplay.Tactics
// {
//     // public class PlayerItemGeneralViewMode : MonoBehaviour, IPlayerItemViewMode
//     public class PlayerItemGeneralViewMode : MonoBehaviour
//     {
//         [Header("Containers")]
//         [SerializeField] private GameObject leftContainer;
//         [SerializeField] private GameObject centerContainer;
//         [SerializeField] private GameObject rightContainer;
//         
//         [Header("Left Container Contents")]
//         [SerializeField] public Image conditionImage;
//         [SerializeField] public TextMeshProUGUI conditionText;
//         [SerializeField] public Image matchFitnessImage;
//         [SerializeField] public TextMeshProUGUI matchFitnessText;
//         
//         [Header("Center Container Contents")]
//         [SerializeField] public TextMeshProUGUI numberText; // This will now display player number OR placeholder text
//         /// <summary>
//         /// Player image (shirt display). This displays team shirt sprite or placeholder images.
//         /// </summary>
//         [SerializeField] private Image shirtImage; 
//         /// <summary>
//         /// Sprite for the team's shirt (when a player is valid).
//         /// </summary>
//         [SerializeField] private Sprite teamShirtSprite; 
//         /// <summary>
//         /// Sprite for the default placeholder image (always the same, only changes in opacity).
//         /// </summary>
//         [SerializeField] private Sprite defaultPlaceholderSprite; // CONSOLIDATED FIELD
//         
//         [Header("Right Container Contents")]
//         [SerializeField] public Image averageRatingImageView;
//         [SerializeField] public TextMeshProUGUI averageRatingText;
//         [SerializeField] public Image moraleImageView;
//
//         
//         // General Stuff
//         private PlayerItemView playerItemView;
//         [SerializeField] private Color shirtTextColor;
//         
//         private void Start()
//         {
//             shirtTextColor = Color.blue;
//             numberText.color = shirtTextColor;
//         
//             conditionText.color = Color.white;
//             matchFitnessText.color = Color.white;
//         }
//     
//         private void SetShirtAndTextForPlayer(Core.Entities.Player profile)
//         {
//             shirtImage.sprite = teamShirtSprite;
//             SetShirtOpacity(1f); // Full opacity for team shirt
//
//             numberText.color = shirtTextColor;
//             numberText.text = profile.SquadNumber;
//
//             conditionImage.color = NumToVisualHelper.GetColorFromValue(profile.Condition);
//             conditionText.text = profile.Condition.ToString();
//
//             matchFitnessImage.color = NumToVisualHelper.GetColorFromValue(profile.MatchFitness);
//             matchFitnessText.text = profile.MatchFitness.ToString();
//             
//             leftContainer.gameObject.SetActive(true); 
//             rightContainer.gameObject.SetActive(true);
//         }
//
//         private void SetShirtAndTextForPlaceholder()
//         {
//             if (playerItemView == null) return;
//
//             shirtImage.sprite = defaultPlaceholderSprite; // Always use the single default placeholder sprite
//
//             // Opacity for placeholders is now primarily managed by PlayerItemView's UpdateViewVisibility
//             // However, we can set a base opacity here if needed, or rely solely on PlayerItemView.
//             // For consistency, let's ensure it's not fully transparent initially if PlayerItemView doesn't override.
//             // SetShirtOpacity(0.5f); // A base opacity for all placeholders, PlayerItemView can override for StartingList
//
//             // Determine placeholder text based on owner option
//             switch (playerItemView.ViewOwnerOption)
//             {
//                 case PlayerItemViewOwnerOption.StartingList:
//                     if (playerItemView.ParentPositionZoneView != null)
//                     {
//                         numberText.text = TacticalPositionUtils.GetTypeForPosition(playerItemView.ParentPositionZoneView.tacticalPositionOption).ToString() ?? string.Empty;
//                     }
//                     else
//                     {
//                         Debug.LogWarning("GeneralViewMode: ParentPositionZoneView is null for StartingList placeholder.");
//                         numberText.text = string.Empty; // Default if no parent view
//                     }
//                     // Opacity will be handled by PlayerItemView's UpdateViewVisibility
//                     break;
//
//                 case PlayerItemViewOwnerOption.BenchList:
//                     numberText.text = "S" + (playerItemView.BenchPlayersListIndex + 1);
//                     SetShirtOpacity(0.5f); // Bench placeholders often appear slightly faded by default
//                     break;
//
//                 case PlayerItemViewOwnerOption.DragAndDrop:
//                     numberText.text = "DRAG"; // Generic text for drag operation
//                     SetShirtOpacity(0.5f); // Often semi-transparent during drag
//                     break;
//
//                 case PlayerItemViewOwnerOption.ReserveList:
//                     numberText.text = string.Empty; // Reserve list placeholders typically don't show position text
//                     SetShirtOpacity(0.5f); // Reserve placeholders might also be slightly faded
//                     break;
//
//                 default:
//                     numberText.text = string.Empty;
//                     SetShirtOpacity(0f); // Fully transparent if unhandled/default
//                     break;
//             }
//
//             // Hide player-specific containers for placeholders
//             leftContainer.gameObject.SetActive(false); 
//             rightContainer.gameObject.SetActive(false);
//
//             // Clear condition and match fitness text/images for placeholders
//             conditionImage.color = Color.clear; // Make transparent
//             conditionText.text = string.Empty;
//             matchFitnessImage.color = Color.clear; // Make transparent
//             matchFitnessText.text = string.Empty;
//             moraleImageView.gameObject.SetActive(false); // Hide morale image for placeholders
//         }
//
//         public void UpdateView(PlayerItemView view)
//         {
//             playerItemView = view; // Ensure playerItemView is set for internal logic
//
//             if (playerItemView.HasPlayerItem)
//             {
//                 SetShirtAndTextForPlayer(playerItemView.Profile);
//             }
//             else // It's a placeholder
//             {
//                 SetShirtAndTextForPlaceholder();
//             }
//         }
//
//         private void SetShirtOpacity(float opacity)
//         {
//             Color color = shirtImage.color;
//             color.a = opacity;
//             shirtImage.color = color;
//         }
//
//         public void Show() => gameObject.SetActive(true);
//         public void Hide() => gameObject.SetActive(false);
//     }
// }

// using TMPro;
// using UltimateFootballSystem.Common.Scripts.Utils;
// using UltimateFootballSystem.Core.TacticsEngine.Utils;
// using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// namespace UltimateFootballSystem.Gameplay.Tactics
// {
//     public class PlayerItemGeneralViewMode : MonoBehaviour
//     {
//         [Header("Containers")]
//         [SerializeField] private GameObject leftContainer;
//         [SerializeField] private GameObject centerContainer;
//         [SerializeField] private GameObject rightContainer;
//         
//         [Header("Left Container Contents")]
//         [SerializeField] public Image conditionImage;
//         [SerializeField] public TextMeshProUGUI conditionText;
//         [SerializeField] public Image matchFitnessImage;
//         [SerializeField] public TextMeshProUGUI matchFitnessText;
//         
//         [Header("Center Container Contents")]
//         [SerializeField] public TextMeshProUGUI numberText; // This will now display player number OR placeholder text
//         /// <summary>
//         /// Player image (shirt display). This displays team shirt sprite or placeholder images.
//         /// </summary>
//         [SerializeField] private Image shirtImage; 
//         /// <summary>
//         /// Sprite for the team's shirt (when a player is valid or for dimmed bench/dragged items).
//         /// </summary>
//         [SerializeField] private Sprite teamShirtSprite; 
//         /// <summary>
//         /// Sprite for the default placeholder image (for "not in use for formation" and ReserveList).
//         /// </summary>
//         [SerializeField] private Sprite defaultPlaceholderSprite; // CONSOLIDATED FIELD
//         
//         [Header("Right Container Contents")]
//         [SerializeField] public Image averageRatingImageView;
//         [SerializeField] public TextMeshProUGUI averageRatingText;
//         [SerializeField] public Image moraleImageView;
//
//         
//         // General Stuff
//         private PlayerItemView playerItemView;
//         [SerializeField] private Color shirtTextColor; // For player numbers and bench placeholder numbers
//         private readonly Color defaultPlaceholderTextColor = Color.white; // New: For default placeholder text
//         
//         private void Start()
//         {
//             shirtTextColor = Color.blue; // Normal text color for player numbers
//             numberText.color = shirtTextColor; // Initial set
//
//             conditionText.color = Color.white;
//             matchFitnessText.color = Color.white;
//             averageRatingText.color = Color.white; // Assuming white for average rating text
//         }
//     
//         private void SetShirtAndTextForPlayer(Core.Entities.Player profile)
//         {
//             shirtImage.sprite = teamShirtSprite;
//             SetShirtOpacity(1f); // Full opacity for team shirt
//
//             numberText.color = shirtTextColor;
//             numberText.text = profile.SquadNumber;
//
//             conditionImage.color = NumToVisualHelper.GetColorFromValue(profile.Condition);
//             conditionText.text = profile.Condition.ToString();
//
//             matchFitnessImage.color = NumToVisualHelper.GetColorFromValue(profile.MatchFitness);
//             matchFitnessText.text = profile.MatchFitness.ToString();
//
//             // Assuming average rating is for players
//             // averageRatingImageView.gameObject.SetActive(true); 
//             // averageRatingText.text = profile.CurrentAbility.ToString(); // Or a specific average rating property
//             // averageRatingImageView.color = NumToVisualHelper.GetColorFromValue(profile.CurrentAbility); // Example for rating background
//
//             moraleImageView.gameObject.SetActive(true); // Assuming morale is for players
//             // TODO: Set moraleImageView.sprite/color based on profile.Morale
//
//             leftContainer.gameObject.SetActive(true); 
//             rightContainer.gameObject.SetActive(true);
//             centerContainer.gameObject.SetActive(true); // Ensure center is active for player data
//         }
//
//         private void SetShirtAndTextForPlaceholder()
//         {
//             if (playerItemView == null) return;
//
//             // Hide player-specific containers for all placeholders initially
//             leftContainer.gameObject.SetActive(false); 
//             rightContainer.gameObject.SetActive(false);
//             
//             // Clear condition and match fitness text/images for placeholders
//             conditionImage.color = Color.clear; // Make transparent
//             conditionText.text = string.Empty;
//             matchFitnessImage.color = Color.clear; // Make transparent
//             matchFitnessText.text = string.Empty;
//             moraleImageView.gameObject.SetActive(false); // Hide morale image
//             averageRatingImageView.gameObject.SetActive(false); // Hide average rating image
//             averageRatingText.text = string.Empty; // Clear average rating text
//             
//             centerContainer.gameObject.SetActive(true); // Center container generally active for all types
//
//
//             switch (playerItemView.ViewOwnerOption)
//             {
//                 case PlayerItemViewOwnerOption.StartingList:
//                     // This is the "not in use for formation" placeholder
//                     shirtImage.sprite = defaultPlaceholderSprite; 
//                     SetShirtOpacity(0.5f); // Dimmed by default, PlayerItemView will override for InUseForFormation=true
//                     numberText.color = defaultPlaceholderTextColor; // White for placeholder text
//                     if (playerItemView.ParentPositionZoneView != null)
//                     {
//                         numberText.text = TacticalPositionUtils.GetTypeForPosition(playerItemView.ParentPositionZoneView.tacticalPositionOption).ToString() ?? string.Empty;
//                     }
//                     else
//                     {
//                         Debug.LogWarning("GeneralViewMode: ParentPositionZoneView is null for StartingList placeholder.");
//                         numberText.text = string.Empty;
//                     }
//                     break;
//
//                 case PlayerItemViewOwnerOption.BenchList:
//                     shirtImage.sprite = teamShirtSprite; // Show team shirt
//                     SetShirtOpacity(0.5f); // Dimmed
//                     numberText.color = shirtTextColor; // Normal text color
//                     numberText.text = "S" + (playerItemView.BenchPlayersListIndex + 1);
//                     break;
//
//                 case PlayerItemViewOwnerOption.DragAndDrop:
//                     shirtImage.sprite = teamShirtSprite; // Show team shirt
//                     SetShirtOpacity(1f); // No dimming
//                     numberText.color = shirtTextColor; // Normal text color
//                     numberText.text = "DRAG"; // Generic text for drag operation
//                     break;
//
//                 case PlayerItemViewOwnerOption.ReserveList:
//                     shirtImage.sprite = defaultPlaceholderSprite; // Uses default placeholder sprite
//                     SetShirtOpacity(0.5f); // Dimmed
//                     numberText.color = defaultPlaceholderTextColor; // White for placeholder text
//                     numberText.text = string.Empty; // Typically no text for reserve placeholders
//                     break;
//
//                 default:
//                     shirtImage.sprite = null; // No image
//                     SetShirtOpacity(0f); // Fully transparent
//                     numberText.color = defaultPlaceholderTextColor;
//                     numberText.text = string.Empty;
//                     break;
//             }
//         }
//
//         public void UpdateView(PlayerItemView view)
//         {
//             playerItemView = view; 
//
//             if (playerItemView.HasPlayerItem)
//             {
//                 SetShirtAndTextForPlayer(playerItemView.Profile);
//             }
//             else // It's a placeholder
//             {
//                 SetShirtAndTextForPlaceholder();
//             }
//         }
//
//         private void SetShirtOpacity(float opacity)
//         {
//             Color color = shirtImage.color;
//             color.a = opacity;
//             shirtImage.color = color;
//         }
//
//         public void Show() => gameObject.SetActive(true);
//         public void Hide() => gameObject.SetActive(false);
//     }
// }

using TMPro;
using UltimateFootballSystem.Common.Scripts.Utils;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemGeneralViewMode : MonoBehaviour
    {
        [Header("Containers")]
        [SerializeField] private GameObject leftContainer;
        [SerializeField] private GameObject centerContainer;
        [SerializeField] private GameObject rightContainer;
        
        [Header("Left Container Contents")]
        [SerializeField] public Image conditionImage;
        [SerializeField] public TextMeshProUGUI conditionText;
        [SerializeField] public Image matchFitnessImage;
        [SerializeField] public TextMeshProUGUI matchFitnessText;
        
        [Header("Center Container Contents")]
        [SerializeField] public TextMeshProUGUI numberText; // This will now display player number OR placeholder text
        [SerializeField] private Image shirtImage; 
        [SerializeField] private Sprite teamShirtSprite; 
        [SerializeField] private Color shirtTextColor; // For player numbers and bench placeholder numbers
        [SerializeField] private Sprite defaultPlaceholderSprite;
        [SerializeField] private Color defaultPlaceholderTextColor = Color.white; // For default placeholder text
        
        [Header("Right Container Contents")]
        [SerializeField] public Image averageRatingImageView;
        [SerializeField] public TextMeshProUGUI averageRatingText;
        [SerializeField] public Image moraleImageView;

        
        // General Stuff
        private PlayerItemView playerItemView;
        
        private void Start()
        {
            // Ensure values are set consistently at start
            shirtTextColor = Color.blue; // Example default, ensure this is set in Inspector for production
            numberText.color = shirtTextColor;
        
            conditionText.color = Color.white;
            matchFitnessText.color = Color.white;
            // averageRatingText.color = Color.white; 
        }
    
        private void SetShirtAndTextForPlayer(Core.Entities.Player profile)
        {
            // Set Shirt Image and Opacity
            shirtImage.sprite = teamShirtSprite;
            SetShirtOpacity(1f); // Full opacity for actual players

            // Set Number Text
            numberText.color = shirtTextColor;
            numberText.text = profile.SquadNumber;
            Debug.Log($"Squad Number: {profile.SquadNumber}");

            // Set Condition
            conditionImage.color = NumToVisualHelper.GetColorFromValue(profile.Condition);
            conditionText.text = profile.Condition.ToString();

            // Set Match Fitness
            matchFitnessImage.color = NumToVisualHelper.GetColorFromValue(profile.MatchFitness);
            matchFitnessText.text = profile.MatchFitness.ToString();
            
            // Set Average Rating
            // averageRatingImageView.gameObject.SetActive(true); 
            // averageRatingText.text = profile.CurrentAbility.ToString();
            // averageRatingImageView.color = NumToVisualHelper.GetColorFromValue(profile.CurrentAbility);

            // Set Morale
            // moraleImageView.gameObject.SetActive(true);
            // TODO: Set moraleImageView.sprite/color based on profile.Morale

            // Activate player-specific containers
            leftContainer.gameObject.SetActive(true); 
            rightContainer.gameObject.SetActive(true);
            centerContainer.gameObject.SetActive(true); 
        }

        private void SetShirtAndTextForPlaceholder()
        {
            if (playerItemView == null) return;

            // Hide player-specific containers for all placeholders initially
            leftContainer.gameObject.SetActive(false); 
            rightContainer.gameObject.SetActive(false);
            
            // Clear player-specific texts/images
            conditionImage.color = Color.clear;
            conditionText.text = string.Empty;
            matchFitnessImage.color = Color.clear;
            matchFitnessText.text = string.Empty;
            // moraleImageView.gameObject.SetActive(false);
            // averageRatingImageView.gameObject.SetActive(false);
            // averageRatingText.text = string.Empty;
            
            centerContainer.gameObject.SetActive(true); // Center container generally active for number/shirt

            // Clear so old data don't show
            numberText.text = string.Empty;
            
            switch (playerItemView.ViewOwnerOption)
            {
                case PlayerItemViewOwnerOption.StartingList:
                    /*
                     * This case handles placeholder view for in use or not in use player view with null players
                     * Show default placeholder sprite if not in use or Show team shirt sprite if in use
                     */  
                    // This is the "not in use for formation" type of placeholder
                    // shirtImage.sprite = defaultPlaceholderSprite; 
                    
                    // Opacity: Full if in use for formation, dimmed (50%) if not
                    SetShirtOpacity(playerItemView.InUseForFormation ? 1f : 0.6f);

                    shirtImage.sprite = playerItemView.InUseForFormation ? teamShirtSprite : defaultPlaceholderSprite;
                    numberText.color = playerItemView.InUseForFormation ? shirtTextColor : defaultPlaceholderTextColor; // White for placeholder text
                    
                    // numberText.text = string.Empty;
                    // if (playerItemView.ParentPositionZoneView != null)
                    // {
                    //     numberText.text = TacticalPositionUtils.GetTypeForPosition(playerItemView.ParentPositionZoneView.tacticalPositionOption).ToString() ?? string.Empty;
                    // }
                    // else
                    // {
                    //     Debug.LogWarning("GeneralViewMode: ParentPositionZoneView is null for StartingList placeholder.");
                    //     numberText.text = string.Empty;
                    // }
                    break;

                case PlayerItemViewOwnerOption.BenchList:
                    shirtImage.sprite = teamShirtSprite; // Show team shirt
                    SetShirtOpacity(0.6f); // Dimmed
                    numberText.color = shirtTextColor; // Normal text color for "S#"
                    numberText.text = "S" + (playerItemView.BenchPlayersListIndex + 1);
                    Debug.Log($"sub Number: {(playerItemView.BenchPlayersListIndex + 1)}");
                    break;

                case PlayerItemViewOwnerOption.DragAndDrop:
                    shirtImage.sprite = teamShirtSprite; // Show team shirt
                    SetShirtOpacity(1f); // No dimming
                    // numberText.color = shirtTextColor; // Normal text color
                    // numberText.text = "DRAG"; // Generic text for drag operation
                    break;

                case PlayerItemViewOwnerOption.ReserveList:
                    shirtImage.sprite = defaultPlaceholderSprite; // Uses default placeholder sprite
                    SetShirtOpacity(0.5f); // Dimmed
                    numberText.color = defaultPlaceholderTextColor; // White for placeholder text
                    numberText.text = string.Empty; // Typically no text for reserve placeholders
                    break;

                default:
                    shirtImage.sprite = null; // No image
                    SetShirtOpacity(0f); // Fully transparent
                    numberText.color = defaultPlaceholderTextColor;
                    numberText.text = string.Empty;
                    break;
            }
        }

        public void UpdateView(PlayerItemView view)
        {
            playerItemView = view; 

            if (playerItemView.HasPlayerItem)
            {
                SetShirtAndTextForPlayer(playerItemView.Profile);
            }
            else // It's a placeholder
            {
                SetShirtAndTextForPlaceholder();
            }
        }

        private void SetShirtOpacity(float opacity)
        {
            Color color = shirtImage.color;
            color.a = opacity;
            shirtImage.color = color;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}