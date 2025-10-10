using System;
using TMPro;
using UltimateFootballSystem.Core.Tactics;
using UltimateFootballSystem.Core.Tactics.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemViewPlaceholder : MonoBehaviour
    {
        [SerializeField]
        private PlayerItemView playerItemView;

        [SerializeField]
        private Image startingPlaceholderImage;
        [SerializeField]
        private TextMeshProUGUI startingPlaceholderPositionText;
        
        [SerializeField]
        private Image subPlaceholderImage;
        [SerializeField]
        private TextMeshProUGUI subPlaceholderPositionText;

        public void Show()
        {
            gameObject.SetActive(true);
            SetPositionText();
        }

        public void ToggleShow(bool show)
        {
            gameObject.SetActive(show);
            if (show)
            {
                SetPositionText();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdatePositionText()
        {
            SetPositionText();
        }

        private void Awake()
        {
            if (!playerItemView)
            {
                playerItemView = GetComponentInParent<PlayerItemView>();
            }
        }
         private void SetPositionText()
         {
             if (!playerItemView)
             {
                 startingPlaceholderPositionText.text = string.Empty;
                 subPlaceholderPositionText.text = string.Empty;
                 return;
             }
             
             switch (playerItemView.ViewOwnerOption)
             { 
                 case PlayerItemViewOwnerOption.StartingList:
                     if (playerItemView.ParentPositionZoneView != null) // Ensure ParentPositionZoneView exists for StartingList
                     {
                         startingPlaceholderPositionText.text =
                             TacticalPositionUtils
                                 .GetTypeForPosition(playerItemView.ParentPositionZoneView.tacticalPositionOption)
                                 .ToString() ?? string.Empty;
                         if (!startingPlaceholderImage.isActiveAndEnabled) startingPlaceholderImage.gameObject.SetActive(true);
                         if (subPlaceholderImage.isActiveAndEnabled) subPlaceholderImage.gameObject.SetActive(false);
                         if (!playerItemView.HasPlayerItem)
                         {
                             var currentColor = startingPlaceholderImage.color; // Get the current color
                             // if not in use formation, reduce (to 50% - 0,5f) opacity, else restore (to 100% - 1f)
                             currentColor.a = !playerItemView.InUseForFormation ? 0.5f : 1f;
                             startingPlaceholderImage.color = currentColor; // Assign the modified color back
                         }
                     }
                     else
                     {
                         Debug.LogWarning("PlayerItemViewPlaceholder: ParentPositionZoneView is null for StartingList placeholder.");
                         startingPlaceholderPositionText.text = string.Empty; // Or a default like "POS"
                         if (startingPlaceholderImage != null) startingPlaceholderImage.gameObject.SetActive(true); // Ensure image is active even if text is empty
                         if (subPlaceholderImage != null) subPlaceholderImage.gameObject.SetActive(false);
                     }
                     break; 
                 case PlayerItemViewOwnerOption.DragAndDrop:
                     // Show a generic drag image and some text
                     startingPlaceholderPositionText.text = string.Empty;
                     subPlaceholderPositionText.text = string.Empty; // Keep sub text empty
                     
                     // Ensure the correct image is active
                     if (!startingPlaceholderImage.isActiveAndEnabled) startingPlaceholderImage.gameObject.SetActive(true);
                     if (subPlaceholderImage.isActiveAndEnabled) subPlaceholderImage.gameObject.SetActive(false);
                     break;
                 case PlayerItemViewOwnerOption.BenchList: 
                     subPlaceholderPositionText.text = "S" + (playerItemView.BenchPlayersListIndex + 1);
                     if (!subPlaceholderImage.isActiveAndEnabled) subPlaceholderImage.gameObject.SetActive(true);
                     if (startingPlaceholderImage.isActiveAndEnabled) startingPlaceholderImage.gameObject.SetActive(false);
                     break;
                 case PlayerItemViewOwnerOption.ReserveList:
                     startingPlaceholderPositionText.text = string.Empty; 
                     subPlaceholderPositionText.text = string.Empty;
                     if (startingPlaceholderImage != null) startingPlaceholderImage.gameObject.SetActive(true); // Assuming you want a generic placeholder image for reserve
                     if (subPlaceholderImage != null) subPlaceholderImage.gameObject.SetActive(false);
                     break;
                 default:
                     // Fallback for any other unhandled PlayerItemViewOwnerOption
                     startingPlaceholderPositionText.text = string.Empty; 
                     subPlaceholderPositionText.text = string.Empty;
                     if (startingPlaceholderImage != null) startingPlaceholderImage.gameObject.SetActive(false); // Hide image by default for unhandled
                     if (subPlaceholderImage != null) subPlaceholderImage.gameObject.SetActive(false);
                     break;
             }
         }
    }
}
