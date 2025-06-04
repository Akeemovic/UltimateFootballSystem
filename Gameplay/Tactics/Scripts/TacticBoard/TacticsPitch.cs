using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class TacticsPitch : MonoBehaviour
    {
        [SerializeField]
        public PositionZonesContainerView[] zoneContainerViews = new PositionZonesContainerView[6];
    
        [SerializeField]
        public TMP_Dropdown viewModesDropDown;
    
        private void Awake()
        {
            // Debug.Log("Starting Awake() in TacticsPitch");

            // Debug.Log("Awake() in TacticsPitch completed successfully");
            SetupViewModeDropdownOptions();   
        }

        private void Start()
        {
            // zoneContainerViews = TacticsBoardController.Instance.zoneContainerViews;
        
        }

        private void Update()
        {
            // AdjustZoneContainerSpacing();
            // Debug.Log("Update()-ed");
            // if (Input.GetKeyDown(KeyCode.H))
            // {
            //     HideUnusedPlayerItemViews();
            // }
        }
    
        private void SetupViewModeDropdownOptions()
        {
            // Convert enum values to an array of strings
            string[] optionNames = Enum.GetNames(typeof(PlayerItemViewModeOption));

            // Create a list to hold the dropdown options
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            // Add each enum name as an option
            foreach (string optionName in optionNames)
            {
                options.Add(new TMP_Dropdown.OptionData(optionName));
            }

            // Clear existing options and add the new ones
            viewModesDropDown.ClearOptions();
            viewModesDropDown.AddOptions(options);
        }
    
        // public static void ShowUsablePlayerItemViews(PositionZonesContainerView[] zoneContainerViews)
        public void ShowUsablePlayerItemViews()
        {
            // using( new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2() )
            // {
            for (var i = 0; i < zoneContainerViews.Length; i++)
            {
                // exclude first zone 
                // as it only contains one zone/playerItemView that is always available
                if (i == 0) continue;
                foreach (var zoneView in zoneContainerViews[i].ZoneViews)
                {
                    if(zoneView == null) continue;
                    // zoneView.gameObject.SetActive(true);
                    // zoneView.childPlayerItemView.gameObject.SetActive(true);
                    zoneView.Show();
                    zoneView.childPlayerItemView.Show();

                    if (!zoneView.InUseForFormation || !zoneView.childPlayerItemView.InUseForFormation)
                    {
                        zoneView.childPlayerItemView.mainView.gameObject.SetActive(false);
                        zoneView.childPlayerItemView.placeholderView.gameObject.SetActive(true);
                        // zoneView.childPlayerItemView.mainView.Hide();
                        // zoneView.childPlayerItemView.placeholderView.Show();
                    }
                }
            }
            // }
        }
    
        // public static void HideUnusedPlayerItemViews(PositionZonesContainerView[] zoneContainerViews)
        public void HideUnusedPlayerItemViews()
        {
            // using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            // {
            for (var i = 0; i < zoneContainerViews.Length; i++)
            {
                // Exclude the first zone as it only contains one zone/playerItemView that is always available
                if (i == 0) continue;

                var zoneViews = zoneContainerViews[i].ZoneViews;

                if (i >= 1 && i <= 4) // For 2nd to 5th containers (index 1 to 4)
                {
                    if (zoneViews.Length >= 3) // Ensure there is a 3rd child zone
                    {
                        var thirdZoneView = zoneViews[2];
                        if (thirdZoneView != null && !thirdZoneView.childPlayerItemView.InUseForFormation)
                        {
                            // thirdZoneView.gameObject.SetActive(false);
                            thirdZoneView.Hide();
                        }
                        else
                        {
                            // thirdZoneView.gameObject.SetActive(true);
                            thirdZoneView.Show();
                        }
                    }

                    // Hide only the childPlayerItemView for other zones if they are not in formation
                    for (int j = 0; j < zoneViews.Length; j++)
                    {
                        if (j != 2) // Skip the 3rd zone here as it's handled above
                        {
                            var zoneView = zoneViews[j];
                            if (zoneView != null && !zoneView.childPlayerItemView.InUseForFormation)
                            {
                                // zoneView.childPlayerItemView.gameObject.SetActive(false);
                                zoneView.childPlayerItemView.Hide();
                            }
                        }
                    }
                }
                else if (i == zoneContainerViews.Length - 1) // For the last container
                {
                    foreach (var zoneView in zoneViews)
                    {
                        if (zoneView != null && !zoneView.childPlayerItemView.InUseForFormation)
                        {
                            // zoneView.childPlayerItemView.gameObject.SetActive(false);
                            zoneView.childPlayerItemView.Hide();
                        }
                    }
                }
            }
            // }
        }
    }
}
