using UnityEngine;

namespace UltimateFootballSystem.Common.Scripts.Utils
{
    public static class NumToVisualHelper
    {
        // Method to get color based on the input value
        public static Color GetColorFromValue(int value)
        {
            if (value < 0 || value > 100)
            {
                Debug.LogError("Value out of range. Must be between 0 and 100.");
                return Color.black; // Default color in case of error
            }

            if (value <= 20)
                return new Color(0.56f, 0.16f, 0.20f); // Very Red (8e2834)
            else if (value <= 40)
                return new Color(0.79f, 0.27f, 0.27f); // Red (c94545)
            else if (value <= 65)
                // return new Color(0.90f, 0.73f, 0.17f); // Yellow (e5b92c)
                // return new Color(0.855f, 0.647f, 0.125f); // Goldenrod (#DAA520)
                return new Color(0.8f, 0.6f, 0f); // Rich golden yellow (#CC9900)
            // return new Color(0.804f, 0.608f, 0.114f); // Darker gold (#CD9B1D)
            // return new Color(0.722f, 0.525f, 0.043f); // Dark goldenrod (#B8860B)
            else if (value <= 80)
                return new Color(0.26f, 0.69f, 0.39f); // Green (43af64)
            else
                return new Color(0.11f, 0.45f, 0.25f); // Very Green (1d723f)
        }

        // Method to get color description based on the input value
        public static string GetColorDescriptionFromValue(int value)
        {
            if (value < 0 || value > 100)
            {
                return "Invalid value. Must be between 0 and 100.";
            }

            if (value <= 20)
                return "Very Red";
            else if (value <= 40)
                return "Red";
            else if (value <= 65)
                return "Yellow";
            else if (value <= 80)
                return "Green";
            else
                return "Very Green";
        }
    }
}