using System;
using UnityEditor;
using UnityEngine;

namespace UltimateFootballSystem.Editor.DatabaseManager.Windows
{
    /// <summary>
    /// Pop-up window for creating/editing roles - NOT IMPLEMENTED YET
    /// Use Inspector with OdinInspector for now
    /// </summary>
    public class RoleEditorWindow : EditorWindow
    {
        public static void ShowCreateMode(Action onSaveCallback = null)
        {
            EditorUtility.DisplayDialog(
                "Not Implemented",
                "Role Editor Window is not fully implemented yet.\n\n" +
                "Please use the Inspector to edit roles directly.\n" +
                "Select a role asset and configure it in the Inspector.",
                "OK");
        }

        public static void ShowEditMode(Core.Tactics.TacticalRoleDefinition role, Action onSaveCallback = null)
        {
            // Just select the role in the inspector
            Selection.activeObject = role;
            EditorGUIUtility.PingObject(role);
        }
    }
}
