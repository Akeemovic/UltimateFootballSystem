using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UltimateFootballSystem.Core.TacticsEngine.Types;

namespace UltimateFootballSystem.Core.TacticsEngine.Utils
{
    /// <summary>
    /// Manages saving and loading of tactics
    /// </summary>
    public class TacticManager
    {
        private readonly string _savePath;

        public TacticManager(string savePath)
        {
            _savePath = savePath;

            // Ensure directory exists
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        // Save tactic to file
        public void SaveTactic(Tactic tactic, string fileName)
        {
            var data = tactic.ToData();
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(Path.Combine(_savePath, fileName), json);
        }

        // Load tactic from file
        public Tactic LoadTactic(string fileName)
        {
            var path = Path.Combine(_savePath, fileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Tactic file not found: {fileName}");
            }

            var json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<TacticData>(json);
            return Tactic.FromData(data);
        }

        // Get list of saved tactics
        public List<string> GetSavedTactics()
        {
            return Directory.GetFiles(_savePath, "*.json")
                .Select(Path.GetFileName)
                .ToList();
        }

        // Delete a saved tactic
        public bool DeleteTactic(string fileName)
        {
            var path = Path.Combine(_savePath, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
