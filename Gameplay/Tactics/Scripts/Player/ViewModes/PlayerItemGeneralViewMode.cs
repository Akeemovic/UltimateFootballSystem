using TMPro;
using UltimateFootballSystem.Common.Scripts.Utils;
using UltimateFootballSystem.Core.Tactics.Utils;
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
        [SerializeField] public TextMeshProUGUI numberText;
        [SerializeField] private Image shirtImage; 
        [SerializeField] private Sprite teamShirtSprite; 
        [SerializeField] private Color shirtTextColor = Color.blue; // Set default in declaration
        [SerializeField] private Sprite defaultPlaceholderSprite;
        [SerializeField] private Color defaultPlaceholderTextColor = Color.white;
        private int defaultNumberTextFontSize = 12;
        
        [Header("Right Container Contents")]
        [SerializeField] public Image averageRatingImageView;
        [SerializeField] public TextMeshProUGUI averageRatingText;
        [SerializeField] public Image moraleImageView;

        // General Stuff
        private PlayerItemView playerItemView;
        private bool isInitialized = false;
        private bool isPostInitialized = false;
        
        private void Awake()
        {
            // Initialize critical values in Awake instead of Start
            InitializeDefaults();
        }
        
        private void InitializeDefaults()
        {
            // Ensure colors are set immediately
            if (shirtTextColor == Color.clear || shirtTextColor.a == 0)
            {
                shirtTextColor = Color.blue;
            }
            
            numberText.color = shirtTextColor;
            conditionText.color = Color.white;
            matchFitnessText.color = Color.white;
            
            // Validate sprite assignments
            if (teamShirtSprite == null)
            {
                Debug.LogWarning($"teamShirtSprite is not assigned on {gameObject.name}");
            }
            if (defaultPlaceholderSprite == null)
            {
                Debug.LogWarning($"defaultPlaceholderSprite is not assigned on {gameObject.name}");
            }
            
            isInitialized = true;
        }
        
        private void Start()
        {
            // Double-check initialization in case Awake was skipped
            if (!isInitialized)
            {
                InitializeDefaults();
            }
        }
        
        private void Update()
        {
            if (!isPostInitialized)
            {
                if (playerItemView.HasPlayerItem)
                {
                    SetShirtAndTextForPlayer(playerItemView.Profile);
                }
                else if (!playerItemView.HasPlayerItem)
                {
                    // if (playerItemView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList && !playerItemView.InUseForFormation)
                    // {
                    //     StartCoroutine(
                    //         DeferredUnusedStartingPlaceholderPostInitUpdate());
                    // }
                    // else
                    // {
                        SetShirtAndTextForPlaceholder();
                    // }
                }

                isPostInitialized = true;
            }
        }

        private System.Collections.IEnumerator DeferredUnusedStartingPlaceholderPostInitUpdate()
        {
            // yield return null;
            yield return new WaitForSeconds(1);
            SetShirtAndTextForPlaceholder();
        }
    
        private void SetShirtAndTextForPlayer(Core.Entities.Player profile)
        {
            if (!ValidateSprites()) return;
            
            // Set Shirt Image and Opacity
            shirtImage.sprite = teamShirtSprite;
            // shirtImage.overrideSprite = teamShirtSprite;
            SetShirtOpacity(1f);

            // Set Number Text
            numberText.color = shirtTextColor;
            numberText.text = profile.SquadNumber ?? "";
            numberText.fontSize = defaultNumberTextFontSize;
            Debug.Log($"Squad Number: {profile.SquadNumber}");

            // Set Condition
            conditionImage.color = NumToVisualHelper.GetColorFromValue(profile.Condition);
            conditionText.text = profile.Condition.ToString();

            // Set Match Fitness
            matchFitnessImage.color = NumToVisualHelper.GetColorFromValue(profile.MatchFitness);
            matchFitnessText.text = profile.MatchFitness.ToString();

            // Activate player-specific containers
            leftContainer.SetActive(true); 
            rightContainer.SetActive(true);
            centerContainer.SetActive(true); 
        }

        private void SetShirtAndTextForPlaceholder()
        {
            if (playerItemView == null) return;
            if (!ValidateSprites()) return;

            // Hide player-specific containers for all placeholders initially
            leftContainer.SetActive(false); 
            rightContainer.SetActive(false);
            
            // Clear player-specific texts/images
            conditionImage.color = Color.clear;
            conditionText.text = string.Empty;
            matchFitnessImage.color = Color.clear;
            matchFitnessText.text = string.Empty;
            
            centerContainer.SetActive(true);

            // Clear old data
            numberText.text = string.Empty;
            
            switch (playerItemView.ViewOwnerOption)
            {
                case PlayerItemViewOwnerOption.StartingList:
                    // TODO: Fix sprite no changing unless I use overrideSprite and text color not getting assigned
                    float opacity = playerItemView.InUseForFormation ? 1f : 0.6f;
                    SetShirtOpacity(opacity);

                    shirtImage.sprite = playerItemView.InUseForFormation ? teamShirtSprite : defaultPlaceholderSprite;
                    // shirtImage.overrideSprite = playerItemView.InUseForFormation ? teamShirtSprite : defaultPlaceholderSprite;

                    numberText.fontSize = 7;
                    // Position text for starting list placeholders
                    if (!playerItemView.InUseForFormation && playerItemView.ParentPositionZoneView != null)
                    {
                        var posType = TacticalPositionUtils.GetTypeForPosition(
                            playerItemView.ParentPositionZoneView.tacticalPositionOption);
                        numberText.text = posType.ToString();
                        
                        numberText.color = playerItemView.InUseForFormation ? shirtTextColor : defaultPlaceholderTextColor;
                    }
                    break;

                case PlayerItemViewOwnerOption.BenchList:
                    shirtImage.sprite = teamShirtSprite;
                    SetShirtOpacity(0.6f);
                    numberText.color = shirtTextColor;
                    numberText.fontSize = defaultNumberTextFontSize;
                    // Calculate substitute number
                    int subNumber = playerItemView.BenchPlayersListIndex + 1;
                    numberText.text = $"S{subNumber}";
                    Debug.Log($"Setting substitute number: S{subNumber} for index {playerItemView.BenchPlayersListIndex}");
                    break;

                case PlayerItemViewOwnerOption.DragAndDrop:
                    shirtImage.sprite = teamShirtSprite;
                    SetShirtOpacity(1f);
                    numberText.color = shirtTextColor;
                    numberText.text = ""; // No text for drag
                    break;

                case PlayerItemViewOwnerOption.ReserveList:
                    shirtImage.sprite = defaultPlaceholderSprite;
                    SetShirtOpacity(0.5f);
                    numberText.color = defaultPlaceholderTextColor;
                    numberText.text = string.Empty;
                    break;

                default:
                    shirtImage.sprite = defaultPlaceholderSprite;
                    SetShirtOpacity(0.3f);
                    numberText.color = defaultPlaceholderTextColor;
                    numberText.text = string.Empty;
                    break;
            }
        }

        public void UpdateView(PlayerItemView view)
        {
            playerItemView = view;
            
            // Ensure initialization before updating
            if (!isInitialized)
            {
                InitializeDefaults();
            }

            if (playerItemView.HasPlayerItem)
            {
                SetShirtAndTextForPlayer(playerItemView.Profile);
            }
            else
            {
                SetShirtAndTextForPlaceholder();
            }
        }

        private bool ValidateSprites()
        {
            bool isValid = true;
            
            if (teamShirtSprite == null)
            {
                Debug.LogError($"teamShirtSprite is null on {gameObject.name}. Please assign it in the Inspector.");
                isValid = false;
            }
            
            if (defaultPlaceholderSprite == null)
            {
                Debug.LogError($"defaultPlaceholderSprite is null on {gameObject.name}. Please assign it in the Inspector.");
                isValid = false;
            }
            
            return isValid;
        }

        private void SetShirtOpacity(float opacity)
        {
            if (shirtImage != null)
            {
                Color color = shirtImage.color;
                color.a = Mathf.Clamp01(opacity);
                shirtImage.color = color;
            }
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
        
        // Force refresh method for troubleshooting
        public void ForceRefresh()
        {
            if (!isInitialized)
            {
                InitializeDefaults();
            }
            
            if (playerItemView != null)
            {
                UpdateView(playerItemView);
            }
        }
    }
}