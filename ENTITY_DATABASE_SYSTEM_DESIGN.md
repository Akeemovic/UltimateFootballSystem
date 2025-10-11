# Entity Database System Design

## Overview

A **database-centric architecture** for managing all game entities (Players, Teams, Clubs, Countries, Roles, etc.) with support for:
- Design-time editing via custom Unity Editor tools
- Runtime loading from multiple sources (JSON, Binary, Server)
- Save game support (player growth/decline, team changes)
- Modding support (user-created JSON files)
- Multiplayer synchronization

**Inspired by:** Ultimate Inventory System's database pattern

---

## Core Architecture

### The Central Database Pattern

```
┌─────────────────────────────────────────────────┐
│      FootballDatabase (ScriptableObject)        │
│  The single source of truth - everything refs   │
├─────────────────────────────────────────────────┤
│  Defaults (Design-time):                        │
│  ├── List<RoleDefinition> Roles                 │
│  ├── List<Continent> Continents                 │
│  ├── List<Country> Countries                    │
│  ├── List<Club> Clubs                           │
│  ├── List<Competition> Competitions             │
│  └── DatabaseSettings Settings                  │
│                                                   │
│  Runtime Overrides (per save/multiplayer):      │
│  ├── List<Player> Players (mutable)             │
│  ├── List<Team> Teams (mutable)                 │
│  ├── MatchState CurrentMatch                    │
│  └── SaveMetadata CurrentSave                   │
│                                                   │
│  Methods:                                        │
│  ├── Player GetPlayerById(int id)               │
│  ├── List<Club> GetClubsByCountry(Country c)    │
│  ├── LoadFromJson(string path)                  │
│  ├── SaveToJson(string path)                    │
│  └── ValidateReferences()                       │
└─────────────────────────────────────────────────┘
```

---

## Entity Categories

### Design-Time Entities (Static/Rarely Change)
These are authored in the editor and form the game's world structure:

- **Continents** - Europe, Africa, South America, etc.
- **Countries** - England, Spain, Brazil, etc.
- **Clubs** - Manchester United, Barcelona, etc.
- **Competitions** - Premier League, Champions League, etc.
- **RoleDefinitions** - Striker, Winger, Defensive Midfielder, etc.
- **FormationTemplates** - 4-3-3, 4-4-2, 3-5-2, etc.

**Characteristics:**
- Defined at design time
- Referenced by ID
- Hierarchical (Continent → Country → Club → Competition)
- Exported to `defaults.json` for distribution

### Runtime Entities (Dynamic/Per Save)
These change during gameplay and are saved per game session:

- **Players** - Individual player instances with stats that grow/decline
- **Teams** - Squad compositions, tactics, morale
- **Coaches** - Manager instances with reputation, contracts
- **Staff** - Scouts, trainers, physios

**Characteristics:**
- Loaded from save files or generated
- Mutable stats (age, rating, potential, injuries)
- Different per save game
- Can be modded via JSON

### Runtime-Only Entities (Never Persisted in Database)
These are generated during gameplay and don't need design-time definitions:

- **Match** - Individual match instances
- **MatchEvent** - Goals, cards, substitutions
- **TransferOffer** - Bid for a player
- **Contract** - Player/manager contracts
- **Injury** - Player injury state
- **Suspension** - Player suspension state

**Characteristics:**
- Created programmatically
- Temporary (exist during match/transfer window)
- Not stored in database
- Generated from game logic

---

## Data Architecture

### FootballDatabase (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "FootballDatabase", menuName = "Football/Database")]
public class FootballDatabase : ScriptableObject
{
    #region Design-Time Definitions (Static/Defaults)

    [Header("World Structure")]
    [SerializeField] private List<Continent> continents = new();
    [SerializeField] private List<Country> countries = new();
    [SerializeField] private List<Club> clubs = new();
    [SerializeField] private List<Competition> competitions = new();

    [Header("Game Definitions")]
    [SerializeField] private List<RoleDefinition> roleDefinitions = new();
    [SerializeField] private List<FormationTemplate> formationTemplates = new();

    #endregion

    #region Runtime Data (Mutable per save/session)

    // These get loaded from JSON/save file or server
    private List<Player> _players = new();
    private List<Team> _teams = new();
    private Dictionary<int, Player> _playerLookup = new();

