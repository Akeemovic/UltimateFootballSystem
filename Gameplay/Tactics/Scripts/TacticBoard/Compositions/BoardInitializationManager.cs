using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UnityEngine;

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
            // Initialize starting player views
            InitializeStartingPlayersViews();

            // Set formation views based on starting positions mappings,
            // the Keys are TacticalPositions (representing the formation)
            _boardTacticManager.SetFormationViews(_controller.StartingPositionPlayerMapping.Keys.ToArray(), initCall: true);

            // Load players into the board
            LoadPlayers(_controller.StartingPositionPlayerMapping);
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

        public void LoadPlayers(Dictionary<TacticalPositionOption, Core.Entities.Player?> startingPositionPlayersMapping)
        {
            // Loop through each container of zone views on the board
            foreach (var zoneContainer in _controller.zoneContainerViews)
            {
                // Get all zone views (positions) within the current container
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
                
                // Loop through each zone tacticsPitch to find matching tactical positions
                foreach (var zoneView in zoneViews)
                {
                    // Check if the zone is in use for the formation
                    if (zoneView.InUseForFormation)
                    {
                        // Find player, set whatever is return, even nulls
                        if (startingPositionPlayersMapping.TryGetValue(zoneView.tacticalPositionOption, out var player))
                        {
                            // Set the player data in the zone's child player item tacticsPitch
                            zoneView.childPlayerItemView.SetPlayerData(player);
                        }
                    }
                }
            }
        }

        public void LoadPlayersAutofill(IEnumerable<Core.Entities.Player?> players)
        {
            Debug.Log("LoadPlayersAutofill method called.");
    
            int playerIndex = 0;
    
            // Loop through each container of zone views on the board
            foreach (var zoneContainer in _controller.zoneContainerViews)
            {
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
                // Loop through each zone tacticsPitch and assign players or set to null if not in use
                foreach (var zoneView in zoneViews)
                {
                    if (zoneView.InUseForFormation && playerIndex < players.Count())
                    {
                        var player = players.ElementAt(playerIndex);
                        
                        if (player != null)
                        {
                            Debug.Log($"LoadPlayersAutofill: Setting player {player.Name} in zone tacticsPitch {zoneView.name}.");
                            zoneView.childPlayerItemView.SetPlayerData(player);
                        }
                        else
                        {
                            Debug.Log($"LoadPlayersAutofill: No player to assign to zone tacticsPitch {zoneView.name}, setting data to null.");
                            zoneView.childPlayerItemView.SetPlayerData(null);
                        }

                        playerIndex++;
                    }
                    else
                    {
                        Debug.Log($"LoadPlayersAutofill: Zone tacticsPitch {zoneView.name} is not in use for formation, setting data to null.");
                        zoneView.childPlayerItemView.SetPlayerData(null);
                    }
                }
            }
            Debug.Log("LoadPlayersAutofill method completed.");
        }

        public void InitializeSubstitutePlayers()
        {
            // Clear existing views
            foreach (Transform child in _controller.substitutesListSection.viewsContainer)
            {
                Object.Destroy(child.gameObject);
            }

            // Initialize the SubstitutesPlayersViews array with the size of AllowedSubstitutes
            _controller.substitutesPlayersViews = new PlayerItemView[_controller.allowedSubstitutes];

            for (int i = 0; i < _controller.substitutesPlayersViews.Length; i++)
            {
                // Instantiate a new PlayerItemView (assuming you have a prefab for PlayerItemView)
                GameObject playerItemViewObject = Object.Instantiate(_controller.playerItemViewPrefab.gameObject, _controller.substitutesListSection.viewsContainer);
                PlayerItemView playerItemView = playerItemViewObject.GetComponent<PlayerItemView>();
                
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
            // Clear existing views
            foreach (Transform child in _controller.reserveListSection.viewsContainer)
            {
                Object.Destroy(child.gameObject);
            }

            _controller.reservePlayersViews = new PlayerItemView[_controller.ReservePlayersItems.Count];

            for (int i = 0; i < _controller.ReservePlayersItems.Count; i++)
            {
                // Instantiate a new PlayerItemView (assuming you have a prefab for PlayerItemView)
                GameObject playerItemViewObject = Object.Instantiate(_controller.playerItemViewPrefab.gameObject, _controller.reserveListSection.viewsContainer);
                PlayerItemView playerItemView = playerItemViewObject.GetComponent<PlayerItemView>();

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