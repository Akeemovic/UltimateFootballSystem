using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UnityEngine;
using Lean.Pool;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class BoardInitializationManager
    {
        private readonly TacticsBoardController _controller;
        private readonly BoardTacticManager _boardTacticManager;

        public BoardInitializationManager(TacticsBoardController controller, BoardTacticManager boardTacticManager)
        {
            _controller = controller;
            _boardTacticManager = boardTacticManager;
        }

        public void InitializeAndSetupBoard()
        {
            // using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            // {
                // Initialize starting player views
                InitializeStartingPlayersViews();

                _controller.SetFormation(_controller.StartingPositionPlayerMapping.Keys.ToArray(), initCall: true);
                // Set formation views based on starting positions mappings,
                // the Keys are TacticalPositions (representing the formation)
                // _boardTacticManager.SetFormationViews();

                // Players are loaded via UI interactions or bulk selection tools
            // }
        }

        public void InitializeStartingPlayersViews()
        {
            int index = 0;
            foreach (var zoneContainer in _controller.zoneContainerViews)
            {
                zoneContainer.gameObject.SetActive(true);
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
                foreach (var zoneView in zoneViews)
                {
                    if (index < _controller.startingPlayersViews.Length)
                    {
                        var playerItemView = zoneView.GetComponentInChildren<PlayerItemView>(true);
                        if (playerItemView != null)
                        {
                            _controller.startingPlayersViews[index] = playerItemView;
                            
                            // Register for formation status changes
                            playerItemView.OnFormationStatusChanged += _boardTacticManager.HandleFormationStatusChanged;
                            playerItemView.Controller = _controller;   
                            playerItemView.ParentPositionZoneView = zoneView;
                            playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.StartingList;
                            playerItemView.StartingPlayersListIndex = index;
                            var tacticalPositionOption = zoneView.tacticalPositionOption;
                            // Create an instance of the tactical position to manage positions related roles & duties 
                            playerItemView.TacticalPosition = new TacticalPosition(
                                TacticalPositionUtils.GetGroupForPosition(tacticalPositionOption),
                                tacticalPositionOption,
                                RoleManager.GetRolesForPosition(tacticalPositionOption)
                                    .Select(roleOption => RoleManager.GetRole(roleOption))
                                    .ToList()
                            );
                        }
                        index++;
                    }
                }
            }

            for (int i = 0; i < _controller.startingPlayersViews.Length; i++)
            {
                if (_controller.startingPlayersViews[i] == null)
                {
                    Debug.LogError($"StartingPlayersViews[{i}] is not initialized.");
                }
            }
        }

        // LoadPlayers and LoadPlayersAutofill methods removed - obsolete
        // Players are now assigned via UI interactions or BoardSelection bulk tools

        public void InitializeSubstitutePlayers()
        {
            // Clear existing views using LeanPool
            PlayerItemViewPoolManager.DespawnAllInContainer(_controller.substitutesListSection.viewsContainer);

            // Initialize the SubstitutesPlayersViews array with the size of AllowedSubstitutes
            _controller.substitutesPlayersViews = new PlayerItemView[_controller.allowedSubstitutes];

            for (int i = 0; i < _controller.substitutesPlayersViews.Length; i++)
            {
                // Spawn a new PlayerItemView using LeanPool
                PlayerItemView playerItemView = PlayerItemViewPoolManager.SpawnPlayerItemView(_controller.playerItemViewPrefab.GetComponent<PlayerItemView>(), _controller.substitutesListSection.viewsContainer);
                
                playerItemView.Controller = _controller;
                playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.BenchList;
                // Register for formation status changes
                playerItemView.OnFormationStatusChanged += _boardTacticManager.HandleFormationStatusChanged;
                // if (i < _controller.SubstitutesPlayersItems.Count && _controller.SubstitutesPlayersItems[i] != null)
                // {
                //     // Set data for the player profile tacticsPitch if the item exists
                //     playerItemView.SetPlayerData(_controller.SubstitutesPlayersItems[i]);
                // }
                // else
                // {
                //     // Show placeholder and hide the main tacticsPitch if the player item is null or out of range
                //     playerItemView.placeholderView.Show();
                //     playerItemView.mainView.Hide();
                // }
                // playerItemView.BenchPlayersListIndex = i;
                // _controller.substitutesPlayersViews[i] = playerItemView;
                // playerItemView.placeholderView.UpdatePositionText();
                
                if (i < _controller.SubstitutesPlayersItems.Count && _controller.SubstitutesPlayersItems[i] != null)
                {
                    // Set data for the player profile tacticsPitch if the item exists
                    playerItemView.SetPlayerData(_controller.SubstitutesPlayersItems[i]);
                }
                else
                {
                    playerItemView.SetPlayerData(null);
                }
                playerItemView.BenchPlayersListIndex = i;
                _controller.substitutesPlayersViews[i] = playerItemView;
                
                // Logging to verify correct initialization
                string playerName = playerItemView.HasPlayerItem ? playerItemView.Profile.Name : "Placeholder";
                Debug.Log($"Substitutes player {playerName} initialized at index {i}");
            }
    
            if (_controller.allowedSubstitutes <= 9)
            {
                _controller.substitutesListSection.DisableScroll();
            }
            
            _controller.substitutesListSection.SetHeaderTextFormat("{0} ({1}/"+ _controller.allowedSubstitutes +")", "Substitutes");
            _controller.substitutesListSection.UpdateFormattedHeaderText(_controller.SubstitutesPlayersItems.Count.ToString());
        }

        public void InitializeReservePlayers()
        {
            // Clear existing views using LeanPool
            PlayerItemViewPoolManager.DespawnAllInContainer(_controller.reserveListSection.viewsContainer);

            _controller.reservePlayersViews = new PlayerItemView[_controller.ReservePlayersItems.Count];

            for (int i = 0; i < _controller.ReservePlayersItems.Count; i++)
            {
                // Spawn a new PlayerItemView using LeanPool
                PlayerItemView playerItemView = PlayerItemViewPoolManager.SpawnPlayerItemView(_controller.playerItemViewPrefab.GetComponent<PlayerItemView>(), _controller.reserveListSection.viewsContainer);

                playerItemView.Controller = _controller;
                playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.ReserveList;
                // Set data for the player profile tacticsPitch
                playerItemView.SetPlayerData(_controller.ReservePlayersItems[i]);
                playerItemView.ReservePlayersListIndex = i;
                _controller.reservePlayersViews[i] = playerItemView;

                Debug.Log($"Reserve player {playerItemView.Profile.Name} initialized at index {i}");
            }
            
            _controller.reserveListSection.SetHeaderTextFormat("{0} ({1})", "Reserves");
            _controller.reserveListSection.UpdateFormattedHeaderText(_controller.ReservePlayersItems.Count.ToString());
        }
    }
} 