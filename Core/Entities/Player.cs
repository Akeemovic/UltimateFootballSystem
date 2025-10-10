using System.Collections.Generic;
using JetBrains.Annotations;
using UltimateFootballSystem.Core.Stats;
using UltimateFootballSystem.Core.Tactics;
using System.Linq; // Added for LINQ operations
using System.Text; // Added for StringBuilder

namespace UltimateFootballSystem.Core.Entities
{
    /// <summary>
    /// Base class for any player in the system
    /// </summary>
    public class Player
    {
        // Define a constant for the learned position threshold.
        // Assuming a 1-100 rating scale for positions.
        public const int LearnedPositionTypeThreshold = 60; // Example: A player needs at least 60 rating to "learn" a position.

        public int Id { get; set; }
        public string Name { get; set; }
        
        /// <summary>
        /// The squad number assigned to the player.
        /// </summary>
        public string SquadNumber;

        // Abilities
        /// <summary>
        /// The potential ability rating of the player.
        /// </summary>
        public float PotentialAbility = 0;
    
        /// <summary>
        /// The current ability rating of the player.
        /// </summary>
        public float CurrentAbility = 0;

        // Physical and Mental Condition
        /// <summary>
        /// The player's current physical condition rating.
        /// </summary>
        public int Condition;
    
        /// <summary>
        /// The player's morale rating, indicating their current emotional state.
        /// </summary>
        public int Morale;
    
        /// <summary>
        /// The player's match fitness level.
        /// </summary>
        public int MatchFitness;
    
        /// <summary>
        /// Indicates whether the player is currently injured.
        /// </summary>
        public bool IsInjured;
        
        // Using the renamed enum for clarity and consistency
        public Dictionary<TacticalPositionTypeOption, int> PositionTypeRatings { get; set; } = new Dictionary<TacticalPositionTypeOption, int>();
        public Dictionary<TacticalRoleOption, int> RoleRatings { get; set; } = new Dictionary<TacticalRoleOption, int>();

        // Additional player attributes would go here in a real implementation
        // Examples: Pace, Shooting, Passing, etc.

        public Player() {}
        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets the rating for a specific tactical position.
        /// </summary>
        /// <param name="position">The tactical position to get the rating for.</param>
        /// <returns>The rating for the position, or 0 if not found.</returns>
        public int GetPositionRating(TacticalPositionTypeOption position)
        {
            return PositionTypeRatings.TryGetValue(position, out int rating) ? rating : 0;
        }

        /// <summary>
        /// Gets the rating for a specific tactical role type.
        /// </summary>
        /// <param name="roleType">The tactical role type to get the rating for.</param>
        /// <returns>The rating for the role, or 0 if not found.</returns>
        public int GetRoleRating(TacticalRoleOption roleType)
        {
            return RoleRatings.TryGetValue(roleType, out int rating) ? rating : 0;
        }
        
