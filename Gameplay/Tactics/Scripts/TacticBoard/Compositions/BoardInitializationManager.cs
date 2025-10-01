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
    /// <summary>
    /// Handles the initialization and setup of the tactics board components.
    /// Focuses on structural setup - PlayerItemView handles its own tactical position initialization.
    /// </summary>
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
                            
                            // // Set up basic properties
                            // playerItemView.Controller = _controller;   
                            // playerItemView.ParentPositionZoneView = zoneView;
                            // playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.StartingList;
                            // playerItemView.StartingPlayersListIndex = index;
                            //
                            // // Initialize tactical position after all properties are set
                            // playerItemView.InitializeTacticalPosition();
                            //
                            // // Register for formation status changes after initialization
                            // playerItemView.OnFormationStatusChanged += _boardTacticManager.HandleFormationStatusChanged;
                            
                            // Initialize 
                            playerItemView.Initialize(_controller, zoneView, PlayerItemViewOwnerOption.StartingList, index);
                            
                            // Register for formation status changes after initialization
                            playerItemView.OnFormationStatusChanged += _boardTacticManager.HandleFormationStatusChanged;
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
                
                // Set up basic properties
                // playerItemView.Controller = _controller;
                // playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.BenchList;
                // playerItemView.BenchPlayersListIndex = i;
                // _controller.substitutesPlayersViews[i] = playerItemView;
                playerItemView.Initialize(_controller, null, PlayerItemViewOwnerOption.BenchList, i);
                _controller.substitutesPlayersViews[i] = playerItemView;
                
                // Set player data (handles null gracefully)
                var playerData = (i < _controller.SubstitutesPlayersItems.Count) ? _controller.SubstitutesPlayersItems[i] : null;
                playerItemView.SetPlayerData(playerData);
                
                // Register for formation status changes after setup
                playerItemView.OnFormationStatusChanged += _boardTacticManager.HandleFormationStatusChanged;
                
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

                // Set up basic properties
                // playerItemView.Controller = _controller;
                // playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.ReserveList;
                // playerItemView.ReservePlayersListIndex = i;
                // _controller.reservePlayersViews[i] = playerItemView;
                playerItemView.Initialize(_controller, null, PlayerItemViewOwnerOption.ReserveList, i);
                _controller.reservePlayersViews[i] = playerItemView;
                
                // Set player data
                playerItemView.SetPlayerData(_controller.ReservePlayersItems[i]);

                Debug.Log($"Reserve player {playerItemView.Profile.Name} initialized at index {i}");
            }
            
            _controller.reserveListSection.SetHeaderTextFormat("{0} ({1})", "Reserves");
            _controller.reserveListSection.UpdateFormattedHeaderText(_controller.ReservePlayersItems.Count.ToString());
        }
    }
} 