using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Gameplay.Tactics.Tactics;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class BoardTacticManager
    {
        private readonly TacticsBoardController _controller;
        private Tactic currentTactic;
        private readonly string saveDirectory = "Tactics";

        public BoardTacticManager(TacticsBoardController controller)
        {
            this._controller = controller;
            InitializeTactic();
            EnsureSaveDirectoryExists();
        }

        private void EnsureSaveDirectoryExists()
        {
            var path = Path.Combine(Application.persistentDataPath, saveDirectory);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void InitializeTactic()
        {
            // Create a new tactic with an empty formation
            currentTactic = new Tactic
            {
                Name = "Current Tactic",
                ActivePositions = new List<TacticalPosition>()
            };
        }

        public void HandleFormationStatusChanged(bool inUseForFormation)
        {
            Debug.Log("Formation status changed. Updating tactic based on active zones.");
            UpdateTacticFromActiveZones();
        }

        private void UpdateTacticFromActiveZones()
        {
            // Clear current positions
            currentTactic.ActivePositions.Clear();

            // Get all active zones and their tactical positions
            // foreach (var zoneContainer in _controller.zoneContainerViews)
            // {
            //     if (zoneContainer == null) continue;
            //
            //     foreach (var zoneView in zoneContainer.ZoneViews)
            //     {
            //         if (zoneView == null || !zoneView.InUseForFormation) continue;
            //
            //         var playerItemView = zoneView.childPlayerItemView;
            //         if (playerItemView == null || playerItemView.TacticalPosition == null) continue;
            //
            //         // Add the tactical position to the current tactic
            //         currentTactic.ActivePositions.Add(playerItemView.TacticalPosition);
            //     }
            // }
            
            foreach (var playerItemView in _controller.startingPlayersViews)
            {
                if (playerItemView == null) continue;
                if (!playerItemView.ParentPositionZoneView.InUseForFormation || !playerItemView.InUseForFormation) continue;
                if (playerItemView.TacticalPosition == null) continue;

                // Add the tactical position to the current tactic
                currentTactic.ActivePositions.Add(playerItemView.TacticalPosition);
            }

            currentTactic.Substitutes.Clear();
            foreach (var substituteItemPlayerView in _controller.substitutesPlayersViews)
            {
                if (substituteItemPlayerView == null) continue;
                // Add subs ids to subs
                currentTactic.Substitutes.Add(substituteItemPlayerView.Profile.Id);
            }

            // Update the formation string
            Debug.Log($"Current formation: {currentTactic.FormationToString()}");
        }

        public void SetFormationViews(TacticalPositionOption[] formationTacticalPositions, bool initCall = false)
        {
            // Show all usable if not init call 
            if (!initCall)
            {
                _controller.tacticsPitch.ShowUsablePlayerItemViews();
            }

            // Update each zone's formation status based on the provided positions
            foreach (var zoneContainer in _controller.zoneContainerViews)
            {
                if (zoneContainer == null) continue;

                foreach (var zoneView in zoneContainer.ZoneViews)
                {
                    if (zoneView == null) continue;

                    var isInFormation = formationTacticalPositions.Contains(zoneView.tacticalPositionOption);
                    zoneView.SetInUseForFormation(isInFormation, initCall);
                }
            }

            // Update the tactic after formation changes
            UpdateTacticFromActiveZones();
            
            // Hide unused positional zones
            _controller.tacticsPitch.HideUnusedPlayerItemViews();
        }

        public void SwapPlayers(int index1, int index2)
        {
            if (index1 >= 0 && index1 < currentTactic.ActivePositions.Count &&
                index2 >= 0 && index2 < currentTactic.ActivePositions.Count)
            {
                var player1 = currentTactic.ActivePositions[index1].AssignedPlayer;
                var player2 = currentTactic.ActivePositions[index2].AssignedPlayer;

                currentTactic.ActivePositions[index1].AssignPlayer(player2);
                currentTactic.ActivePositions[index2].AssignPlayer(player1);
            }
        }

        public Tactic GetCurrentTactic()
        {
            return currentTactic;
        }

        public void SaveCurrentTactic(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveDirectory, fileName);
            var json = currentTactic.ToJson();
            File.WriteAllText(path, json);
            Debug.Log($"Tactic saved to: {path}");
        }

        public void SaveTacticWithTimestamp()
        {
            // Get the formation string (e.g. "4-4-2")
            var formationString = currentTactic.FormationToString();
            
            // Get the positions in order (e.g. "GK-DC-DC-DC-DC-MC-MC-MC-MC-ST-ST")
            var positionsString = string.Join("-", currentTactic.ActivePositions
                .OrderBy(p => p.Position)
                .Select(p => p.Position.ToString()));

            // Create timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // Create filename
            var fileName = $"Tactic_{positionsString}_{timestamp}.json";

            // Save the tactic
            SaveCurrentTactic(fileName);
        }

        public void LoadTactic(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveDirectory, fileName);
            if (!File.Exists(path))
            {
                Debug.LogError($"Tactic file not found: {path}");
                return;
            }

            var json = File.ReadAllText(path);
            currentTactic = Tactic.FromJson(json);
            
            // Update the formation views based on the loaded tactic
            var formationPositions = currentTactic.ActivePositions
                .Select(p => p.Position)
                .ToArray();
            
            SetFormationViews(formationPositions, true);
        }

        public string[] GetSavedTactics()
        {
            var path = Path.Combine(Application.persistentDataPath, saveDirectory);
            return Directory.GetFiles(path, "*.json")
                .Select(Path.GetFileName)
                .ToArray();
        }
    }
}
