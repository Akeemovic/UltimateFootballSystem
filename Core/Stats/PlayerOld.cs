using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UltimateFootballSystem.Core.Stats
{
    /// <summary>
    /// Represents the profile of a player, including personal details, abilities, and stats.
    /// </summary>
    [Serializable]
    public class PlayerOld
    {
        // Personal Information
        /// <summary>
        /// The unique identifier for the player.
        /// </summary>
        [SerializeField] public int Id = 0;
    
        /// <summary>
        /// The full name of the player.
        /// </summary>
        [SerializeField] public string Name;
    
        /// <summary>
        /// The first name of the player.
        /// </summary>
        [SerializeField] public string FirstName;
    
        /// <summary>
        /// The last name of the player.
        /// </summary>
        [SerializeField] public string LastName;
    
        /// <summary>
        /// The age of the player.
        /// </summary>
        [SerializeField] public int Age;
    
        /// <summary>
        /// The squad number assigned to the player.
        /// </summary>
        [SerializeField] public string SquadNumber;

        // Abilities
        /// <summary>
        /// The potential ability rating of the player.
        /// </summary>
        [SerializeField] public float PotentialAbility = 0;
    
        /// <summary>
        /// The current ability rating of the player.
        /// </summary>
        [SerializeField] public float CurrentAbility = 0;

        // Physical and Mental Condition
        /// <summary>
        /// The player's current physical condition rating.
        /// </summary>
        [SerializeField] public int Condition;
    
        /// <summary>
        /// The player's morale rating, indicating their current emotional state.
        /// </summary>
        [SerializeField] public int Morale;
    
        /// <summary>
        /// The player's match fitness level.
        /// </summary>
        [SerializeField] public int MatchFitness;
    
        /// <summary>
        /// Indicates whether the player is currently injured.
        /// </summary>
        [SerializeField] public bool IsInjured;
    
        /// <summary>
        /// Indicates whether the player is available for the next match.
        /// </summary>
        [SerializeField] public bool IsAvailableForMatch;

        // Financial Information
        /// <summary>
        /// The market value of the player, expressed in currency.
        /// </summary>
        [SerializeField] public float Value;

        // Contract and Career Information
        /// <summary>
        /// The player's current contract details.
        /// </summary>
        public PlayerContract Contract;

        /// <summary>
        /// The player's preferred foot (left, right, or both).
        /// </summary>
        public string PreferredFoot;

        /// <summary>
        /// The player's rating for using their weak foot.
        /// </summary>
        public float WeakFootRating;

        /// <summary>
        /// A list of nationalities the player holds.
        /// </summary>
        public List<string> Nationalities;

        /// <summary>
        /// The country for which the player is registered to play.
        /// </summary>
        public string NationRegisteredFor;

        // Attributes
        // Goalkeeping Attributes
        /// <summary>
        /// The player's ability to command the area in front of the goal.
        /// </summary>
        public float CommandOfArea;
    
        /// <summary>
        /// The player's ability to handle the ball as a goalkeeper.
        /// </summary>
        public float Handling;

        // Outfield Attributes
        /// <summary>
        /// The player's marking ability on the field (also for non-goalkeepers).
        /// </summary>
        public float Marking;

        /// <summary>
        /// The player's ability to tackle opponents.
        /// </summary>
        public float Tackling;
    
        /// <summary>
        /// The player's ability to finish scoring chances.
        /// </summary>
        public float Finishing;

        [CanBeNull] public PlayerMatchStats MatchStats = null;
    
        /// <summary>
        /// A list of the player's performance stats for each season.
        /// </summary>
        [CanBeNull] public PlayerSeasonStats SeasonStats = null;
        [CanBeNull] public List<PlayerSeasonStats> HistoryStats = null;
    }
}
