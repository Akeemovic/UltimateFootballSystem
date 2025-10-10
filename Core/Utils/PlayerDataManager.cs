using System;
using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.Tactics;

namespace UltimateFootballSystem.Core.Utils
{
    public class PlayerDataManager
    {
        private static List<Player> allPlayers;
        private static Random rand = new Random(); // Keep a single Random instance

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

            foreach (var player in allPlayers)
            {
                player.MatchFitness = rand.Next(10, 100);
                player.Morale = rand.Next(10, 100);
                player.Condition = rand.Next(10, 100);

                // Programmatically generate position type ratings
                GeneratePositionTypeRatings(player);
            }
        }

        /// <summary>
        /// Generates realistic-ish random position ratings for a player based on their likely primary role.
        /// Accounts for multi-positional players.
        /// </summary>
        /// <param name="player">The player object to update.</param>
        private static void GeneratePositionTypeRatings(Player player)
        {
            // Clear existing ratings first if any
            player.PositionTypeRatings.Clear();

            int baseRating = (int)(player.CurrentAbility * 20) + rand.Next(50, 70); // Scale CurrentAbility (1-5) to 1-100 and add a base
            if (baseRating > 100) baseRating = 100; // Cap at 100

            // Specific logic for multi-positional star players
            switch (player.Name)
            {
                case "Neymar":
                    // AML (natural), ST, AMC (accomplished)
                    player.PositionTypeRatings[TacticalPositionTypeOption.AML] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.ST] = Math.Min(100, baseRating + rand.Next(0, 15));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMC] = Math.Min(100, baseRating + rand.Next(0, 10));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMR] = Math.Min(100, baseRating + rand.Next(-10, 5)); // Can play, but not as naturally
                    break;
                case "Messi":
                    // AMR (natural), ST, AMC (accomplished)
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMR] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.ST] = Math.Min(100, baseRating + rand.Next(0, 15));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMC] = Math.Min(100, baseRating + rand.Next(0, 10));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AML] = Math.Min(100, baseRating + rand.Next(-10, 5)); // Can play, but not as naturally
                    break;
                case "Rodri":
                    // DMC (natural), MC (accomplished)
                    player.PositionTypeRatings[TacticalPositionTypeOption.DMC] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.MC] = Math.Min(100, baseRating + rand.Next(5, 15));
                    // Some basic defensive cover ability
                    player.PositionTypeRatings[TacticalPositionTypeOption.DC] = Math.Min(100, baseRating + rand.Next(-20, -5)); // Competent, not natural
                    break;
                case "Kroos":
                    // MC (natural), DMC (accomplished), AML/AMR (can play)
                    player.PositionTypeRatings[TacticalPositionTypeOption.MC] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.DMC] = Math.Min(100, baseRating + rand.Next(0, 10));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AML] = Math.Min(100, baseRating + rand.Next(-10, 5));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMR] = Math.Min(100, baseRating + rand.Next(-10, 5));
                    break;
                case "Marcelo":
                    // DL (natural), WBL (accomplished), ML (can play)
                    player.PositionTypeRatings[TacticalPositionTypeOption.DL] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.WBL] = Math.Min(100, baseRating + rand.Next(0, 10));
                    player.PositionTypeRatings[TacticalPositionTypeOption.ML] = Math.Min(100, baseRating + rand.Next(-10, 5));
                    break;
                case "Hakimi":
                    // DR (natural), WBR (accomplished), MR (can play)
                    player.PositionTypeRatings[TacticalPositionTypeOption.DR] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.WBR] = Math.Min(100, baseRating + rand.Next(0, 10));
                    player.PositionTypeRatings[TacticalPositionTypeOption.MR] = Math.Min(100, baseRating + rand.Next(-10, 5));
                    break;
                case "Ronaldo":
                    // ST (natural), AML (accomplished), AMR (accomplished)
                    player.PositionTypeRatings[TacticalPositionTypeOption.ST] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AML] = Math.Min(100, baseRating + rand.Next(5, 15));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMR] = Math.Min(100, baseRating + rand.Next(5, 15));
                    break;

                // Existing logic for other players (Goalies, Defenders, etc.)
                case "Courtois":
                case "Neueur":
                case "Dragowski":
                    player.PositionTypeRatings[TacticalPositionTypeOption.GK] = Math.Min(100, baseRating + rand.Next(10, 20));
                    break;
                case "Van Dijk":
                case "Ramos":
                case "Christensen":
                    player.PositionTypeRatings[TacticalPositionTypeOption.DC] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.DL] = Math.Min(100, baseRating + rand.Next(-15, 5)); // Competent, not natural
                    player.PositionTypeRatings[TacticalPositionTypeOption.DR] = Math.Min(100, baseRating + rand.Next(-15, 5)); // Competent, not natural
                    break;
                case "De Bruyne":
                case "Modric":
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMC] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.MC] = Math.Min(100, baseRating + rand.Next(5, 15));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AML] = Math.Min(100, baseRating + rand.Next(-10, 5));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMR] = Math.Min(100, baseRating + rand.Next(-10, 5));
                    break;
                case "Lewandowski":
                case "Haaland":
                    player.PositionTypeRatings[TacticalPositionTypeOption.ST] = Math.Min(100, baseRating + rand.Next(10, 20));
                    player.PositionTypeRatings[TacticalPositionTypeOption.AMC] = Math.Min(100, baseRating + rand.Next(-15, 5)); // Can play deeper, but less naturally
                    break;

                default:
                    // Generic players: Assign ratings to a few random positions, ensuring at least one is "learned"
                    var allPositions = Enum.GetValues(typeof(TacticalPositionTypeOption)).Cast<TacticalPositionTypeOption>().ToList();
                    List<TacticalPositionTypeOption> assignedPositions = new List<TacticalPositionTypeOption>();

                    // Ensure at least one position is "learned" for all generic players
                    TacticalPositionTypeOption primaryGenericPos = allPositions[rand.Next(allPositions.Count)];
                    player.PositionTypeRatings[primaryGenericPos] = Math.Min(100, baseRating + rand.Next(0, 20));
                    assignedPositions.Add(primaryGenericPos);

                    // Assign 1-2 more additional positions with varying proficiency
                    int additionalPositions = rand.Next(0, 3); // 0, 1, or 2 additional
                    for (int i = 0; i < additionalPositions; i++)
                    {
                        TacticalPositionTypeOption randomPos;
                        do
                        {
                            randomPos = allPositions[rand.Next(allPositions.Count)];
                        } while (assignedPositions.Contains(randomPos)); // Avoid duplicates

                        player.PositionTypeRatings[randomPos] = Math.Min(100, baseRating + rand.Next(-40, 10)); // Can be lower or slightly higher
                        assignedPositions.Add(randomPos);
                    }
                    break;
            }

            // Fill in remaining positions with lower, random ratings to simulate basic competency/unfamiliarity
            foreach (TacticalPositionTypeOption pos in Enum.GetValues(typeof(TacticalPositionTypeOption)))
            {
                if (!player.PositionTypeRatings.ContainsKey(pos))
                {
                    player.PositionTypeRatings[pos] = rand.Next(1, 50); // Lower ratings for unfamiliar positions
                }
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