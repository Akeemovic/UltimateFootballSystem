using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UltimateFootballSystem.Core;
using UltimateFootballSystem.Core.Tactics;
using UltimateFootballSystem.Editor.DatabaseManager.Windows;

namespace UltimateFootballSystem.Editor.DatabaseManager
{
    public class DatabaseEditorWindow : EditorWindow
    {
        // Menu items for direct access
        [MenuItem("Tools/Ultimate Football System/Database Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<DatabaseEditorWindow>("Database Manager");
            window.minSize = new Vector2(800, 600);
        }

        [MenuItem("Tools/Ultimate Football System/Database Manager/Welcome")]
        public static void ShowWelcomeTab() => ShowTabDirect(0);

        [MenuItem("Tools/Ultimate Football System/Database Manager/Setup")]
        public static void ShowSetupTab() => ShowTabDirect(1);

        [MenuItem("Tools/Ultimate Football System/Database Manager/Definitions")]
        public static void ShowDefinitionsTab() => ShowTabDirect(2);

        [MenuItem("Tools/Ultimate Football System/Database Manager/Tools")]
        public static void ShowToolsTab() => ShowTabDirect(3);

        private static void ShowTabDirect(int tabIndex)
        {
            var window = GetWindow<DatabaseEditorWindow>("Database Manager");
            window.minSize = new Vector2(800, 600);
            window.SwitchToTab(tabIndex);
        }

        private FootballDatabase _database;
        private VisualElement _root;

        // Tab references
        private Button[] _tabButtons;
        private VisualElement[] _tabContents;

        // Definition sub-tab references
        private Button[] _defSubTabButtons;
        private VisualElement[] _defSubTabContents;

        // Tools sub-tab references
        private Button[] _toolsSubTabButtons;
        private VisualElement[] _toolsSubTabContents;

        private void CreateGUI()
        {
            _root = rootVisualElement;

            // Load UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UltimateFootballSystem/Editor/DatabaseManager/UI/DatabaseManager.uxml");

            if (visualTree == null)
            {
                Debug.LogError("Could not load DatabaseManager.uxml! Make sure the path is correct.");
                var errorLabel = new Label("ERROR: Could not load UI layout file!");
                errorLabel.style.color = Color.red;
                errorLabel.style.fontSize = 16;
                errorLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                errorLabel.style.paddingTop = 50;
                _root.Add(errorLabel);
                return;
            }

            visualTree.CloneTree(_root);

            // Initialize UI
            InitializeTabButtons();
            InitializeSubTabs();
            InitializeButtons();
            InitializeDatabaseField();

            // Show welcome tab by default
            SwitchToTab(0);
        }

        #region Initialization

        private void InitializeTabButtons()
        {
            _tabButtons = new Button[4];
            _tabContents = new VisualElement[4];

            // Get tab buttons
            _tabButtons[0] = _root.Q<Button>("tab-welcome");
            _tabButtons[1] = _root.Q<Button>("tab-setup");
            _tabButtons[2] = _root.Q<Button>("tab-definitions");
            _tabButtons[3] = _root.Q<Button>("tab-tools");

            // Get tab contents
            _tabContents[0] = _root.Q<VisualElement>("content-welcome");
            _tabContents[1] = _root.Q<VisualElement>("content-setup");
            _tabContents[2] = _root.Q<VisualElement>("content-definitions");
            _tabContents[3] = _root.Q<VisualElement>("content-tools");

            // Register click events
            for (int i = 0; i < _tabButtons.Length; i++)
            {
                int index = i; // Capture for lambda
                _tabButtons[i].clicked += () => SwitchToTab(index);
            }
        }

        private void InitializeSubTabs()
        {
            // Definitions sub-tabs
            _defSubTabButtons = new Button[2];
            _defSubTabContents = new VisualElement[2];

            _defSubTabButtons[0] = _root.Q<Button>("subtab-roles");
            _defSubTabButtons[1] = _root.Q<Button>("subtab-formations");

            _defSubTabContents[0] = _root.Q<VisualElement>("subcontent-roles");
            _defSubTabContents[1] = _root.Q<VisualElement>("subcontent-formations");

            for (int i = 0; i < _defSubTabButtons.Length; i++)
            {
                int index = i;
                _defSubTabButtons[i].clicked += () => SwitchDefinitionsSubTab(index);
            }

            // Tools sub-tabs
            _toolsSubTabButtons = new Button[3];
            _toolsSubTabContents = new VisualElement[3];

            _toolsSubTabButtons[0] = _root.Q<Button>("subtab-test");
            _toolsSubTabButtons[1] = _root.Q<Button>("subtab-validation");
            _toolsSubTabButtons[2] = _root.Q<Button>("subtab-importexport");

            _toolsSubTabContents[0] = _root.Q<VisualElement>("subcontent-test");
            _toolsSubTabContents[1] = _root.Q<VisualElement>("subcontent-validation");
            _toolsSubTabContents[2] = _root.Q<VisualElement>("subcontent-importexport");

            for (int i = 0; i < _toolsSubTabButtons.Length; i++)
            {
                int index = i;
                _toolsSubTabButtons[i].clicked += () => SwitchToolsSubTab(index);
            }
        }

        private void InitializeButtons()
        {
            // Welcome tab buttons
            _root.Q<Button>("btn-create-database").clicked += CreateNewDatabase;
            _root.Q<Button>("btn-open-database").clicked += () => SwitchToTab(1);

            // Setup tab buttons
            _root.Q<Button>("btn-open-asset").clicked += OpenDatabaseAsset;

            // Definitions tab buttons
            _root.Q<Button>("btn-add-role").clicked += CreateNewRoleDefinition;

            // Tools tab buttons
            _root.Q<Button>("btn-test-integrity").clicked += TestDatabaseIntegrity;
            _root.Q<Button>("btn-print-info").clicked += PrintDatabaseInfo;
            _root.Q<Button>("btn-list-roles").clicked += ListAllRoles;
        }

        private void InitializeDatabaseField()
        {
            var databaseField = _root.Q<UnityEditor.UIElements.ObjectField>("database-field");
            if (databaseField != null)
            {
                databaseField.objectType = typeof(FootballDatabase);
                databaseField.RegisterValueChangedCallback(evt =>
                {
                    _database = evt.newValue as FootballDatabase;
                    OnDatabaseChanged();
                });
            }
        }

        #endregion

        #region Tab Switching

        public void SwitchToTab(int tabIndex)
        {
            // Update button states
            for (int i = 0; i < _tabButtons.Length; i++)
            {
                if (i == tabIndex)
                {
                    _tabButtons[i].AddToClassList("tab-button-active");
                }
                else
                {
                    _tabButtons[i].RemoveFromClassList("tab-button-active");
                }
            }

            // Show/hide content
            for (int i = 0; i < _tabContents.Length; i++)
            {
                if (i == tabIndex)
                {
                    _tabContents[i].RemoveFromClassList("hidden");
                }
                else
                {
                    _tabContents[i].AddToClassList("hidden");
                }
            }

            // Update tab-specific UI
            if (tabIndex == 1) // Setup tab
            {
                UpdateSetupTab();
            }
            else if (tabIndex == 2) // Definitions tab
            {
                UpdateDefinitionsTab();
            }
        }

        private void SwitchDefinitionsSubTab(int subTabIndex)
        {
            for (int i = 0; i < _defSubTabButtons.Length; i++)
            {
                if (i == subTabIndex)
                {
                    _defSubTabButtons[i].AddToClassList("sub-tab-button-active");
                    _defSubTabContents[i].RemoveFromClassList("hidden");
                }
                else
                {
                    _defSubTabButtons[i].RemoveFromClassList("sub-tab-button-active");
                    _defSubTabContents[i].AddToClassList("hidden");
                }
            }

            if (subTabIndex == 0) // Roles sub-tab
            {
                UpdateRolesSubTab();
            }
        }

        private void SwitchToolsSubTab(int subTabIndex)
        {
            for (int i = 0; i < _toolsSubTabButtons.Length; i++)
            {
                if (i == subTabIndex)
                {
                    _toolsSubTabButtons[i].AddToClassList("sub-tab-button-active");
                    _toolsSubTabContents[i].RemoveFromClassList("hidden");
                }
                else
                {
                    _toolsSubTabButtons[i].RemoveFromClassList("sub-tab-button-active");
                    _toolsSubTabContents[i].AddToClassList("hidden");
                }
            }
        }

        #endregion

        #region Database Updates

        private void OnDatabaseChanged()
        {
            // Update header
            var headerLabel = _root.Q<Label>("database-name-label");
            if (headerLabel != null)
            {
                headerLabel.text = _database != null ? $"DB: {_database.DatabaseName}" : "No Database Selected";
            }

            // Update setup tab
            UpdateSetupTab();

            // Update definitions tab
            UpdateDefinitionsTab();
        }

        private void UpdateSetupTab()
        {
            var databaseInfo = _root.Q<VisualElement>("database-info");
            var statistics = _root.Q<VisualElement>("statistics");
            var openAssetBtn = _root.Q<Button>("btn-open-asset");

            if (_database == null)
            {
                databaseInfo?.AddToClassList("hidden");
                statistics?.AddToClassList("hidden");
                openAssetBtn?.AddToClassList("hidden");
                return;
            }

            databaseInfo?.RemoveFromClassList("hidden");
            statistics?.RemoveFromClassList("hidden");
            openAssetBtn?.RemoveFromClassList("hidden");

            // Update database info
            _root.Q<Label>("info-name")?.ChangeText(_database.DatabaseName);
            _root.Q<Label>("info-version")?.ChangeText(_database.DatabaseVersion);
            _root.Q<Label>("info-description")?.ChangeText(_database.Description);

            var validLabel = _root.Q<Label>("info-valid");
            if (validLabel != null)
            {
                validLabel.text = _database.IsValid ? "✓ Yes" : "✗ No";
                validLabel.style.color = _database.IsValid ? Color.green : Color.red;
            }

            // Update statistics
            _root.Q<Label>("stat-roles")?.ChangeText(_database.RoleDefinitionCount.ToString());
        }

        private void UpdateDefinitionsTab()
        {
            UpdateRolesSubTab();
        }

        private void UpdateRolesSubTab()
        {
            var rolesList = _root.Q<VisualElement>("roles-list");
            if (rolesList == null) return;

            rolesList.Clear();

            if (_database == null || _database.RoleDefinitions == null || _database.RoleDefinitions.Length == 0)
            {
                var helpBox = new VisualElement();
                helpBox.AddToClassList("help-box");
                var label = new Label("No role definitions found in this database.");
                label.AddToClassList("help-text");
                helpBox.Add(label);
                rolesList.Add(helpBox);
                return;
            }

            // Add each role
            for (int i = 0; i < _database.RoleDefinitions.Length; i++)
            {
                var role = _database.RoleDefinitions[i];
                if (role == null)
                {
                    var missingLabel = new Label($"[{i}] Missing Role Definition");
                    missingLabel.style.color = Color.red;
                    missingLabel.style.paddingTop = 4;
                    missingLabel.style.paddingBottom = 4;
                    missingLabel.style.paddingLeft = 4;
                    missingLabel.style.paddingRight = 4;
                    rolesList.Add(missingLabel);
                    continue;
                }

                var roleItem = CreateRoleListItem(role, i);
                rolesList.Add(roleItem);
            }
        }

        private VisualElement CreateRoleListItem(Core.Tactics.TacticalRoleDefinition role, int index)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            container.style.marginBottom = 2;
            container.style.paddingTop = 6;
            container.style.paddingBottom = 6;
            container.style.paddingLeft = 8;
            container.style.paddingRight = 8;
            container.style.borderBottomLeftRadius = 3;
            container.style.borderBottomRightRadius = 3;
            container.style.borderTopLeftRadius = 3;
            container.style.borderTopRightRadius = 3;

            var indexLabel = new Label($"{index + 1}.");
            indexLabel.style.minWidth = 30;
            indexLabel.style.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            container.Add(indexLabel);

            var nameLabel = new Label(role.name);
            nameLabel.style.flexGrow = 1;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            container.Add(nameLabel);

            var selectBtn = new Button(() =>
            {
                Selection.activeObject = role as UnityEngine.Object;
                EditorGUIUtility.PingObject(role as UnityEngine.Object);
            });
            selectBtn.text = "Select";
            selectBtn.style.width = 70;
            selectBtn.style.marginLeft = 4;
            container.Add(selectBtn);

            var editBtn = new Button(() =>
            {
                Selection.activeObject = role as UnityEngine.Object;
                EditorGUIUtility.PingObject(role as UnityEngine.Object);
            });
            editBtn.text = "Edit";
            editBtn.style.width = 70;
            editBtn.style.marginLeft = 4;
            container.Add(editBtn);

            return container;
        }