    #endregion

    #region Initialization & Loading

    public void Initialize(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.NewCareer:
                LoadDefaultPlayers(); // From JSON or generated
                break;
            case GameMode.LoadSave:
                LoadFromSaveFile(SaveManager.CurrentSavePath);
                break;
            case GameMode.Multiplayer:
                LoadFromServer(ServerManager.ServerUrl);
                break;
        }

        BuildLookupTables();
        ValidateReferences();
    }

    public void LoadFromJson(string path)
    {
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<DatabaseSnapshot>(json);
        ApplySnapshot(data);
    }

    public void SaveToJson(string path)
    {
        var snapshot = CreateSnapshot();
        var json = JsonUtility.ToJson(snapshot, true);
        File.WriteAllText(path, json);
    }

    #endregion

    #region Query Methods

    public Player GetPlayerById(int id) => _playerLookup.TryGetValue(id, out var p) ? p : null;

    public List<Club> GetClubsByCountry(int countryId)
        => clubs.Where(c => c.CountryId == countryId).ToList();

    public List<Competition> GetCompetitionsByCountry(int countryId)
        => competitions.Where(c => c.CountryId == countryId).ToList();

    public RoleDefinition GetRoleDefinition(TacticalRoleOption role)
        => roleDefinitions.FirstOrDefault(r => r.Role == role);

    #endregion

    #region Validation

    public void ValidateReferences()
    {
        // Check for orphaned references
        foreach (var club in clubs)
        {
            if (!countries.Any(c => c.Id == club.CountryId))
                Debug.LogError($"Club {club.Name} references invalid country {club.CountryId}");
        }

        foreach (var competition in competitions)
        {
            foreach (var clubId in competition.ParticipatingClubIds)
            {
                if (!clubs.Any(c => c.Id == clubId))
                    Debug.LogError($"Competition {competition.Name} references invalid club {clubId}");
            }
        }
    }

    #endregion
}
```

---

## Entity Definitions

### Base Interface

```csharp
public interface IEntity
{
    int Id { get; }
    string Name { get; }
}
```

### Design-Time Entities

```csharp
[System.Serializable]
public class Continent : IEntity
{
    public int Id;
    public string Name;
    public string Code; // e.g., "EU", "SA", "AS"
}

[System.Serializable]
public class Country : IEntity
{
    public int Id;
    public string Name;
    public int ContinentId; // Reference to Continent
    public string Code; // e.g., "ENG", "ESP", "BRA"
    public string FlagSpritePath;
}

[System.Serializable]
public class Club : IEntity
{
    public int Id;
    public string Name;
    public int CountryId; // Reference to Country
    public string BadgeSpritePath;
    public int FoundedYear;
    public string StadiumName;
    public int StadiumCapacity;
}

[System.Serializable]
public class Competition : IEntity
{
    public int Id;
    public string Name;
    public int CountryId; // Or ContinentId for international competitions
    public List<int> ParticipatingClubIds; // References to Clubs
    public CompetitionType Type; // League, Cup, Continental
    public int NumberOfRounds;
    public bool HasKnockoutStage;
}
```

### Runtime Entities

```csharp
[System.Serializable]
public class Player : IEntity
{
    public int Id;
    public string Name;
    public int Age;
    public int CurrentClubId;

    // Growth/decline stats (change per save)
    public float Rating;
    public float Potential;
    public int MatchesPlayed;
    public int GoalsScored;
    public int Assists;

    // Physical attributes
    public int Pace;
    public int Shooting;
    public int Passing;
    public int Dribbling;
    public int Defending;
    public int Physical;
}

[System.Serializable]
public class Team
{
    public int Id;
    public string Name;
    public int ClubId; // Reference to Club
    public List<int> PlayerIds; // References to Players
    public Tactic ActiveTactic;
    public float Morale;
    public int LeaguePosition;
}
```

---

## Manager Pattern (Runtime Access)

### Base EntityManager

```csharp
public abstract class EntityManager<T> where T : IEntity
{
    protected FootballDatabase Database { get; private set; }
    protected Dictionary<int, T> Lookup { get; } = new();

    public void Initialize(FootballDatabase database)
    {
        Database = database;
        BuildLookup();
    }

    protected abstract void BuildLookup();

