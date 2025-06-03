using System;
using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.Entities;

namespace UltimateFootballSystem.Core.Utils
{
    public class PlayerDataManager
    {
        private static List<Player> allPlayers;

        static PlayerDataManager()
        {
            InitializePlayers();
        }

        private static void InitializePlayers()
        {
            allPlayers = new List<Player>() 
            {
                new Player { Id = 1, Name = "Courtois", SquadNumber = "1", CurrentAbility = 4.5f },
                new Player { Id = 2, Name = "Marcelo", SquadNumber = "12", CurrentAbility = 4f },
                new Player { Id = 3, Name = "Christensen", SquadNumber = "15", CurrentAbility = 3.5f },
                new Player { Id = 4, Name = "Van Dijk", SquadNumber = "4", CurrentAbility = 4.5f },
                new Player { Id = 5, Name = "Hakimi", SquadNumber = "16", CurrentAbility = 4f },
                new Player { Id = 6, Name = "Neymar", SquadNumber = "11", CurrentAbility = 4.5f },
                new Player { Id = 7, Name = "De Bruyne", SquadNumber = "17", CurrentAbility = 4.5f },
                new Player { Id = 8, Name = "Kroos", SquadNumber = "8", CurrentAbility = 5f },
                new Player { Id = 9, Name = "Modric", SquadNumber = "15", CurrentAbility = 4.5f },
                new Player { Id = 10, Name = "Messi", SquadNumber = "10", CurrentAbility = 5f },
                new Player { Id = 11, Name = "Ronaldo", SquadNumber = "7", CurrentAbility = 5f },
                new Player { Id = 101, Name = "Neueur", SquadNumber = "21", CurrentAbility = 4.5f },
                new Player { Id = 110, Name = "Lewandowski", SquadNumber = "9", CurrentAbility = 5f },
                new Player { Id = 107, Name = "Dragowski", SquadNumber = "27", CurrentAbility = 5f },
                new Player { Id = 108, Name = "Rodri", SquadNumber = "6", CurrentAbility = 5f },
                new Player { Id = 201, Name = "Ramos", SquadNumber = "44", CurrentAbility = 3f },
                new Player { Id = 210, Name = "Vini Jr", SquadNumber = "77", CurrentAbility = 4.5f },
                new Player { Id = 207, Name = "Haaland", SquadNumber = "99", CurrentAbility = 4.5f },
                new Player { Id = 202, Name = "Smith", SquadNumber = "10", CurrentAbility = 3.5f },
                new Player { Id = 203, Name = "Johnson", SquadNumber = "8", CurrentAbility = 4f },
                new Player { Id = 204, Name = "Williams", SquadNumber = "5", CurrentAbility = 3f },
                new Player { Id = 205, Name = "Brown", SquadNumber = "3", CurrentAbility = 2.5f },
                new Player { Id = 206, Name = "Jones", SquadNumber = "2", CurrentAbility = 3.5f },
                new Player { Id = 208, Name = "Miller", SquadNumber = "4", CurrentAbility = 4f },
                new Player { Id = 209, Name = "Davis", SquadNumber = "7", CurrentAbility = 3.5f },
                new Player { Id = 211, Name = "Garcia", SquadNumber = "6", CurrentAbility = 3f },
                new Player { Id = 212, Name = "Martinez", SquadNumber = "11", CurrentAbility = 4.5f },
                new Player { Id = 213, Name = "Robinson", SquadNumber = "12", CurrentAbility = 3.5f },
                new Player { Id = 214, Name = "Clark", SquadNumber = "14", CurrentAbility = 3f },
                new Player { Id = 215, Name = "Rodriguez", SquadNumber = "9", CurrentAbility = 4f },
                new Player { Id = 216, Name = "Lewis", SquadNumber = "15", CurrentAbility = 2.5f },
                new Player { Id = 217, Name = "Lee", SquadNumber = "13", CurrentAbility = 3.5f },
                new Player { Id = 218, Name = "Walker", SquadNumber = "16", CurrentAbility = 4f },
                new Player { Id = 219, Name = "Hall", SquadNumber = "17", CurrentAbility = 3.5f },
                new Player { Id = 220, Name = "Allen", SquadNumber = "18", CurrentAbility = 3f },
                new Player { Id = 221, Name = "Young", SquadNumber = "19", CurrentAbility = 4.5f },
                new Player { Id = 222, Name = "Hernandez", SquadNumber = "20", CurrentAbility = 4f },
                new Player { Id = 223, Name = "King", SquadNumber = "21", CurrentAbility = 3.5f },
                new Player { Id = 224, Name = "Wright", SquadNumber = "22", CurrentAbility = 3f },
                new Player { Id = 225, Name = "Lopez", SquadNumber = "23", CurrentAbility = 2.5f }
            };
        
            Random rand = new Random();
        
            foreach (var player in allPlayers)
            {
                player.MatchFitness = rand.Next(10, 100);
                player.Morale = rand.Next(10, 100);
                player.Condition = rand.Next(10, 100);
            }
        }

    
    
        // Static methods to access player data
        public static List<Player> GetCurrentSquad(int teamId = 0)
        {
            return allPlayers;
        }
    
        public static Player GetPlayerById(int playerId)
        {
            return allPlayers.FirstOrDefault(p => p.Id == playerId);
        }
    }
}
