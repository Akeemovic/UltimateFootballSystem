﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine.Instructions.Team;
using UltimateFootballSystem.Core.TacticsEngine.Types;
using UltimateFootballSystem.Core.TacticsEngine.Utils;

namespace UltimateFootballSystem.Core.TacticsEngine
{
    /// <summary>
    /// Represents a complete tactic with formation, roles, and duties
    /// </summary>
    /// <summary>
    /// Represents a complete tactic with formation, roles, and duties
    /// </summary>
    public class Tactic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // public List<TacticalPosition> ActivePositions { get; private set; } = new List<TacticalPosition>();
        public List<TacticalPosition> ActivePositions { get; set; } = new List<TacticalPosition>();
        public List<int?> Substitutes { get; set; } = new List<int?>();// Players' Ids
        public List<int?> Reserves { get; set; } // Players' Ids

        public TeamInstructions TeamInstructions { get; set; }
        public InPossessionInstructions InPossessionInstructions;
        public InTransitionInstructions InTransitionInstructions;
        public OutOfPossessionInstructions OutOfPossessionInstructions;

        public Tactic()
        {
            Name = "New Tactic";
        }

        public Tactic(string name)
        {
            Name = name;
            TeamInstructions = new TeamInstructions();
            InPossessionInstructions = TeamInstructions.InPossession;
            InTransitionInstructions = TeamInstructions.InTransition;
            OutOfPossessionInstructions = TeamInstructions.OutOfPossession;
        }

        // Create a new tactic from a formation template
        public static Tactic SetFormation(TacticalPositionOption[] formation)
        {
            var tactic = new Tactic();

            // Create all tactical positions from the formation array
            foreach (var posOption in formation)
            {
                var posGroup = TacticalPositionUtils.GetGroupForPosition(posOption);
                var availableRoles = RoleManager.GetRolesForPosition(posOption)
                    .Select(r => RoleManager.GetRole(r))
                    .ToList();

                var position = new TacticalPosition(posGroup, posOption, availableRoles);
                tactic.ActivePositions.Add(position);
            }

            return tactic;
        }

        public void AssignPlayersToPosition(List<int?> playerIds)
        {
            for (int i = 0; i < ActivePositions.Count; i++)
            {
                if (i < playerIds.Count)
                {
                    int? playerId = playerIds[i]; // Remove .Value - work directly with nullable int

                    // Assign player to the position (can be null)
                    ActivePositions[i].AssignedPlayerId = playerId;

                    // Only remove from substitutes if playerId has a value
                    if (playerId.HasValue)
                    {
                        Substitutes.Remove(playerId);
                    }
                }
                else
                {
                    // Unassign if no player available
                    ActivePositions[i].AssignedPlayerId = null;
                }
            }
        }

        // Find position index by TacticalPositionOption
        public int FindPositionIndex(TacticalPositionOption positionOption)
        {
            for (int i = 0; i < ActivePositions.Count; i++)
            {
                if (ActivePositions[i].Position == positionOption)
                {
                    return i;
                }
            }
            return -1;
        }

        // Change formation while preserving player assignments where possible
        public void ChangeFormation(TacticalPositionOption[] newFormation, bool preservePlayerAssignments = true)
        {
            // Preserve player assignments by position type when possible
            var oldAssignments = new Dictionary<TacticalPositionOption, Player>();
            var oldRoles = new Dictionary<TacticalPositionOption, TacticalRoleOption>();
            var oldDuties = new Dictionary<TacticalPositionOption, TacticalDutyOption>();

            if (preservePlayerAssignments)
            {
                foreach (var pos in ActivePositions)
                {
                    if (pos.AssignedPlayer != null)
                    {
                        oldAssignments[pos.Position] = pos.AssignedPlayer;
                    }

                    if (pos.SelectedRole != null)
                    {
                        oldRoles[pos.Position] = pos.SelectedRole.RoleOption;
                        oldDuties[pos.Position] = pos.SelectedRole.SelectedDuty;
                    }
                }
            }

            // Create new positions from formation
            var newPositions = new List<TacticalPosition>();
            foreach (var posOption in newFormation)
            {
                var posGroup = TacticalPositionUtils.GetGroupForPosition(posOption);
                var roles = RoleManager.GetRolesForPosition(posOption)
                    .Select(r => RoleManager.GetRole(r))
                    .ToList();

                var position = new TacticalPosition(posGroup, posOption, roles);

                // Restore player if position matches
                if (oldAssignments.TryGetValue(posOption, out var player))
                {
                    position.AssignPlayer(player);
                }

                // Restore role and duty if position matches
                if (oldRoles.TryGetValue(posOption, out var roleType))
                {
                    position.SetRole(roleType);

                    if (oldDuties.TryGetValue(posOption, out var duty))
                    {
                        position.SetSelectedDuty(duty);
                    }
                }

                newPositions.Add(position);
            }

            // Replace positions
            ActivePositions = newPositions;
        }

