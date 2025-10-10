using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.Tactics;
using UnityEngine;
using Lean.Pool;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Main view component for the tactics board.
    /// Manages ALL view components and UI updates.
    /// Notifies controller of user interactions.
    /// </summary>
    public class TacticBoardView : MonoBehaviour
    {
        [Header("Pitch Layout")]
        [SerializeField] public PositionZonesContainerView[] zoneContainerViews = new PositionZonesContainerView[6];

        [Header("UI Controls")]
        [SerializeField] public TMP_Dropdown viewModesDropDown;

        [Header("Player Item Lists")]
        [SerializeField] public ListSection substitutesListSection;
        [SerializeField] public ListSection reserveListSection;

        [Header("Prefabs")]
        [SerializeField] public Transform playerItemViewPrefab;

        // View references (managed by this view)
        public PlayerItemView[] startingPlayersViews = new PlayerItemView[24];
        public PlayerItemView[] substitutesPlayersViews;
        public PlayerItemView[] reservePlayersViews;
        public PlayerItemView dragInfoView;

        // Events for controller
        public event Action<int> OnViewModeChanged;
        public event Action<PlayerItemView> OnPlayerItemClicked;

        private TacticBoardController _controller;

        public void Initialize(TacticBoardController controller)
        {
            _controller = controller;

            SetupViewModeDropdown();
            SetupDragInfoView();
        }

        private void Awake()
        {
            SetupViewModeDropdown();
        }

        #region Initialization

        private void SetupViewModeDropdown()
        {
            if (viewModesDropDown == null) return;

            string[] optionNames = Enum.GetNames(typeof(PlayerItemViewModeOption));
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (string optionName in optionNames)
            {
                options.Add(new TMP_Dropdown.OptionData(optionName));
            }

            viewModesDropDown.ClearOptions();
            viewModesDropDown.AddOptions(options);

            // Register listener
            viewModesDropDown.onValueChanged.AddListener((selectedIndex) =>
            {
                OnViewModeChanged?.Invoke(selectedIndex);
            });
        }

        private void SetupDragInfoView()
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null || playerItemViewPrefab == null) return;

            dragInfoView = PlayerItemViewPoolManager.SpawnPlayerItemView(
                playerItemViewPrefab.GetComponent<PlayerItemView>(),
                canvas.gameObject.transform
            );
            dragInfoView.ViewOwnerOption = PlayerItemViewOwnerOption.DragAndDrop;
            dragInfoView.gameObject.SetActive(false);
        }

        /// <summary>
        /// Initialize starting player views from zone containers
        /// </summary>
        public void InitializeStartingPlayerViews(TacticBoardController controller, Action<bool> onFormationStatusChanged)
        {
            int index = 0;
            foreach (var zoneContainer in zoneContainerViews)
            {
                zoneContainer.gameObject.SetActive(true);
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);

                foreach (var zoneView in zoneViews)
                {
                    if (index < startingPlayersViews.Length)
                    {
                        var playerItemView = zoneView.GetComponentInChildren<PlayerItemView>(true);
                        if (playerItemView != null)
                        {
                            startingPlayersViews[index] = playerItemView;
                            playerItemView.Initialize(controller, zoneView, PlayerItemViewOwnerOption.StartingList, index);

                            if (onFormationStatusChanged != null)
                            {
                                playerItemView.OnFormationStatusChanged += onFormationStatusChanged;
                            }
                        }
                        index++;
                    }
                }
            }

            for (int i = 0; i < startingPlayersViews.Length; i++)
            {
                if (startingPlayersViews[i] == null)
                {
                    Debug.LogError($"StartingPlayersViews[{i}] is not initialized.");
                }
            }
        }

        /// <summary>
        /// Initialize substitute player views
        /// </summary>
        public void InitializeSubstituteViews(TacticBoardController controller, ObservableList<Player> substitutesData, int allowedSubstitutes, Action<bool> onFormationStatusChanged)
        {
            PlayerItemViewPoolManager.DespawnAllInContainer(substitutesListSection.viewsContainer);

            substitutesPlayersViews = new PlayerItemView[allowedSubstitutes];

            for (int i = 0; i < allowedSubstitutes; i++)
            {
                PlayerItemView playerItemView = PlayerItemViewPoolManager.SpawnPlayerItemView(
                    playerItemViewPrefab.GetComponent<PlayerItemView>(),
                    substitutesListSection.viewsContainer
                );

                playerItemView.Initialize(controller, null, PlayerItemViewOwnerOption.BenchList, i);
                substitutesPlayersViews[i] = playerItemView;

                var playerData = (i < substitutesData.Count) ? substitutesData[i] : null;
                playerItemView.SetPlayerData(playerData);

                if (onFormationStatusChanged != null)
                {
                    playerItemView.OnFormationStatusChanged += onFormationStatusChanged;
                }
            }

            if (allowedSubstitutes <= 9)
            {
                substitutesListSection.DisableScroll();
            }

            substitutesListSection.SetHeaderTextFormat("{0} ({1}/"+ allowedSubstitutes +")", "Substitutes");
            UpdateSubstitutesHeaderCount(substitutesData.Count(p => p != null));
        }

        /// <summary>
        /// Initialize reserve player views
        /// </summary>
        public void InitializeReserveViews(TacticBoardController controller, ObservableList<Player> reservesData)
        {
            PlayerItemViewPoolManager.DespawnAllInContainer(reserveListSection.viewsContainer);

            reservePlayersViews = new PlayerItemView[reservesData.Count];

            for (int i = 0; i < reservesData.Count; i++)
            {
                PlayerItemView playerItemView = PlayerItemViewPoolManager.SpawnPlayerItemView(
                    playerItemViewPrefab.GetComponent<PlayerItemView>(),
                    reserveListSection.viewsContainer
                );

                playerItemView.Initialize(controller, null, PlayerItemViewOwnerOption.ReserveList, i);
                reservePlayersViews[i] = playerItemView;
                playerItemView.SetPlayerData(reservesData[i]);
            }

            reserveListSection.SetHeaderTextFormat("{0} ({1})", "Reserves");
            UpdateReservesHeaderCount(reservesData.Count(p => p != null));
        }

        #endregion

        #region View Updates

        /// <summary>
        /// Update starting player views from mapping
        /// </summary>
        public void UpdateStartingPlayers(Dictionary<TacticalPositionOption, Player?> mapping)
        {
            foreach (var zoneContainer in zoneContainerViews)
            {
                if (zoneContainer == null) continue;

                foreach (var zoneView in zoneContainer.ZoneViews)
                {
                    if (zoneView == null || !zoneView.InUseForFormation) continue;

                    var position = zoneView.tacticalPositionOption;
                    if (mapping.TryGetValue(position, out var player))
                    {
                        zoneView.childPlayerItemView.SetPlayerData(player);
                    }
                }
            }
        }

        /// <summary>
        /// Refresh substitute views
        /// </summary>
        public void RefreshSubstituteViews(ObservableList<Player> substitutesData, int allowedSubstitutes)
        {
            if (substitutesPlayersViews == null || substitutesPlayersViews.Length != allowedSubstitutes)
            {
                return;
            }

            for (int i = 0; i < substitutesPlayersViews.Length; i++)
            {
                var view = substitutesPlayersViews[i];
                if (view == null) continue;

                Player player = null;
                if (i < substitutesData.Count)
                {
                    player = substitutesData[i];
                }

                view.SetPlayerData(player);
                view.BenchPlayersListIndex = i;
            }

            UpdateSubstitutesHeaderCount(substitutesData.Count(p => p != null));
        }

        /// <summary>
        /// Update substitutes header count
        /// </summary>
        public void UpdateSubstitutesHeaderCount(int count)
        {
            substitutesListSection.UpdateFormattedHeaderText(count.ToString());
        }

        /// <summary>
        /// Update reserves header count
        /// </summary>
        public void UpdateReservesHeaderCount(int count)
        {
            reserveListSection.UpdateFormattedHeaderText(count.ToString());
        }

        #endregion

        #region Formation Display

        /// <summary>
        /// Update formation zones based on tactical positions
        /// </summary>
        public void UpdateFormationZones(TacticalPositionOption[] formation, bool initCall = false)
        {
            foreach (var zoneContainer in zoneContainerViews)
            {
                if (zoneContainer == null) continue;

                foreach (var zoneView in zoneContainer.ZoneViews)
                {
                    if (zoneView == null) continue;

                    var isInFormation = formation.Contains(zoneView.tacticalPositionOption);
                    zoneView.SetInUseForFormation(isInFormation, initCall);
                }
            }
        }

        /// <summary>
        /// Show all usable player item views (for selection mode)
        /// </summary>
        public void ShowUsablePlayerItemViews()
        {
            for (var i = 0; i < zoneContainerViews.Length; i++)
            {
                if (i == 0) continue; // Skip first zone (GK)

                foreach (var zoneView in zoneContainerViews[i].ZoneViews)
                {
                    if(zoneView == null) continue;
                    zoneView.Show();
                    zoneView.childPlayerItemView.Show();
                }
            }
        }

        /// <summary>
        /// Hide unused player item views (only show formation)
        /// </summary>
        public void HideUnusedPlayerItemViews()
        {
            for (var i = 0; i < zoneContainerViews.Length; i++)
            {
                if (i == 0) continue; // Skip first zone

                var zoneViews = zoneContainerViews[i].ZoneViews;

                if (i >= 1 && i <= 4) // For 2nd to 5th containers
                {
                    if (zoneViews.Length >= 3) // Third child special handling
                    {
                        var thirdZoneView = zoneViews[2];
                        if (thirdZoneView != null && !thirdZoneView.childPlayerItemView.InUseForFormation)
                        {
                            thirdZoneView.Hide();
                        }
                        else
                        {
                            thirdZoneView.Show();
                        }
                    }

                    for (int j = 0; j < zoneViews.Length; j++)
                    {
                        if (j != 2) // Skip third zone (handled above)
                        {
                            var zoneView = zoneViews[j];
                            if (zoneView != null && !zoneView.childPlayerItemView.InUseForFormation)
                            {
                                zoneView.childPlayerItemView.Hide();
                            }
                        }
                    }
                }
                else if (i == zoneContainerViews.Length - 1) // Last container
                {
                    foreach (var zoneView in zoneViews)
                    {
                        if (zoneView != null && !zoneView.childPlayerItemView.InUseForFormation)
                        {
                            zoneView.childPlayerItemView.Hide();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Show only formation zones
        /// </summary>
        public void ShowFormationZonesOnly()
        {
            for (var i = 0; i < zoneContainerViews.Length; i++)
            {
                if (i == 0) continue;

                var zoneViews = zoneContainerViews[i].ZoneViews;

                if (i >= 1 && i <= 4)
                {
                    if (zoneViews.Length >= 3)
                    {
                        var thirdZoneView = zoneViews[2];
                        if (thirdZoneView != null)
                        {
                            if (thirdZoneView.childPlayerItemView.InUseForFormation)
                            {
                                thirdZoneView.Show();
                                thirdZoneView.childPlayerItemView.Show();
                            }
                            else
                            {
                                thirdZoneView.Hide();
                            }
                        }
                    }

                    for (int j = 0; j < zoneViews.Length; j++)
                    {
                        if (j != 2)
                        {
                            var zoneView = zoneViews[j];
                            if (zoneView != null)
                            {
                                zoneView.Show();
                                if (zoneView.childPlayerItemView.InUseForFormation)
                                {
                                    zoneView.childPlayerItemView.Show();
                                }
                                else
                                {
                                    zoneView.childPlayerItemView.Hide();
                                }
                            }
                        }
                    }
                }
                else if (i == zoneContainerViews.Length - 1)
                {
                    foreach (var zoneView in zoneViews)
                    {
                        if (zoneView != null)
                        {
                            zoneView.Show();
                            if (zoneView.childPlayerItemView.InUseForFormation)
                            {
                                zoneView.childPlayerItemView.Show();
                            }
                            else
                            {
                                zoneView.childPlayerItemView.Hide();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region User Interaction Notifications

        /// <summary>
        /// Called when a player item is clicked - notify controller
        /// </summary>
        public void NotifyPlayerItemClicked(PlayerItemView playerItemView)
        {
            OnPlayerItemClicked?.Invoke(playerItemView);
        }

        #endregion

        #region Cleanup

        public void Cleanup()
        {
            // Despawn all pooled views
            if (substitutesListSection != null)
            {
                PlayerItemViewPoolManager.DespawnAllInContainer(substitutesListSection.viewsContainer);
            }

            if (reserveListSection != null)
            {
                PlayerItemViewPoolManager.DespawnAllInContainer(reserveListSection.viewsContainer);
            }

            if (dragInfoView != null)
            {
                PlayerItemViewPoolManager.DespawnPlayerItemView(dragInfoView);
            }
        }

        #endregion
    }
}