    public T GetById(int id) => Lookup.TryGetValue(id, out var entity) ? entity : default;
    public IEnumerable<T> GetAll() => Lookup.Values;
    public int Count => Lookup.Count;
}
```

### Specific Managers

```csharp
public class PlayerManager : EntityManager<Player>
{
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance ??= new();

    protected override void BuildLookup()
    {
        foreach (var player in Database.GetAllPlayers())
            Lookup[player.Id] = player;
    }

    public void UpdatePlayerStats(int playerId, MatchStats stats)
    {
        var player = GetById(playerId);
        if (player != null)
        {
            player.MatchesPlayed++;
            player.GoalsScored += stats.Goals;
            player.Assists += stats.Assists;
            // Growth/decline logic here
        }
    }

    public List<Player> GetPlayersByClub(int clubId)
        => GetAll().Where(p => p.CurrentClubId == clubId).ToList();
}

public class ClubManager : EntityManager<Club>
{
    private static ClubManager _instance;
    public static ClubManager Instance => _instance ??= new();

    protected override void BuildLookup()
    {
        foreach (var club in Database.GetAllClubs())
            Lookup[club.Id] = club;
    }

    public List<Club> GetClubsByCountry(int countryId)
        => GetAll().Where(c => c.CountryId == countryId).ToList();
}

public class CompetitionManager : EntityManager<Competition>
{
    private static CompetitionManager _instance;
    public static CompetitionManager Instance => _instance ??= new();

    protected override void BuildLookup()
    {
        foreach (var competition in Database.GetAllCompetitions())
            Lookup[competition.Id] = competition;
    }

    public List<Competition> GetCompetitionsByCountry(int countryId)
        => GetAll().Where(c => c.CountryId == countryId).ToList();

    public List<Club> GetParticipatingClubs(int competitionId)
    {
        var competition = GetById(competitionId);
        return competition?.ParticipatingClubIds
            .Select(id => ClubManager.Instance.GetById(id))
            .Where(club => club != null)
            .ToList() ?? new List<Club>();
    }
}
```

---

## Database Manager (Single Entry Point)

```csharp
public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private FootballDatabase database;

    private static DatabaseManager _instance;
    public static FootballDatabase Database => _instance.database;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {
        // Initialize database with appropriate mode
        database.Initialize(GameStateManager.GameMode);

        // Initialize all managers
        PlayerManager.Instance.Initialize(database);
        ClubManager.Instance.Initialize(database);
        CountryManager.Instance.Initialize(database);
        CompetitionManager.Instance.Initialize(database);
        RoleManager.Instance.Initialize(database);
    }
}

// Usage from anywhere in the game:
public class TacticsBoardController : MonoBehaviour
{
    private void Start()
    {
        // Get player from centralized database
        var player = PlayerManager.Instance.GetById(123);

        // Get role definition from database
        var role = DatabaseManager.Database.GetRoleDefinition(TacticalRoleOption.Striker);

        // Get clubs by country
        var englishClubs = ClubManager.Instance.GetClubsByCountry(countryId: 44);
    }
}
```

---

## Data Flow

### Design Time Flow

```
1. Editor Tool (Unity Editor)
   ↓ Create/Edit entities
2. FootballDatabase.asset (ScriptableObject)
   ↓ Export to
3. defaults.json (JSON file for distribution)
```

### Runtime Flow

```
1. Game Start
   ↓
2. DatabaseManager.Initialize()
   ↓ Loads from (priority order):
   - save_001.json (Load Save - players evolved)
   - server_data.json (Multiplayer - synced data)
   - mods/custom.json (User Mods - custom content)
   - defaults.json (New Game - fresh start)
   - FootballDatabase.asset (Fallback - bundled defaults)
   ↓
3. Entity Managers Initialized
   - PlayerManager
   - ClubManager
   - CompetitionManager
   - RoleManager
   ↓
4. Game Systems Access Data
   - TacticBoard
   - Match Engine
   - Transfer Market