        #endregion

        #region Button Actions

        private void CreateNewDatabase()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Football Database",
                "FootballDatabase",
                "asset",
                "Create a new football database");

            if (string.IsNullOrEmpty(path)) return;

            var database = CreateInstance<FootballDatabase>();
            AssetDatabase.CreateAsset(database, path);
            AssetDatabase.SaveAssets();

            _database = database;

            // Update the ObjectField
            var databaseField = _root.Q<UnityEditor.UIElements.ObjectField>("database-field");
            if (databaseField != null)
            {
                databaseField.value = database as UnityEngine.Object;
            }

            SwitchToTab(1); // Switch to Setup tab

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = database as UnityEngine.Object;

            Debug.Log($"Created new database at: {path}");
        }

        private void OpenDatabaseAsset()
        {
            if (_database != null)
            {
                Selection.activeObject = _database as UnityEngine.Object;
                EditorGUIUtility.PingObject(_database as UnityEngine.Object);
            }
        }

        private void CreateNewRoleDefinition()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create New Role Definition",
                "NewRole",
                "asset",
                "Choose where to save the new role definition");

            if (string.IsNullOrEmpty(path)) return;

            var role = ScriptableObject.CreateInstance<TacticalRoleDefinition>();
            role.roleName = "New Role";

            AssetDatabase.CreateAsset(role, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = role;
            EditorGUIUtility.PingObject(role);

            UpdateRolesSubTab();

            Debug.Log($"Created new role at: {path}");
        }

        private void TestDatabaseIntegrity()
        {
            if (_database == null)
            {
                EditorUtility.DisplayDialog("Error", "No database selected!", "OK");
                return;
            }

            bool isValid = _database.IsValid;
            EditorUtility.DisplayDialog(
                "Database Test",
                isValid
                    ? "✓ Database is valid!\n\nAll checks passed."
                    : "✗ Database has issues!\n\nPlease check the console for details.",
                "OK");

            if (!isValid)
            {
                Debug.LogWarning("Database validation failed. Check role definitions.");
            }
            else
            {
                Debug.Log("Database validation passed!");
            }
        }

        private void PrintDatabaseInfo()
        {
            if (_database == null)
            {
                Debug.LogWarning("No database selected!");
                return;
            }

            Debug.Log("=== Database Information ===");
            Debug.Log($"Name: {_database.DatabaseName}");
            Debug.Log($"Version: {_database.DatabaseVersion}");
            Debug.Log($"Description: {_database.Description}");
            Debug.Log($"Role Definitions: {_database.RoleDefinitionCount}");
            Debug.Log($"Valid: {_database.IsValid}");
            Debug.Log("===========================");
        }

        private void ListAllRoles()
        {
            if (_database == null)
            {
                Debug.LogWarning("No database selected!");
                return;
            }

            Debug.Log("=== Role Definitions ===");
            if (_database.RoleDefinitions != null)
            {
                for (int i = 0; i < _database.RoleDefinitions.Length; i++)
                {
                    var role = _database.RoleDefinitions[i];
                    Debug.Log($"[{i}] {(role != null ? role.name : "NULL")}");
                }
            }
            else
            {
                Debug.Log("No role definitions found.");
            }
            Debug.Log("========================");
        }

        #endregion
    }

    // Extension methods for Label
    public static class LabelExtensions
    {
        public static void ChangeText(this Label label, string text)
        {
            if (label != null)
            {
                label.text = text;
            }
        }
    }
}
