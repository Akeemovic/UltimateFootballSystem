using Sirenix.OdinInspector.Editor;
using UltimateFootballSystem.Core.Tactics;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual.OnPlayerHasBall;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual.OnTeamHasBall;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual.OnOppositionHasBall;
using UnityEditor;
using UnityEngine;

namespace UltimateFootballSystem.Editor.DatabaseManager.Inspectors
{
    [CustomEditor(typeof(TacticalRoleDefinition))]
    public class TacticalRoleDefinitionEditor : OdinEditor
    {
        private SerializableIndividualInstruction _instructionConfig;
        private int _instructionTabIndex = 0;
        private readonly string[] _instructionTabs = { "On Player Has Ball", "On Team Has Ball", "On Opposition Has Ball" };

        protected override void OnEnable()
        {
            base.OnEnable();

            var role = target as TacticalRoleDefinition;
            if (role != null)
            {
                _instructionConfig = role.GetInstructionConfiguration();
            }

            // Load tab preference
            _instructionTabIndex = EditorPrefs.GetInt("TacticalRole_InstructionTab", 0);
        }

        public override void OnInspectorGUI()
        {
            var role = target as TacticalRoleDefinition;
            if (role == null) return;

            EditorGUILayout.Space(10);

            // Draw default Odin inspector
            base.OnInspectorGUI();

            EditorGUILayout.Space(30);

            // Instructions Section
            DrawInstructionsSection(role);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(role);
            }
        }

