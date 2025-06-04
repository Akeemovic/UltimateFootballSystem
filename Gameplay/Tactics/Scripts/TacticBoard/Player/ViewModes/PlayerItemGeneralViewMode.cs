using TMPro;
using UltimateFootballSystem.Common.Scripts.Utils;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemGeneralViewMode : MonoBehaviour, IPlayerItemViewMode
    {
    
        [SerializeField] private GameObject leftContainer;
        [SerializeField] private GameObject rightContainer;
    
        /// <summary>
        /// Player image tacticsPitch.
        /// </summary>
        [SerializeField]
        private Image ShirtImage;
    
        /// <summary>
        /// ShirtSO text color.
        /// </summary>
        [SerializeField]
        private Color ShirtTextColor;
    
        /// <summary>
        /// SquadNumber tacticsPitch.
        /// </summary>
        public TextMeshProUGUI NumberText;
    
        /// <summary>
        /// Condition Image tacticsPitch.
        /// </summary>
        [SerializeField]
        public Image ConditionImage;
    
        /// <summary>
        /// Condition Text tacticsPitch.
        /// </summary>
        [SerializeField]
        public TextMeshProUGUI ConditionText;
    
        /// <summary>
        /// Match Fitness Image tacticsPitch.
        /// </summary>
        [SerializeField]
        public Image MatchFitnessImage;
    
        /// <summary>
        /// Match Fitness Text tacticsPitch.
        /// </summary>
        [SerializeField]
        public TextMeshProUGUI MatchFitnessText;
    
        /// <summary>
        /// Morale tacticsPitch.
        /// </summary>
        [SerializeField]
        public Image MoraleImageView;

        private PlayerItemView playerItemView;
    
        private void Start()
        {
            ShirtTextColor = Color.blue;
            NumberText.color = ShirtTextColor;
        
            ConditionText.color = Color.white;
            MatchFitnessText.color = Color.white;
        }
    
        private void SetShirtOpacity(float opacity)
        {
            // Set shirt opacity
            Color color = ShirtImage.color;
            color.a = opacity; 
            ShirtImage.color = color;
        }
    
        // public void UpdateView(Player profile)
        public void UpdateView(PlayerItemView view)
        {
            if (view.HasPlayerItem)
            {
                playerItemView = view;
                Debug.Log("General: Team Id: " + playerItemView.Controller.teamId + " - " + playerItemView.Profile.Name);
            
                var profile = view.Profile;
                leftContainer.gameObject.SetActive(true); 
                rightContainer.gameObject.SetActive(true);
            
                // Reduce shirt opacity
                SetShirtOpacity(1);

                NumberText.color = ShirtTextColor;
                NumberText.text = profile.SquadNumber;

                ConditionImage.color = NumToVisualHelper.GetColorFromValue(profile.Condition);
                ConditionText.text = profile.Condition.ToString();

                MatchFitnessImage.color = NumToVisualHelper.GetColorFromValue(profile.MatchFitness);
                MatchFitnessText.text = profile.MatchFitness.ToString();
            }
            else
            {
                SetDefaultView();
            }
        }

        public void SetDefaultView()
        {
            leftContainer.gameObject.SetActive(false); 
            rightContainer.gameObject.SetActive(false);
        
            ConditionText.text = "";
            NumberText.text = "";
        
            // Reduce shirt opacity
            SetShirtOpacity(.3f);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}