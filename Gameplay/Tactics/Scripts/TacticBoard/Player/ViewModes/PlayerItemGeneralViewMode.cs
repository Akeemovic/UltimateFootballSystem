using TMPro;
using UltimateFootballSystem.Common.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.ViewModes
{
    public class PlayerItemGeneralViewMode : MonoBehaviour, IPlayerItemViewMode
    {
    
        [SerializeField] private GameObject leftContainer;
        [SerializeField] private GameObject rightContainer;
    
        /// <summary>
        /// Player image view.
        /// </summary>
        [SerializeField]
        private Image ShirtImage;
    
        /// <summary>
        /// ShirtSO text color.
        /// </summary>
        [SerializeField]
        private Color ShirtTextColor;
    
        /// <summary>
        /// SquadNumber view.
        /// </summary>
        public TextMeshProUGUI NumberText;
    
        /// <summary>
        /// Condition Image view.
        /// </summary>
        [SerializeField]
        public Image ConditionImage;
    
        /// <summary>
        /// Condition Text view.
        /// </summary>
        [SerializeField]
        public TextMeshProUGUI ConditionText;
    
        /// <summary>
        /// Match Fitness Image view.
        /// </summary>
        [SerializeField]
        public Image MatchFitnessImage;
    
        /// <summary>
        /// Match Fitness Text view.
        /// </summary>
        [SerializeField]
        public TextMeshProUGUI MatchFitnessText;
    
        /// <summary>
        /// Morale view.
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