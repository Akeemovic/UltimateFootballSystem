using System;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.Tactics;
using UltimateFootballSystem.Core.Tactics.Utils;
using UltimateFootballSystem.Core.Utils;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Pure data model holding all tactics board state.
    /// No Unity dependencies - can be tested independently.
    /// </summary>
    public class TacticBoardModel
    {
        // Core data
        public Dictionary<TacticalPositionOption, Player?> StartingPositionPlayerMapping { get; private set; }
        public ObservableList<Player> SubstitutesPlayersItems { get; private set; }
        public ObservableList<Player> ReservePlayersItems { get; private set; }

        // Team and tactics
        public Team Team { get; private set; }
        public Tactic CurrentTactic { get; private set; }

        // Configuration
        public int AllowedSubstitutes { get; set; } = 9;
        public bool AutoSortSubstitutes { get; set; } = false;
        public bool AutoSortReserves { get; set; } = true;

        // Current formation positions
        public TacticalPositionOption[] CurrentFormation { get; private set; }

        // Events for state changes
        public event Action OnDataChanged;
        public event Action<int> OnSubstitutesCountChanged;
        public event Action<int> OnReservesCountChanged;

        public TacticBoardModel(Team team)
        {
            Team = team;
            CurrentTactic = team?.ActiveTactic ?? new Tactic();

            StartingPositionPlayerMapping = new Dictionary<TacticalPositionOption, Player?>();
            SubstitutesPlayersItems = new ObservableList<Player>();
            ReservePlayersItems = new ObservableList<Player>();

            // Subscribe to observable list changes
            SubstitutesPlayersItems.OnCollectionChange += () => OnSubstitutesCountChanged?.Invoke(SubstitutesPlayersItems.Count(p => p != null));
            ReservePlayersItems.OnCollectionChange += () => OnReservesCountChanged?.Invoke(ReservePlayersItems.Count(p => p != null));
        }

        #region Formation Management

        /// <summary>
        /// Set the formation, preserving existing player assignments with intelligent position mapping
        /// </summary>
        public void SetFormation(TacticalPositionOption[] newFormation)
        {
            CurrentFormation = newFormation;

            // Create a copy of existing mapping before clearing
            var oldMapping = new Dictionary<TacticalPositionOption, Player?>(StartingPositionPlayerMapping);

            // Clear old mapping
            StartingPositionPlayerMapping.Clear();

            // Initialize new mapping with nulls
            foreach (var position in newFormation)
            {
                StartingPositionPlayerMapping[position] = null;
            }

            // Try to preserve players by matching same positions first
            foreach (var kvp in oldMapping)
            {
                if (kvp.Value == null) continue;

                // If exact position exists in new formation, keep player there
                if (StartingPositionPlayerMapping.ContainsKey(kvp.Key))
                {
                    StartingPositionPlayerMapping[kvp.Key] = kvp.Value;
                }
            }

            // For players that couldn't be matched, assign to empty slots by positional group
            var unassignedPlayers = oldMapping
                .Where(kvp => kvp.Value != null && !StartingPositionPlayerMapping.ContainsValue(kvp.Value))
                .ToList();

            foreach (var kvp in unassignedPlayers)
            {
                var player = kvp.Value;
                var oldPosition = kvp.Key;
                var oldGroup = TacticalPositionUtils.GetGroupForPosition(oldPosition);

                // Find first empty position in same group
                var matchingPosition = newFormation
                    .FirstOrDefault(pos =>
                        StartingPositionPlayerMapping[pos] == null &&
                        TacticalPositionUtils.GetGroupForPosition(pos) == oldGroup);

                if (matchingPosition != default(TacticalPositionOption))
                {
                    StartingPositionPlayerMapping[matchingPosition] = player;
                }
                else
                {
                    // If no matching group, assign to any empty position
                    var emptyPosition = newFormation
                        .FirstOrDefault(pos => StartingPositionPlayerMapping[pos] == null);

                    if (emptyPosition != default(TacticalPositionOption))
                    {
                        StartingPositionPlayerMapping[emptyPosition] = player;
                    }
                    // If no empty positions, player goes to reserves (handled by caller)
                }
            }

            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Remap players to formation preserving assignments
        /// </summary>
        public void RemapPlayersToFormation(TacticalPositionOption[] newFormation, Dictionary<TacticalPositionOption, Player?> existingMapping)
        {
            CurrentFormation = newFormation;

            var existingPlayers = existingMapping.Values
                .Where(player => player != null)
                .ToList();

            StartingPositionPlayerMapping.Clear();

            for (int i = 0; i < newFormation.Length; i++)
            {
                var position = newFormation[i];
                var player = i < existingPlayers.Count ? existingPlayers[i] : null;
                StartingPositionPlayerMapping[position] = player;
            }

            OnDataChanged?.Invoke();
        }

        #endregion

        #region Player Management

        /// <summary>
        /// Swap two players in starting lineup by position
        /// </summary>
        public bool SwapStartingPlayers(TacticalPositionOption pos1, TacticalPositionOption pos2)
        {
            if (!StartingPositionPlayerMapping.ContainsKey(pos1) || !StartingPositionPlayerMapping.ContainsKey(pos2))
                return false;

            var player1 = StartingPositionPlayerMapping[pos1];
            var player2 = StartingPositionPlayerMapping[pos2];

            StartingPositionPlayerMapping[pos1] = player2;
            StartingPositionPlayerMapping[pos2] = player1;

            OnDataChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Set a player at a specific starting position
        /// </summary>
        public bool SetStartingPlayer(TacticalPositionOption position, Player player)
        {
            if (!StartingPositionPlayerMapping.ContainsKey(position))
                return false;

            StartingPositionPlayerMapping[position] = player;
            OnDataChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Add player to substitutes if space available
        /// </summary>
        public bool AddSubstitute(Player player)
        {
            if (SubstitutesPlayersItems.Count >= AllowedSubstitutes)
                return false;

            SubstitutesPlayersItems.Add(player);
            OnDataChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Remove substitute at index
        /// </summary>
        public bool RemoveSubstitute(int index)
        {
            if (index < 0 || index >= SubstitutesPlayersItems.Count)
                return false;

            var player = SubstitutesPlayersItems[index];
            SubstitutesPlayersItems.RemoveAt(index);

            // Move to reserves
            if (player != null && !ReservePlayersItems.Contains(player))
            {
                ReservePlayersItems.Add(player);
            }

            OnDataChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Swap substitute at index with another player
        /// </summary>
        public bool SetSubstituteAt(int index, Player player)
        {
            if (index < 0 || index >= AllowedSubstitutes)
                return false;

            // Ensure list has enough slots
            while (SubstitutesPlayersItems.Count <= index)
            {
                SubstitutesPlayersItems.Add(null);
            }

            var oldPlayer = SubstitutesPlayersItems[index];
            SubstitutesPlayersItems[index] = player;

            // Move old player to reserves if not null
            if (oldPlayer != null && !ReservePlayersItems.Contains(oldPlayer))
            {
                ReservePlayersItems.Add(oldPlayer);
            }

            OnDataChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Add player to reserves
        /// </summary>
        public void AddReserve(Player player)
        {
            if (!ReservePlayersItems.Contains(player))
            {
                ReservePlayersItems.Add(player);
                OnDataChanged?.Invoke();
            }
        }

        /// <summary>
        /// Remove reserve at index
        /// </summary>
        public bool RemoveReserve(int index)
        {
            if (index < 0 || index >= ReservePlayersItems.Count)
                return false;

            ReservePlayersItems.RemoveAt(index);
            OnDataChanged?.Invoke();
            return true;
        }

        #endregion

        #region Data Operations

        /// <summary>
        /// Initialize data from team
        /// </summary>
        public void InitializeFromTeam(PlayerDataManager playerDataManager)
        {
            if (Team == null || Team.ActiveTactic == null)
                return;

            StartingPositionPlayerMapping.Clear();
            SubstitutesPlayersItems.Clear();
            ReservePlayersItems.Clear();

            // Get squad - PlayerDataManager has static GetPlayerById method
            List<Player> currentSquad = Team.Players
                .Select(id => PlayerDataManager.GetPlayerById(id))
                .Where(player => player != null)
                .ToList();

            // Populate starting positions
            foreach (var position in Team.ActiveTactic.ActivePositions)
            {
                TacticalPositionOption posOption = position.Position;
                var playerId = position.AssignedPlayerId;
                var playerProfile = currentSquad.FirstOrDefault(p => p.Id == playerId);
                StartingPositionPlayerMapping[posOption] = playerProfile;
            }

            // Populate substitutes and reserves
            var benchPlayerIds = Team.ActiveTactic.Substitutes;
            int benchCount = 0;

            for (int i = 0; i < benchPlayerIds.Count; i++)
            {
                var playerId = benchPlayerIds[i];
                var playerProfile = currentSquad.FirstOrDefault(p => p.Id == playerId);

                if (playerProfile != null)
                {
                    if (benchCount < AllowedSubstitutes)
                    {
                        SubstitutesPlayersItems.Add(playerProfile);
                        benchCount++;
                    }
                    else
                    {
                        ReservePlayersItems.Add(playerProfile);
                    }
                }
            }

            // Add remaining players to reserves
            foreach (var player in currentSquad)
            {
                if (!StartingPositionPlayerMapping.ContainsValue(player) &&
                    !SubstitutesPlayersItems.Contains(player) &&
                    !ReservePlayersItems.Contains(player))
                {
                    ReservePlayersItems.Add(player);
                }
            }

            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Clear all starting positions
        /// </summary>
        public void ClearStartingLineup()
        {
            var playersToMove = StartingPositionPlayerMapping.Values
                .Where(p => p != null)
                .ToList();

            var keys = StartingPositionPlayerMapping.Keys.ToList();
            foreach (var key in keys)
            {
                StartingPositionPlayerMapping[key] = null;
            }

            // Move players to reserves
            foreach (var player in playersToMove)
            {
                if (!ReservePlayersItems.Contains(player))
                {
                    ReservePlayersItems.Add(player);
                }
            }

            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Clear all substitutes
        /// </summary>
        public void ClearSubstitutes()
        {
            var playersToMove = SubstitutesPlayersItems.Where(p => p != null).ToList();

            SubstitutesPlayersItems.Clear();
            for (int i = 0; i < AllowedSubstitutes; i++)
            {
                SubstitutesPlayersItems.Add(null);
            }

            // Move to reserves
            foreach (var player in playersToMove)
            {
                if (!ReservePlayersItems.Contains(player))
                {
                    ReservePlayersItems.Add(player);
                }
            }

            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Compact substitutes - remove nulls and push to end
        /// </summary>
        public void CompactSubstitutes()
        {
            UnityEngine.Debug.Log($"[MODEL COMPACT] Substitute count BEFORE: {SubstitutesPlayersItems.Count}");
            for (int i = 0; i < Math.Min(SubstitutesPlayersItems.Count, AllowedSubstitutes); i++)
            {
                var player = SubstitutesPlayersItems[i];
                if (player == null)
                {
                    UnityEngine.Debug.Log($"[MODEL COMPACT]   [{i}] = NULL REFERENCE");
                }
                else
                {
                    UnityEngine.Debug.Log($"[MODEL COMPACT]   [{i}] = Player EXISTS, Name: '{player.Name ?? "NULL STRING"}', ID: {player.Id}");
                }
            }

            // Filter out both null references AND players with invalid data
            var players = SubstitutesPlayersItems.Where(p => p != null && !string.IsNullOrEmpty(p.Name) && p.Id > 0).ToList();
            var nullCount = AllowedSubstitutes - players.Count;

            UnityEngine.Debug.Log($"[MODEL COMPACT] Valid players found: {players.Count}, nulls needed: {nullCount}");

            SubstitutesPlayersItems.Clear();
            foreach (var player in players)
            {
                SubstitutesPlayersItems.Add(player);
            }

            for (int i = 0; i < nullCount; i++)
            {
                SubstitutesPlayersItems.Add(null);
            }

            UnityEngine.Debug.Log($"[MODEL COMPACT] Substitute count AFTER: {SubstitutesPlayersItems.Count}");

            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Compact reserves - remove ALL null entries (reserves should never have nulls)
        /// </summary>
        public void CompactReserves()
        {
            UnityEngine.Debug.Log($"[MODEL COMPACT] Reserve count BEFORE: {ReservePlayersItems.Count}");
            for (int i = 0; i < Math.Min(5, ReservePlayersItems.Count); i++)
            {
                var player = ReservePlayersItems[i];
                if (player == null)
                {
                    UnityEngine.Debug.Log($"[MODEL COMPACT]   [{i}] = NULL REFERENCE");
                }
                else
                {
                    UnityEngine.Debug.Log($"[MODEL COMPACT]   [{i}] = Player EXISTS, Name: '{player.Name ?? "NULL STRING"}', ID: {player.Id}");
                }
            }

            // Filter out both null references AND players with invalid data
            var players = ReservePlayersItems.Where(p => p != null && !string.IsNullOrEmpty(p.Name) && p.Id > 0).ToList();
            UnityEngine.Debug.Log($"[MODEL COMPACT] Non-null players found: {players.Count}");

            ReservePlayersItems.Clear();
            foreach (var player in players)
            {
                ReservePlayersItems.Add(player);
            }

            UnityEngine.Debug.Log($"[MODEL COMPACT] Reserve count AFTER: {ReservePlayersItems.Count}");
            for (int i = 0; i < Math.Min(5, ReservePlayersItems.Count); i++)
            {
                UnityEngine.Debug.Log($"[MODEL COMPACT]   [{i}] = {ReservePlayersItems[i]?.Name ?? "NULL"}");
            }

            OnDataChanged?.Invoke();
        }

        #endregion

        #region Complex Swap Operations

        /// <summary>
        /// Perform a complex swap between two positions (starting, bench, or reserves)
        /// </summary>
        public void SwapPlayers(
            TacticalPositionOption? startingPos1, int? benchIndex1, int? reserveIndex1, Player player1,
            TacticalPositionOption? startingPos2, int? benchIndex2, int? reserveIndex2, Player player2)
        {
            // Set player1 to position2
            if (startingPos2.HasValue && StartingPositionPlayerMapping.ContainsKey(startingPos2.Value))
            {
                // Only update positions that are IN the formation
                StartingPositionPlayerMapping[startingPos2.Value] = player1;
            }
            else if (benchIndex2.HasValue && benchIndex2.Value >= 0)
            {
                SetSubstituteAt(benchIndex2.Value, player1);
            }
            else if (reserveIndex2.HasValue && reserveIndex2.Value >= 0)
            {
                // Reserves should NEVER have null entries
                if (player1 != null && reserveIndex2.Value < ReservePlayersItems.Count)
                {
                    ReservePlayersItems[reserveIndex2.Value] = player1;
                }
                // If player1 is null, don't add it - reserves doesn't allow nulls
            }

            // Set player2 to position1
            if (startingPos1.HasValue && StartingPositionPlayerMapping.ContainsKey(startingPos1.Value))
            {
                // Only update positions that are IN the formation
                StartingPositionPlayerMapping[startingPos1.Value] = player2;
            }
            else if (benchIndex1.HasValue && benchIndex1.Value >= 0)
            {
                // Special case: if moving from bench to empty position and player2 is null
                if (player2 == null && benchIndex1.Value < SubstitutesPlayersItems.Count)
                {
                    SubstitutesPlayersItems[benchIndex1.Value] = null;
                }
                else
                {
                    SetSubstituteAt(benchIndex1.Value, player2);
                }
            }
            else if (reserveIndex1.HasValue && reserveIndex1.Value >= 0)
            {
                // Special case: if moving from reserves to empty position and player2 is null
                if (player2 == null)
                {
                    if (reserveIndex1.Value < ReservePlayersItems.Count)
                    {
                        ReservePlayersItems.RemoveAt(reserveIndex1.Value);
                    }
                }
                else
                {
                    // Reserves should NEVER have null entries
                    if (player2 != null && reserveIndex1.Value < ReservePlayersItems.Count)
                    {
                        ReservePlayersItems[reserveIndex1.Value] = player2;
                    }
                    // If player2 is null, remove the entry at reserveIndex1
                    else if (player2 == null && reserveIndex1.Value < ReservePlayersItems.Count)
                    {
                        ReservePlayersItems.RemoveAt(reserveIndex1.Value);
                    }
                }
            }

            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Sync the model's formation with the active views after a formation change
        /// Removes positions not in formation, adds new positions
        /// </summary>
        public void SyncFormationFromViews(TacticalPositionOption[] activePositions, Dictionary<TacticalPositionOption, Player> positionPlayerMap)
        {
            // Rebuild the dictionary with only the active formation positions
            var newMapping = new Dictionary<TacticalPositionOption, Player>();

            foreach (var position in activePositions)
            {
                // Use the player from the provided map, or null if not found
                var player = positionPlayerMap.TryGetValue(position, out var p) ? p : null;
                newMapping[position] = player;
            }

            StartingPositionPlayerMapping.Clear();
            foreach (var kvp in newMapping)
            {
                StartingPositionPlayerMapping[kvp.Key] = kvp.Value;
            }

            OnDataChanged?.Invoke();
        }

        /// <summary>
        /// Get player at a specific location
        /// </summary>
        public Player GetPlayerAt(TacticalPositionOption? startingPos, int? benchIndex, int? reserveIndex)
        {
            if (startingPos.HasValue && StartingPositionPlayerMapping.ContainsKey(startingPos.Value))
            {
                return StartingPositionPlayerMapping[startingPos.Value];
            }
            else if (benchIndex.HasValue && benchIndex.Value >= 0 && benchIndex.Value < SubstitutesPlayersItems.Count)
            {
                return SubstitutesPlayersItems[benchIndex.Value];
            }
            else if (reserveIndex.HasValue && reserveIndex.Value >= 0 && reserveIndex.Value < ReservePlayersItems.Count)
            {
                return ReservePlayersItems[reserveIndex.Value];
            }

            return null;
        }

        #endregion
    }
}
