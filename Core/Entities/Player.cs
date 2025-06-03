using System.Collections.Generic;
using JetBrains.Annotations;
using UltimateFootballSystem.Core.Stats;
using UltimateFootballSystem.Core.TacticsEngine;

namespace UltimateFootballSystem.Core.Entities
{
    /// <summary>
    /// Base class for any player in the system
    /// </summary>
    public class Player
    {
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
        
        public Dictionary<TacticalPositionOption, int> PositionRatings { get; set; } = new Dictionary<TacticalPositionOption, int>();
        public Dictionary<TacticalRoleOption, int> RoleRatings { get; set; } = new Dictionary<TacticalRoleOption, int>();

        // Additional player attributes would go here in a real implementation
        // Examples: Pace, Shooting, Passing, etc.

        public Player() {}
        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int GetPositionRating(TacticalPositionOption position)
        {
            return PositionRatings.TryGetValue(position, out int rating) ? rating : 0;
        }

        public int GetRoleRating(TacticalRoleOption roleType)
        {
            return RoleRatings.TryGetValue(roleType, out int rating) ? rating : 0;
        }
        
        

        [CanBeNull] public PlayerMatchStats MatchStats = null;
    
        /// <summary>
        /// A list of the player's performance stats for each season.
        /// </summary>
        [CanBeNull] public PlayerSeasonStats SeasonStats = null;
        [CanBeNull] public List<PlayerSeasonStats> HistoryStats = null;
    }
}