```

---

## Custom Editor Tool Design

### Database Inspector Window

```
┌──────────────────────────────────────────────────────┐
│  ⚽ Football Database Manager                        │
├────────────┬─────────────────────────────────────────┤
│ World      │  Database: [FootballDatabase.asset ▼]  │
│ ├ Continents│                                        │
│ ├ Countries│  Country: England                       │
│ └ Clubs    │  Continent: Europe                      │
│            │                                          │
│ Competitions│ Clubs (25):                            │
│            │  ┌────────────────────────────────┐    │
│ Definitions│  │ ☑ Manchester United            │    │
│ ├ Roles    │  │ ☑ Liverpool FC                 │    │
│ └ Formations│ │ ☑ Arsenal FC                   │    │
│            │  │ ☑ Chelsea FC                   │    │
│ Import/Export│ │ ... (21 more)                 │    │
│ ├ Import   │  └────────────────────────────────┘    │
│ ├ Export   │                                         │
│ └ Validate │  Competitions:                          │
│            │  ├ Premier League (20 clubs)            │
│ Settings   │  └ FA Cup (select from clubs above)    │
│            │                                          │
│            │  [Add Competition] [Edit] [Delete]      │
│            │                                          │
│            │  [Validate All References]              │
└────────────┴─────────────────────────────────────────┘
```

### Key Features

1. **Hierarchical Editing**
   - Continent → Country → Club → Competition
   - Visual tree structure
   - Drag-and-drop organization

2. **Reference Validation**
   - Can only select clubs from the selected country for competitions
   - Shows warnings for orphaned references
   - Real-time validation with error highlighting

3. **Import/Export**
   - Export entire database to JSON
   - Export individual entity types
   - Import from JSON (merge or replace)
   - Bulk operations

4. **Search & Filter**
   - Search by name, ID, or attributes
   - Filter by country, continent, type
   - Quick navigation

5. **Visual Relationship Indicators**
   - Show which clubs are in which competitions
   - Display country flags and club badges
   - Highlight missing references

---

## Implementation Approaches

### Option A: IMGUI (Faster Prototyping)

```csharp
public class DatabaseEditorWindow : EditorWindow
{
    [MenuItem("Tools/Football Database Manager")]
    public static void ShowWindow()
    {
        GetWindow<DatabaseEditorWindow>("Database Manager");
    }

    private int selectedTab = 0;
    private string[] tabs = { "World", "Competitions", "Definitions", "Import/Export" };

    private void OnGUI()
    {
        selectedTab = GUILayout.Toolbar(selectedTab, tabs);

        switch (selectedTab)
        {
            case 0: DrawWorldTab(); break;
            case 1: DrawCompetitionsTab(); break;
            case 2: DrawDefinitionsTab(); break;
            case 3: DrawImportExportTab(); break;
        }
    }

    private void DrawWorldTab()
    {
        // Continent → Country → Club hierarchy
        EditorGUILayout.LabelField("World Structure", EditorStyles.boldLabel);
        // ... implementation
    }
}
```

**Pros:**
- Quick to implement
- Familiar if you know Unity
- Less boilerplate

**Cons:**
- Less flexible layouts
- Harder to maintain at scale
- No data binding

### Option B: UIElements (Modern, Scalable)

```csharp
public class DatabaseEditorWindow : EditorWindow
{
    [MenuItem("Tools/Football Database Manager")]
    public static void ShowWindow()
    {
        GetWindow<DatabaseEditorWindow>("Database Manager");
    }