        private void DrawInstructionsSection(TacticalRoleDefinition roleDefinition)
        {
            Sirenix.Utilities.Editor.SirenixEditorGUI.Title("Individual Instructions", "", TextAlignment.Left, true);

            EditorGUILayout.HelpBox(
                "Configure which instructions are Available (optional), Required (locked), or Unavailable (hidden).",
                MessageType.Info);

            EditorGUILayout.Space(10);

            // Tab selection
            var newTabIndex = GUILayout.Toolbar(_instructionTabIndex, _instructionTabs, GUILayout.Height(25));
            if (newTabIndex != _instructionTabIndex)
            {
                _instructionTabIndex = newTabIndex;
                EditorPrefs.SetInt("TacticalRole_InstructionTab", _instructionTabIndex);
            }

            EditorGUILayout.Space(10);

            // Draw content based on selected tab
            switch (_instructionTabIndex)
            {
                case 0: // On Player Has Ball
                    DrawOnPlayerHasBallInstructions();
                    break;
                case 1: // On Team Has Ball
                    DrawOnTeamHasBallInstructions();
                    break;
                case 2: // On Opposition Has Ball
                    DrawOnOppositionHasBallInstructions();
                    break;
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Save Instruction Configuration", GUILayout.Height(30)))
            {
                roleDefinition.SetInstructionConfiguration(_instructionConfig);
                EditorUtility.SetDirty(roleDefinition);
                AssetDatabase.SaveAssets();
                Debug.Log($"Saved instruction configuration for {roleDefinition.roleName}");
            }
        }

        private void DrawOnPlayerHasBallInstructions()
        {
            var config = _instructionConfig.onPlayerHasBall;

            // Quick action buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Make All Available", GUILayout.Height(22)))
            {
                config.holdUpBallAvailability = InstructionAvailability.Available;
                config.wingPlayAvailability = InstructionAvailability.Available;
                config.shootingFrequencyAvailability = InstructionAvailability.Available;
                config.dribblingFrequencyAvailability = InstructionAvailability.Available;
                config.crossingFrequencyAvailability = InstructionAvailability.Available;
                config.crossDistanceAvailability = InstructionAvailability.Available;
                config.crossTargetAvailability = InstructionAvailability.Available;
                config.passingStyleAvailability = InstructionAvailability.Available;
                config.creativePassingAvailability = InstructionAvailability.Available;
                GUI.changed = true;
            }
            if (GUILayout.Button("Make All Unavailable", GUILayout.Height(22)))
            {
                config.holdUpBallAvailability = InstructionAvailability.Unavailable;
                config.wingPlayAvailability = InstructionAvailability.Unavailable;
                config.shootingFrequencyAvailability = InstructionAvailability.Unavailable;
                config.dribblingFrequencyAvailability = InstructionAvailability.Unavailable;
                config.crossingFrequencyAvailability = InstructionAvailability.Unavailable;
                config.crossDistanceAvailability = InstructionAvailability.Unavailable;
                config.crossTargetAvailability = InstructionAvailability.Unavailable;
                config.passingStyleAvailability = InstructionAvailability.Unavailable;
                config.creativePassingAvailability = InstructionAvailability.Unavailable;
                GUI.changed = true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);

            DrawInstruction("Hold Up Ball", ref config.holdUpBallAvailability, ref config.holdUpBallDefault);
            DrawInstruction("Wing Play", ref config.wingPlayAvailability, ref config.wingPlayDefault);
            DrawInstruction("Shooting Frequency", ref config.shootingFrequencyAvailability, ref config.shootingFrequencyDefault);
            DrawInstruction("Dribbling Frequency", ref config.dribblingFrequencyAvailability, ref config.dribblingFrequencyDefault);
            DrawInstruction("Crossing Frequency", ref config.crossingFrequencyAvailability, ref config.crossingFrequencyDefault);
            DrawInstruction("Cross Distance", ref config.crossDistanceAvailability, ref config.crossDistanceDefault);
            DrawInstruction("Cross Target", ref config.crossTargetAvailability, ref config.crossTargetDefault);
            DrawInstruction("Passing Style", ref config.passingStyleAvailability, ref config.passingStyleDefault);
            DrawInstruction("Creative Passing", ref config.creativePassingAvailability, ref config.creativePassingDefault);
        }

        private void DrawOnTeamHasBallInstructions()
        {
            var config = _instructionConfig.onTeamHasBall;

            // Quick action buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Make All Available", GUILayout.Height(22)))
            {
                config.moreForwardRunsAvailability = InstructionAvailability.Available;
                config.openChannelRunsAvailability = InstructionAvailability.Available;
                config.mobilityAvailability = InstructionAvailability.Available;
                config.positioningWidthAvailability = InstructionAvailability.Available;
                GUI.changed = true;
            }
            if (GUILayout.Button("Make All Unavailable", GUILayout.Height(22)))
            {
                config.moreForwardRunsAvailability = InstructionAvailability.Unavailable;
                config.openChannelRunsAvailability = InstructionAvailability.Unavailable;
                config.mobilityAvailability = InstructionAvailability.Unavailable;
                config.positioningWidthAvailability = InstructionAvailability.Unavailable;
                GUI.changed = true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);

            DrawInstruction("More Forward Runs", ref config.moreForwardRunsAvailability, ref config.moreForwardRunsDefault);
            DrawInstruction("Open Channel Runs", ref config.openChannelRunsAvailability, ref config.openChannelRunsDefault);
            DrawInstruction("Mobility", ref config.mobilityAvailability, ref config.mobilityDefault);
            DrawInstruction("Positioning Width", ref config.positioningWidthAvailability, ref config.positioningWidthDefault);
        }

        private void DrawOnOppositionHasBallInstructions()
        {
            var config = _instructionConfig.onOppositionHasBall;

            // Quick action buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Make All Available", GUILayout.Height(22)))
            {
                config.pressingFrequencyAvailability = InstructionAvailability.Available;
                config.pressingStyleAvailability = InstructionAvailability.Available;
                config.tighterMarkingAvailability = InstructionAvailability.Available;
                config.tacklingStyleAvailability = InstructionAvailability.Available;
                GUI.changed = true;
            }
            if (GUILayout.Button("Make All Unavailable", GUILayout.Height(22)))
            {
                config.pressingFrequencyAvailability = InstructionAvailability.Unavailable;
                config.pressingStyleAvailability = InstructionAvailability.Unavailable;
                config.tighterMarkingAvailability = InstructionAvailability.Unavailable;
                config.tacklingStyleAvailability = InstructionAvailability.Unavailable;
                GUI.changed = true;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);

            DrawInstruction("Pressing Frequency", ref config.pressingFrequencyAvailability, ref config.pressingFrequencyDefault);
            DrawInstruction("Pressing Style", ref config.pressingStyleAvailability, ref config.pressingStyleDefault);
            DrawBoolInstruction("Tighter Marking", ref config.tighterMarkingAvailability, ref config.tighterMarkingDefault);
            DrawInstruction("Tackling Style", ref config.tacklingStyleAvailability, ref config.tacklingStyleDefault);
        }

        private void DrawInstruction<T>(string label, ref InstructionAvailability availability, ref T defaultValue) where T : System.Enum
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            // Color indicator
            var color = GetAvailabilityColor(availability);
            var colorRect = GUILayoutUtility.GetRect(4, EditorGUIUtility.singleLineHeight, GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(colorRect, color);

            GUILayout.Space(4);

            // Label (fixed width)
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Width(140));

            // Availability dropdown
            var newAvailability = (InstructionAvailability)EditorGUILayout.EnumPopup(availability, GUILayout.Width(100));
            if (newAvailability != availability)
            {
                availability = newAvailability;
                GUI.changed = true;
            }

            // Default value (only shown if Available or Required)
            if (availability != InstructionAvailability.Unavailable)
            {
                EditorGUILayout.LabelField("Default:", GUILayout.Width(50));
                var newDefault = (T)EditorGUILayout.EnumPopup(defaultValue);
                if (!newDefault.Equals(defaultValue))
                {
                    defaultValue = newDefault;
                    GUI.changed = true;
                }
            }
            else
            {
                // Add space so the layout is consistent
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        private void DrawBoolInstruction(string label, ref InstructionAvailability availability, ref bool defaultValue)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            // Color indicator
            var color = GetAvailabilityColor(availability);
            var colorRect = GUILayoutUtility.GetRect(4, EditorGUIUtility.singleLineHeight, GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(colorRect, color);

            GUILayout.Space(4);

            // Label (fixed width)
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Width(140));

            // Availability dropdown
            var newAvailability = (InstructionAvailability)EditorGUILayout.EnumPopup(availability, GUILayout.Width(100));
            if (newAvailability != availability)
            {
                availability = newAvailability;
                GUI.changed = true;
            }

            // Default value (only shown if Available or Required)
            if (availability != InstructionAvailability.Unavailable)
            {
                EditorGUILayout.LabelField("Default:", GUILayout.Width(50));
                var newDefault = EditorGUILayout.Toggle(defaultValue);
                if (newDefault != defaultValue)
                {
                    defaultValue = newDefault;
                    GUI.changed = true;
                }
            }
            else
            {
                // Add space so the layout is consistent
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        private Color GetAvailabilityColor(InstructionAvailability availability)
        {
            return availability switch
            {
                InstructionAvailability.Unavailable => new Color(0.5f, 0.5f, 0.5f),
                InstructionAvailability.Available => new Color(0.4f, 0.7f, 1f),
                InstructionAvailability.Required => new Color(1f, 0.6f, 0.2f),
                _ => Color.white
            };
        }
    }
}