        /// <summary>
        /// Gets a list of tactical positions where the player's rating meets or exceeds the LEARNED_POSITION_TYPE_THRESHOLD.
        /// </summary>
        /// <returns>A list of learned tactical positions.</returns>
        public List<TacticalPositionTypeOption> GetLearnedPositionTypes()
        {
            return PositionTypeRatings
                .Where(kvp => kvp.Value >= LearnedPositionTypeThreshold)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        /// <summary>
        /// Gets a comma-separated string of "learned" tactical positions, grouped by type and side.
        /// Example: 'GK, D(LC), M(RLC), AMC, ST'
        /// </summary>
        /// <returns>A string representing the learned positions.</returns>
        public string GetLearnedPositionsTypeString()
        {
            var learnedPositionTypes = GetLearnedPositionTypes();
            if (!learnedPositionTypes.Any())
            {
                return "No learned positions";
            }

            var positionTypesGroups = new Dictionary<string, SortedSet<char>>(); // Key: Position Type (e.g., "D", "WB", "M", "AM"), Value: SortedSet of sides (L, C, R, K for GK, S for ST)

            foreach (var pos in learnedPositionTypes)
            {
                switch (pos)
                {
                    case TacticalPositionTypeOption.GK:
                        // GK is unique, doesn't combine with others
                        if (!positionTypesGroups.ContainsKey("GK"))
                        {
                            positionTypesGroups["GK"] = new SortedSet<char>();
                        }
                        positionTypesGroups["GK"].Add('K'); // Using 'K' as a placeholder for GK
                        break;
                    case TacticalPositionTypeOption.DL:
                    case TacticalPositionTypeOption.DC:
                    case TacticalPositionTypeOption.DR:
                        AddPositionTypeToGroup(positionTypesGroups, "D", pos);
                        break;
                    case TacticalPositionTypeOption.WBL:
                    case TacticalPositionTypeOption.WBR:
                        AddPositionTypeToGroup(positionTypesGroups, "WB", pos);
                        break;
                    case TacticalPositionTypeOption.DMC:
                        // DMC is a unique position in defensive midfield, often treated separately or as M(C) depending on depth
                        // For now, let's treat DMC as a standalone 'DM' or 'DMC'
                        // If we want to group with M, then we'd need to adjust
                        // Let's make it 'DM' for now.
                        if (!positionTypesGroups.ContainsKey("DM"))
                        {
                            positionTypesGroups["DM"] = new SortedSet<char>();
                        }
                        positionTypesGroups["DM"].Add('C'); // Central defensive midfielder
                        break;
                    case TacticalPositionTypeOption.ML:
                    case TacticalPositionTypeOption.MC:
                    case TacticalPositionTypeOption.MR:
                        AddPositionTypeToGroup(positionTypesGroups, "M", pos);
                        break;
                    case TacticalPositionTypeOption.AML:
                    case TacticalPositionTypeOption.AMC:
                    case TacticalPositionTypeOption.AMR:
                        AddPositionTypeToGroup(positionTypesGroups, "AM", pos);
                        break;
                    case TacticalPositionTypeOption.ST:
                        // ST is unique, doesn't combine with others in the same way
                         if (!positionTypesGroups.ContainsKey("ST"))
                        {
                            positionTypesGroups["ST"] = new SortedSet<char>();
                        }
                        positionTypesGroups["ST"].Add('S'); // Using 'S' as a placeholder for ST
                        break;
                    default:
                        // Fallback for any unhandled positions, just add as is
                        if (!positionTypesGroups.ContainsKey(pos.ToString()))
                        {
                            positionTypesGroups[pos.ToString()] = new SortedSet<char>();
                        }
                        break;
                }
            }

            // Build the final string
            var resultParts = new List<string>();

            // Ensure a consistent order for output (e.g., GK, D, WB, DM, M, AM, ST)
            var orderedKeys = positionTypesGroups.Keys.OrderBy(key => GetPositionTypeGroupOrder(key)).ToList();

            foreach (var key in orderedKeys)
            {
                var sides = positionTypesGroups[key];
                if (key == "GK")
                {
                    resultParts.Add("GK");
                }
                else if (key == "ST")
                {
                    resultParts.Add("ST");
                }
                else if (key == "DM")
                {
                    resultParts.Add("DMC"); // Explicitly show DMC as it's a specific role rather than a broad type L/R
                }
                else if (sides.Count > 1) // If more than one side for this position type
                {
                    resultParts.Add($"{key}({string.Join("", sides)})");
                }
                else if (sides.Count == 1) // If only one side
                {
                    resultParts.Add($"{key}{sides.First()}");
                }
                else // Should not happen with current logic but good for robustness
                {
                    resultParts.Add(key);
                }
            }

            return string.Join(", ", resultParts);
        }

        /// <summary>
        /// Helper to add a position and its side to the correct group.
        /// </summary>
        private void AddPositionTypeToGroup(Dictionary<string, SortedSet<char>> groups, string groupName, TacticalPositionTypeOption position)
        {
            if (!groups.ContainsKey(groupName))
            {
                groups[groupName] = new SortedSet<char>();
            }

            char side = ' ';
            switch (position)
            {
                case TacticalPositionTypeOption.DL:
                case TacticalPositionTypeOption.WBL:
                case TacticalPositionTypeOption.ML:
                case TacticalPositionTypeOption.AML:
                    side = 'L';
                    break;
                case TacticalPositionTypeOption.DC:
                case TacticalPositionTypeOption.MC:
                case TacticalPositionTypeOption.AMC:
                    side = 'C';
                    break;
                case TacticalPositionTypeOption.DR:
                case TacticalPositionTypeOption.WBR:
                case TacticalPositionTypeOption.MR:
                case TacticalPositionTypeOption.AMR:
                    side = 'R';
                    break;
            }
            if (side != ' ')
            {
                groups[groupName].Add(side);
            }
        }

        /// <summary>
        /// Helper to define a consistent order for position groups in the output string.
        /// </summary>
        private int GetPositionTypeGroupOrder(string groupKey)
        {
            switch (groupKey)
            {
                case "GK": return 1;
                case "D": return 2;
                case "WB": return 3;
                case "DM": return 4; // DMC
                case "M": return 5;
                case "AM": return 6;
                case "ST": return 7;
                default: return 99; // For any unhandled keys
            }
        }

        /// <summary>
        /// Determines if the player has "learned" a particular tactical position.
        /// </summary>
        /// <param name="position">The tactical position to check.</param>
        /// <returns>True if the player's rating for the position meets the threshold, false otherwise.</returns>
        public bool IsPositionTypeLearned(TacticalPositionTypeOption position)
        {
            return GetPositionRating(position) >= LearnedPositionTypeThreshold;
        }

        [CanBeNull] public PlayerMatchStats MatchStats = null;
    
        /// <summary>
        /// A list of the player's performance stats for each season.
        /// </summary>
        [CanBeNull] public PlayerSeasonStats SeasonStats = null;
        [CanBeNull] public List<PlayerSeasonStats> HistoryStats = null;
    }
}