    private void CreateGUI()
    {
        var root = rootVisualElement;

        // Load UXML layout
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/Editor/DatabaseManager.uxml");
        visualTree.CloneTree(root);

        // Setup tabs
        var tabView = root.Q<TabbedView>("mainTabs");
        SetupWorldTab(tabView);
        SetupCompetitionsTab(tabView);
        SetupDefinitionsTab(tabView);
    }
}
```

**Pros:**
- Modern UI framework
- Data binding support
- USS styling (like CSS)
- Better for complex UIs
- Responsive layouts

**Cons:**
- Steeper learning curve
- More setup required
- Verbose initially

---

## JSON Schema Examples

### defaults.json (Exported Database)

```json
{
  "continents": [
    {
      "id": 1,
      "name": "Europe",
      "code": "EU"
    }
  ],
  "countries": [
    {
      "id": 44,
      "name": "England",
      "continentId": 1,
      "code": "ENG",
      "flagSpritePath": "Flags/england"
    }
  ],
  "clubs": [
    {
      "id": 100,
      "name": "Manchester United",
      "countryId": 44,
      "badgeSpritePath": "Badges/manutd",
      "foundedYear": 1878,
      "stadiumName": "Old Trafford",
      "stadiumCapacity": 74879
    }
  ],
  "competitions": [
    {
      "id": 1,
      "name": "Premier League",
      "countryId": 44,
      "participatingClubIds": [100, 101, 102, ...],
      "type": "League",
      "numberOfRounds": 38,
      "hasKnockoutStage": false
    }
  ],
  "roleDefinitions": [
    {
      "role": "Striker",
      "name": "Striker",
      "abbreviation": "ST",
      "dutyOptions": ["Attack", "Support"]
    }
  ]
}
```

### save_001.json (Save Game)

```json
{
  "saveMetadata": {
    "saveId": "save_001",
    "playerName": "John Manager",
    "currentDate": "2024-01-15",
    "currentSeason": "2023-24",
    "managedClubId": 100
  },
  "players": [
    {
      "id": 1,
      "name": "Marcus Rashford",
      "age": 26,
      "currentClubId": 100,
      "rating": 85,
      "potential": 88,
      "matchesPlayed": 145,
      "goalsScored": 52,
      "assists": 28,
      "pace": 90,
      "shooting": 82,
      "passing": 78,
      "dribbling": 85,
      "defending": 45,
      "physical": 80
    }
  ],
  "teams": [
    {
      "id": 1,
      "name": "Manchester United First Team",
      "clubId": 100,
      "playerIds": [1, 2, 3, ...],
      "activeTactic": { ... },
      "morale": 75,
      "leaguePosition": 3
    }
  ]
}
```

---

## Data Loading Strategy

### Priority Order

```csharp
public void Initialize(GameMode mode)
{
    switch (mode)
    {
        case GameMode.NewCareer:
            // 1. Try load from defaults.json
            if (LoadFromJson("Data/defaults.json"))
                break;
            // 2. Fallback to ScriptableObject
            LoadFromScriptableObject();
            break;

        case GameMode.LoadSave:
            // 1. Load save file
            LoadFromJson($"Saves/{SaveManager.CurrentSaveName}.json");
            break;

        case GameMode.Multiplayer:
            // 1. Load from server
            LoadFromServer(ServerManager.ServerUrl);
            break;
    }

    BuildLookupTables();
    ValidateReferences();
}
```

### Modding Support

```csharp
public void LoadMods()
{
    var modFolder = Path.Combine(Application.persistentDataPath, "Mods");
    if (!Directory.Exists(modFolder)) return;

    // Load all JSON files from mods folder
    var modFiles = Directory.GetFiles(modFolder, "*.json");
    foreach (var modFile in modFiles)
    {
        try
        {
            var modData = LoadFromJson(modFile);
            MergeModData(modData); // Merge or replace based on mod settings
            Debug.Log($"Loaded mod: {Path.GetFileName(modFile)}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load mod {modFile}: {e.Message}");
        }
    }
}
```

---

## Validation System

### Reference Integrity Checks

```csharp
public class DatabaseValidator
{
    private FootballDatabase database;
    private List<ValidationError> errors = new();

    public List<ValidationError> Validate(FootballDatabase db)
    {
        database = db;
        errors.Clear();

        ValidateCountries();
        ValidateClubs();
        ValidateCompetitions();
        ValidatePlayers();

        return errors;
    }

    private void ValidateCountries()
    {
        foreach (var country in database.GetAllCountries())
        {
            // Check continent reference
            var continent = database.GetContinentById(country.ContinentId);
            if (continent == null)
            {
                errors.Add(new ValidationError
                {
                    Severity = ErrorSeverity.Error,
                    EntityType = "Country",
                    EntityId = country.Id,
                    Message = $"Country '{country.Name}' references invalid continent {country.ContinentId}"
                });
            }
        }
    }

    private void ValidateCompetitions()
    {
        foreach (var competition in database.GetAllCompetitions())
        {
            // Check participating clubs are from the same country
            foreach (var clubId in competition.ParticipatingClubIds)
            {
                var club = database.GetClubById(clubId);
                if (club == null)
                {
                    errors.Add(new ValidationError
                    {
                        Severity = ErrorSeverity.Error,
                        EntityType = "Competition",
                        EntityId = competition.Id,
                        Message = $"Competition '{competition.Name}' references invalid club {clubId}"
                    });
                }
                else if (club.CountryId != competition.CountryId)
                {
                    errors.Add(new ValidationError
                    {
                        Severity = ErrorSeverity.Warning,
                        EntityType = "Competition",
                        EntityId = competition.Id,
                        Message = $"Club '{club.Name}' is not from competition's country"
                    });
                }
            }
        }
    }
}

