using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.TacticsEngine;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Compositions
{
    public class FormationManager
    {
        private TacticsBoardController controller;

        // Constructor that takes the controller as a parameter
        public FormationManager(TacticsBoardController controller)
        {
            this.controller = controller;
        }

        public void HandleFormationStatusChanged(bool inUseForFormation)
        {
            Debug.Log("Formation status changed. Generating formation in background.");
            controller.StartCoroutine(GenerateFormationInBackground());
        }

        public IEnumerator GenerateFormationInBackground()
        {
            yield return null;
            CreateFormation(out controller.startingPositionIdMapping);
            LogFormation(controller.startingPositionIdMapping);
        }

        public void SetFormationViews(TacticalPositionOption[] formationTacticalPositions, bool initCall = false)
        {
            // Show all usable if not init call 
            if (!initCall)
            {
                controller.View.ShowUsablePlayerItemViews();
            }

            foreach (var playerItemView in controller.StartingPlayersViews)
            {
                if (playerItemView == null) continue;
                var isInFormation = false;

                foreach (var formationPosition in formationTacticalPositions)
                {
                    if (playerItemView.ParentPositionZoneView.tacticalPositionOption == formationPosition)
                    {
                        isInFormation = true;
                        break;
                    }
                }

                // Update the formation status of the player view
                playerItemView.SetInUseForFormation(isInFormation, isCalledFromFormationInit: true);
            }

            controller.View.HideUnusedPlayerItemViews();
        }

        public void CreateFormation(out Dictionary<TacticalPositionOption, int?> positionPlayerMapping)
        {
            positionPlayerMapping = new Dictionary<TacticalPositionOption, int?>();

            foreach (var container in controller.zoneContainerViews)
            {
                foreach (var zoneView in container.ZoneViews)
                {
                    if (zoneView == null || !zoneView.childPlayerItemView.InUseForFormation)
                    {
                        continue;
                    }

                    var tacticalPosition = zoneView.tacticalPositionOption;
                    var playerId = zoneView.childPlayerItemView.Profile.Id;

                    if (positionPlayerMapping.ContainsKey(tacticalPosition))
                    {
                        continue;
                    }

                    positionPlayerMapping[tacticalPosition] = playerId;
                }
            }

            if (positionPlayerMapping.Count != 11)
            {
                Debug.LogError("Formation does not have exactly 11 players.");
            }
        }

        public void ClearSelection()
        {
            // Step 1: Null all players in starting position mapping and add to reserves
            foreach (var position in controller.startingPositionPlayerMapping.Keys.ToList())
            {
                var player = controller.startingPositionPlayerMapping[position];
                if (player != null)
                {
                    controller.reservePlayersItems.Add(player);
                    controller.startingPositionPlayerMapping[position] = null; // Nullify the mapping
                }
            }

            // Step 2: Null all bench players but keep the list intact
            for (int i = 0; i < controller.substitutesPlayersItems.Count; i++)
            {
                var player = controller.substitutesPlayersItems[i];
                if (player != null)
                {
                    controller.reservePlayersItems.Add(player);
                }
                controller.substitutesPlayersItems[i] = null; // Nullify the item
            }

            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                // Step 3: Reinitialize the views
                controller.InitializeAndSetupBoard();
                controller.InitializeSubstitutePlayers();
                controller.InitializeReservePlayers();
            }
        }

        public void LogFormation(Dictionary<TacticalPositionOption, int?> formation)
        {
            var formationString = string.Empty;
            var idsString = string.Empty;

            foreach (var position in formation.Keys)
            {
                formationString += $"{position} ";
            }
            foreach (var id in formation.Values)
            {
                idsString += $"{id} ";
            }

            Debug.Log("Formation Tactical Positions: " + formationString);
            Debug.Log("Formation IDs at Positions: " + idsString);
        }
    }
}
