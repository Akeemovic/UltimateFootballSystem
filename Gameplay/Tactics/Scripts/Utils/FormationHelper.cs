using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.Utils
{
    public static class FormationHelper
    {
        // public static void SetFormationViews(PlayerItemView[] StartingPlayersViews, PositionZonesContainerView[] zoneContainerViews, TacticalPositionOption[] formationTacticalPositions)
        // {
        //     Debug.Log("SetFormationViews");
        //     foreach (var playerItemView in StartingPlayersViews)
        //     {
        //         if (playerItemView == null) continue;
        //
        //         var isInFormation = false;
        //
        //         foreach (var formationPosition in formationTacticalPositions)
        //         {
        //             if (playerItemView.ParentPositionZoneView.tacticalPositionOption == formationPosition)
        //             {
        //                 isInFormation = true;
        //                 break;
        //             }
        //         }
        //
        //         playerItemView.SetInUseForFormation(isInFormation, isCalledFromFormationInit: true);
        //     }
        //     // TacticsPitch.HideUnusedPlayerItemViews(zoneContainerViews);
        //     // TacticsPitch.HideUnusedPlayerItemViews(zoneContainerViews);
        // }
        //
    
        // public static void LoadPlayersToFormation(List<Player> players, PositionZonesContainerView[] zoneContainerViews)
        // {
        //     foreach (var player in players)
        //     {
        //         foreach (var zoneContainer in zoneContainerViews)
        //         {
        //             var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
        //             foreach (var zoneView in zoneViews)
        //             {
        //                 if (zoneView.tacticalPositionOption == player.TacticalPosition)
        //                 {
        //                     if (zoneView.InUseForFormation)
        //                     {
        //                         zoneView.childPlayerItemView.SetPlayerData(player);
        //                         break;
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }

        public static void CreateFormation(PositionZonesContainerView[] zoneContainerViews, out Dictionary<TacticalPositionOption, int?> positionPlayerMapping)
        {
            positionPlayerMapping = new Dictionary<TacticalPositionOption, int?>();

            foreach (var container in zoneContainerViews)
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
                        Debug.LogWarning($"Duplicate tactical position found: {tacticalPosition}");
                        continue;
                    }

                    positionPlayerMapping[tacticalPosition] = playerId;
                }
            }

            if (positionPlayerMapping.Count != 11)
            {
                Debug.LogError("Formation does not have exactly 11 players.");
            }
            else
            {
                SaveFormation(positionPlayerMapping);
            }
        }

        private static void SaveFormation(Dictionary<TacticalPositionOption, int?> positionPlayerMapping)
        {
            // string json = JsonConvert.SerializeObject(positionPlayerMapping);
            string json = JsonConvert.SerializeObject(positionPlayerMapping, Formatting.Indented);
            string path = Path.Combine(Application.persistentDataPath, "currentFormation.json");

            try
            {
                File.WriteAllText(path, json);
                Debug.Log("Formation saved successfully.");
            }
            catch (IOException ex)
            {
                Debug.LogError($"Failed to save formation: {ex.Message}");
            }
        }

        public static Dictionary<TacticalPositionOption, int?> LoadFormation()
        {
            string path = Path.Combine(Application.persistentDataPath, "currentFormation.json");
            if (!File.Exists(path))
            {
                Debug.LogError("Formation file not found.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                var positionPlayerMapping = JsonConvert.DeserializeObject<Dictionary<TacticalPositionOption, int?>>(json);
                Debug.Log("Formation loaded successfully.");
                return positionPlayerMapping;
            }
            catch (IOException ex)
            {
                Debug.LogError($"Failed to load formation: {ex.Message}");
                return null;
            }
        }
    
        public static void LogFormation(Dictionary<TacticalPositionOption, int?> formation)
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