public class ValidationError
{
    public ErrorSeverity Severity;
    public string EntityType;
    public int EntityId;
    public string Message;
}

public enum ErrorSeverity
{
    Info,
    Warning,
    Error
}
```

---

## Scale Considerations (1000s-10000s Entities)

### Performance Optimizations

1. **Lazy Loading**
   - Don't load all players at once
   - Load by club/country/league as needed
   - Unload inactive entities

2. **Lookup Tables**
   - Use `Dictionary<int, T>` for O(1) lookups
   - Build indices on frequently queried fields
   - Cache expensive queries

3. **Chunked Loading**
   ```csharp
   public IEnumerator LoadPlayersChunked(int chunkSize = 1000)
   {
       var allPlayers = GetAllPlayersFromJson();
       for (int i = 0; i < allPlayers.Count; i += chunkSize)
       {
           var chunk = allPlayers.Skip(i).Take(chunkSize);
           foreach (var player in chunk)
           {
               _playerLookup[player.Id] = player;
           }
           yield return null; // Spread over frames
       }
   }
   ```

4. **Binary Serialization** (for large datasets)
   ```csharp
   // More efficient than JSON for 10000+ entities
   public void SaveToBinary(string path)
   {
       using var stream = File.OpenWrite(path);
       var formatter = new BinaryFormatter();
       formatter.Serialize(stream, database);
   }
   ```

---

## Implementation Phases

### Phase 1: Foundation (1-2 days)
- [ ] Create `IEntity` interface
- [ ] Define core entity classes (Continent, Country, Club, Competition)
- [ ] Create `FootballDatabase` ScriptableObject
- [ ] Basic JSON serialization/deserialization

### Phase 2: Managers (1-2 days)
- [ ] Create `EntityManager<T>` base class
- [ ] Implement specific managers (PlayerManager, ClubManager, etc.)
- [ ] Create `DatabaseManager` MonoBehaviour
- [ ] Test data loading and querying

### Phase 3: Editor Tool MVP (2-3 days)
- [ ] Basic EditorWindow with tabs (IMGUI)
- [ ] List view for entities
- [ ] Add/Edit/Delete operations
- [ ] Reference validation

### Phase 4: Advanced Features (2-3 days)
- [ ] Hierarchical editing (Continent → Country → Club)
- [ ] Import/Export tools
- [ ] Search and filtering
- [ ] Visual relationship indicators

### Phase 5: Runtime Integration (1-2 days)
- [ ] Integrate with existing systems (TacticBoard, etc.)
- [ ] Save/Load game support
- [ ] Mod loading system
- [ ] Performance optimization

---

## Questions to Consider

1. **ID Generation Strategy**
   - Auto-increment?
   - UUID?
   - Manual assignment?
   - Per-entity-type ranges (e.g., Players: 1-10000, Clubs: 10001-20000)?

2. **Asset Organization**
   - Where to store ScriptableObject assets?
   - Folder structure for exported JSON files?
   - Mod folder location?

3. **Migration Strategy**
   - How to handle schema changes between versions?
   - Backward compatibility for save files?
   - Auto-migration tools?

4. **Multiplayer Synchronization**
   - How often to sync with server?
   - Conflict resolution strategy?
   - Optimistic vs pessimistic updates?

5. **Editor Tool Workflow**
   - Should changes auto-save or require manual save?
   - Undo/redo support?
   - Multi-user editing (version control friendly)?

---

## References & Inspiration

- **Ultimate Inventory System** - Database-centric design pattern
- **Football Manager** - Data scale and entity relationships
- **Unity Addressables** - Lazy loading and asset management
- **ScriptableObject Architecture** - Ryan Hipple's Unite 2017 talk

---

## Next Steps

1. **Prototype FootballDatabase ScriptableObject**
2. **Create base EntityManager pattern**
3. **Build basic editor tool with tabs**
4. **Test with sample data (100 entities)**
5. **Iterate based on usability**

---

*This is a living document - update as design evolves*
