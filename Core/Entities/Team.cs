using System.Collections.Generic;
using UltimateFootballSystem.Core.TacticsEngine;

namespace UltimateFootballSystem.Core.Entities
{
    /// <summary>
    /// Represents a team with players and tactics
    /// </summary>
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Tactic> Tactics { get; } = new List<Tactic>();
        public Tactic ActiveTactic { get; private set; }
        public List<int> Players { get; set; } = new List<int>();

        public Team(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        // Add a tactic to the team
        public void AddTactic(Tactic tactic)
        {
            Tactics.Add(tactic);

            // Set as active if it's the first tactic
            if (ActiveTactic == null)
            {
                ActiveTactic = tactic;
            }
        }

        // Set the active tactic
        public void SetActiveTactic(Tactic tactic)
        {
            if (Tactics.Contains(tactic))
            {
                ActiveTactic = tactic;
            }
        }
    }
}