        // Swap players between positions
        public void SwapPlayers(int index1, int index2)
        {
            if (index1 >= 0 && index1 < ActivePositions.Count &&
                index2 >= 0 && index2 < ActivePositions.Count)
            {
                var player1 = ActivePositions[index1].AssignedPlayer;
                var player2 = ActivePositions[index2].AssignedPlayer;

                ActivePositions[index1].AssignPlayer(player2);
                ActivePositions[index2].AssignPlayer(player1);
            }
        }

        // Serialization - Convert to simple data object
        public TacticData ToData()
        {
            var data = new TacticData
            {
                Id = Id,
                Name = Name
            };

            foreach (var pos in ActivePositions)
            {
                data.Positions.Add(new PositionData
                {
                    Position = pos.Position,
                    PlayerId = pos.AssignedPlayerId,
                    RoleType = pos.SelectedRole?.RoleOption ?? default,
                    Duty = pos.SelectedRole?.SelectedDuty ?? default
                });
            }

            data.Substitutes = Substitutes;
            data.Reserves = Reserves;
            
            return data;
        }

        // Deserialization - Create from data object
        public static Tactic FromData(TacticData data)
        {
            var tactic = new Tactic
            {
                Id = data.Id,
                Name = data.Name
            };

            foreach (var posData in data.Positions)
            {
                var posGroup = TacticalPositionUtils.GetGroupForPosition(posData.Position);
                var roles = RoleManager.GetRolesForPosition(posData.Position)
                    .Select(r => RoleManager.GetRole(r))
                    .ToList();

                var position = new TacticalPosition(posGroup, posData.Position, roles);

                // Set role and duty
                if (posData.RoleType != default)
                {
                    position.SetRole(posData.RoleType);
                    position.SetSelectedDuty(posData.Duty);
                }

                // Player assignment would happen later when players are loaded
                if (posData.PlayerId.HasValue)
                {
                    position.AssignedPlayerId = posData.PlayerId;
                }

                tactic.ActivePositions.Add(position);
            }

            return tactic;
        }

        // Serialize to JSON
        public string ToJson()
        {
            return JsonConvert.SerializeObject(ToData(), Formatting.Indented);
        }

        // Deserialize from JSON
        public static Tactic FromJson(string json)
        {
            var data = JsonConvert.DeserializeObject<TacticData>(json);
            return FromData(data);
        }

        // Get formation as a string (e.g. "4-4-2")
        public string FormationToString()
        {
            // Count positions by position group
            var defenseCnt = ActivePositions.Count(p => p.PositionGroup == TacticalPositionGroupOption.D_Center ||
                                                     p.PositionGroup == TacticalPositionGroupOption.D_Flank);

            var dmCnt = ActivePositions.Count(p => p.PositionGroup == TacticalPositionGroupOption.DM_Center ||
                                                p.PositionGroup == TacticalPositionGroupOption.DM_Flank);

            var midCnt = ActivePositions.Count(p => p.PositionGroup == TacticalPositionGroupOption.M_Center ||
                                                 p.PositionGroup == TacticalPositionGroupOption.M_Flank);

            var amCnt = ActivePositions.Count(p => p.PositionGroup == TacticalPositionGroupOption.AM_Center ||
                                               p.PositionGroup == TacticalPositionGroupOption.AM_Flank);

            var fwdCnt = ActivePositions.Count(p => p.PositionGroup == TacticalPositionGroupOption.ST_Center);

            // Build the formation string
            var formationParts = new List<string>();

            formationParts.Add(defenseCnt.ToString());

            if (dmCnt > 0) formationParts.Add(dmCnt.ToString());
            if (midCnt > 0) formationParts.Add(midCnt.ToString());
            if (amCnt > 0) formationParts.Add(amCnt.ToString());
            if (fwdCnt > 0) formationParts.Add(fwdCnt.ToString());

            return string.Join("-", formationParts);
        }
    }
}