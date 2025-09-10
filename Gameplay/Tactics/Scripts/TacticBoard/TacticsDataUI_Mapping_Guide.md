# Tactics Data-UI Mapping Guide
## Ultimate Football System - Tactics Board

### Overview

This document explains how tactical data flows between the data layer and UI components in the Ultimate Football System's tactics board. It covers formation changes, player assignments, save/load operations, and UI updates.

---

## 🏗️ **Core Architecture**

### **Data Layer**
```
Core Data Models:
├── Tactic (Core.TacticsEngine.Tactic)
│   ├── ActivePositions: List<TacticalPosition>
│   ├── Substitutes: List<int?> (Player IDs)
│   └── Reserves: List<int?> (Player IDs)
├── TacticData (Core.TacticsEngine.Types.TacticData)
│   ├── Positions: List<PositionData>
│   ├── Substitutes: List<int?>
│   └── Reserves: List<int?>
└── PositionData (Core.TacticsEngine.Types.PositionData)
    ├── Position: TacticalPositionOption
    ├── PlayerId: int?
    ├── RoleType: TacticalRoleOption
    └── Duty: TacticalDutyOption
```

### **UI Layer**
```
UI Components:
├── TacticsBoardController (Main Controller)
│   ├── StartingPositionPlayerMapping: Dictionary<TacticalPositionOption, Player?>
│   ├── SubstitutesPlayersItems: ObservableList<Player>
│   └── ReservePlayersItems: ObservableList<Player>
├── PlayerItemView[] (Visual representations)
│   ├── startingPlayersViews[11]
│   ├── substitutesPlayersViews[N]
│   └── reservePlayersViews[M]
└── PositionZoneView[] (Formation slots on pitch)
    └── childPlayerItemView: PlayerItemView
```

---

## ⚽ **Formation Management**

### **Formation Change Flow**
```mermaid
graph TD
    A[User Selects Formation] --> B[TacticsBoardController receives input]
    B --> C[BoardTacticManager.SetFormationViews()]
    C --> D[Update PositionZoneView.InUseForFormation]
    D --> E[PlayerItemView.SetInUseForFormation()]
    E --> F[Visual UI updates on pitch]
    F --> G[UpdateTacticFromActiveZones()]
    G --> H[Tactic.ActivePositions updated]
```

### **Formation Templates**
Located in `FormationsPositions.cs`:
- **F442**: 4-4-2 Formation (11 positions)
- **F4141**: 4-1-4-1 Formation (11 positions)  
- **F433_DM_Wide**: 4-3-3 with Defensive Mid (11 positions)
- **F4231_Wide**: 4-2-3-1 Wide (11 positions)
- **F3232_352**: 3-5-2 Formation (11 positions)
- **F343**: 3-4-3 Formation (11 positions)

### **Formation Setting Process**
```csharp
// 1. Formation template defines positions
TacticalPositionOption[] formation = FormationsPositions.F442;

// 2. BoardTacticManager applies formation to UI
boardTacticManager.SetFormationViews(formation);

// 3. Zone views update their formation status
foreach (PositionZoneView zone in zones)
{
    zone.InUseForFormation = formation.Contains(zone.tacticalPositionOption);
    zone.childPlayerItemView.SetInUseForFormation(zone.InUseForFormation);
}

// 4. Current tactic is updated
currentTactic.ActivePositions = GetActivePositionsFromUI();
```

---

## 👥 **Player Assignment System**

### **Data Mapping Structure**

#### **Starting Players (Pitch)**
```csharp
// Data Storage
Dictionary<TacticalPositionOption, Player?> StartingPositionPlayerMapping;

// UI Representation  
PlayerItemView[] startingPlayersViews; // [11] elements

// Mapping Logic
foreach (var position in formation)
{
    var player = StartingPositionPlayerMapping[position];
    var view = GetViewForPosition(position);
    view.SetPlayerData(player);
}
```

#### **Substitute Players (Bench)**
```csharp
// Data Storage
ObservableList<Player> SubstitutesPlayersItems;

// UI Representation
PlayerItemView[] substitutesPlayersViews; // [allowedSubstitutes] elements

// Mapping Logic
for (int i = 0; i < SubstitutesPlayersItems.Count; i++)
{
    substitutesPlayersViews[i].SetPlayerData(SubstitutesPlayersItems[i]);
}
```

#### **Reserve Players (Squad)**
```csharp
// Data Storage
ObservableList<Player> ReservePlayersItems;

// UI Representation
PlayerItemView[] reservePlayersViews; // Dynamic size

// Mapping Logic
for (int i = 0; i < ReservePlayersItems.Count; i++)
{
    reservePlayersViews[i].SetPlayerData(ReservePlayersItems[i]);
}
```

### **Player Assignment Flow**
```mermaid
graph TD
    A[Player Data from PlayerDataManager] --> B[TacticsBoardController.ProcessPlayerData()]
    B --> C[Update StartingPositionPlayerMapping]
    B --> D[Update SubstitutesPlayersItems]  
    B --> E[Update ReservePlayersItems]
    C --> F[BoardInitializationManager.LoadPlayers()]
    D --> G[BoardInitializationManager.InitializeSubstitutePlayers()]
    E --> H[BoardInitializationManager.InitializeReservePlayers()]
    F --> I[Update PlayerItemViews on pitch]
    G --> J[Update PlayerItemViews on bench]
    H --> K[Update PlayerItemViews in reserves]
```

---

## 💾 **Save/Load Operations**

### **Save Process**
```mermaid
graph TD
    A[User triggers save] --> B[BoardTacticManager.SaveCurrentTactic()]
    B --> C[Collect current UI state]
    C --> D[Update Tactic object from active zones]
    D --> E[Tactic.ToJson()]
    E --> F[Write to file system]
    F --> G[File saved to persistentDataPath/Tactics/]
```

### **Save Implementation**
```csharp
public void SaveCurrentTactic(string fileName)
{
    // 1. Update tactic from current UI state
    UpdateTacticFromActiveZones();
    
    // 2. Serialize to JSON
    var json = currentTactic.ToJson();
    
    // 3. Write to persistent storage
    var path = Path.Combine(Application.persistentDataPath, "Tactics", fileName);
    File.WriteAllText(path, json);
}

// Automatic filename generation
public void SaveTacticWithTimestamp()
{
    var formationString = currentTactic.FormationToString(); // e.g., "4-4-2"
    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    var fileName = $"Tactic_{formationString}_{timestamp}.json";
    SaveCurrentTactic(fileName);
}
```

### **Load Process**
```mermaid
graph TD
    A[User selects tactic file] --> B[BoardTacticManager.LoadTactic()]
    B --> C[Read JSON from file]
    C --> D[Tactic.FromJson()]
    D --> E[Extract formation positions]
    E --> F[SetFormationViews() - Update UI zones]
    F --> G[LoadPlayers() - Apply player assignments]
    G --> H[UI reflects loaded tactic]
```

### **Load Implementation**
```csharp
public void LoadTactic(string fileName)
{
    // 1. Read file from persistent storage
    var path = Path.Combine(Application.persistentDataPath, "Tactics", fileName);
    var json = File.ReadAllText(path);
    
    // 2. Deserialize tactic
    currentTactic = Tactic.FromJson(json);
    
    // 3. Extract formation positions
    var formationPositions = currentTactic.ActivePositions
        .Select(p => p.Position)
        .ToArray();
    
    // 4. Update UI to match loaded formation
    SetFormationViews(formationPositions, initCall: true);
    
    // 5. Apply player assignments
    ApplyPlayerAssignments(currentTactic);
}
```

---

## 🔄 **UI Update Mechanisms**

### **Observable Collections**
The system uses `ObservableList<T>` for automatic UI updates:

```csharp
// Automatic header updates when collections change
SubstitutesPlayersItems.OnCollectionChange += () =>
{
    substitutesListSection.UpdateFormattedHeaderText(
        SubstitutesPlayersItems.Count.ToString()
    );
};

ReservePlayersItems.OnCollectionChange += () =>
{
    reserveListSection.UpdateFormattedHeaderText(
        ReservePlayersItems.Count.ToString()
    );
};
```

### **PlayerItemView Updates**
Each `PlayerItemView` handles its own display updates:

```csharp
public void SetPlayerData(Player profile)
{
    Profile = profile;
    HasPlayerItem = (profile != null && profile.Id >= 1);
    
    // Update visual representation
    mainView.Show();
    mainView.UpdateView(); // Triggers visual refresh
    
    // Notify listeners of changes
    if (oldHasPlayerItem != HasPlayerItem)
    {
        OnFormationStatusChanged?.Invoke(HasPlayerItem);
    }
}
```

---

## 🧹 **Clear Operations**

### **Clear Formation**
```csharp
public void ClearFormation()
{
    // 1. Clear active positions
    currentTactic.ActivePositions.Clear();
    
    // 2. Reset all zone views
    foreach (var zoneContainer in zoneContainerViews)
    {
        foreach (var zoneView in zoneContainer.ZoneViews)
        {
            zoneView.SetInUseForFormation(false);
            zoneView.childPlayerItemView.SetPlayerData(null);
        }
    }
    
    // 3. Update UI display
    UpdateTacticFromActiveZones();
}
```

### **Clear Player Assignments**
```csharp
public void ClearAllPlayerAssignments()
{
    // Clear starting positions
    foreach (var key in StartingPositionPlayerMapping.Keys.ToList())
    {
        StartingPositionPlayerMapping[key] = null;
    }
    
    // Clear substitutes and reserves
    SubstitutesPlayersItems.Clear();
    ReservePlayersItems.Clear();
    
    // Update UI views
    RefreshAllPlayerViews();
}
```

---

## 🎮 **User Interaction Flow**

### **Drag & Drop Player Assignment**
```mermaid
graph TD
    A[User drags player] --> B[PlayerItemDragSupport.OnBeginDrag()]
    B --> C[Create PlayerItemDragData]
    C --> D[Show drag visual feedback]
    D --> E[User drops on target position]
    E --> F[PlayerItemDropSupport.OnDrop()]
    F --> G[Validate drop target]
    G --> H[Update data mappings]
    H --> I[Refresh UI views]
    I --> J[UpdateTacticFromActiveZones()]
```

### **Formation Button Clicks**
```csharp
// Example: 4-4-2 formation button
public void On442ButtonClick()
{
    if (_team?.ActiveTactic?.ActivePositions?.Count > 0)
    {
        // Apply formation and load existing players
        boardTacticManager.SetFormationViews(FormationsPositions.F442);
        BoardInitializationManager.LoadPlayersAutofill(
            StartingPositionPlayerMapping.Values
        );
    }
}
```

---

## 📊 **Data Synchronization Points**

### **Critical Sync Operations**

1. **Formation Change → UI Update**
   - Trigger: User selects new formation
   - Action: `SetFormationViews()` updates zone visibility
   - Result: UI reflects new formation shape

2. **Player Assignment → Data Update**
   - Trigger: Drag & drop or direct assignment
   - Action: Update mapping dictionaries
   - Result: Data layer synchronized with UI

3. **UI Interaction → Tactic Update**
   - Trigger: Any player or formation change
   - Action: `UpdateTacticFromActiveZones()`
   - Result: Core tactic object updated

4. **Save/Load → Full Sync**
   - Trigger: Save/load operations
   - Action: Bidirectional data-UI synchronization
   - Result: Complete state consistency

---

## 🎯 **Key Integration Points**

### **Data → UI Flow**
```
PlayerDataManager → TacticsBoardController → BoardInitializationManager → PlayerItemView
```

### **UI → Data Flow**
```
User Interaction → PlayerItemView → TacticsBoardController → BoardTacticManager → Tactic
```

### **Persistence Flow**
```
Tactic ↔ JSON ↔ File System (persistentDataPath/Tactics/)
```

---

## 🔧 **Common Operations**

### **Adding a New Player**
1. Update source data in `PlayerDataManager`
2. Refresh `StartingPositionPlayerMapping`, `SubstitutesPlayersItems`, or `ReservePlayersItems`
3. Call appropriate initialization method
4. UI automatically updates via `ObservableList` events

### **Changing Formation**
1. Select formation template from `FormationsPositions`
2. Call `BoardTacticManager.SetFormationViews(formation)`
3. Optionally preserve player assignments with `LoadPlayersAutofill()`
4. System automatically updates tactic data

### **Saving Current State**
1. User triggers save action
2. `UpdateTacticFromActiveZones()` synchronizes data
3. `SaveCurrentTactic()` or `SaveTacticWithTimestamp()` persists to disk
4. File saved with formation and timestamp information

This system ensures robust bidirectional data binding between the tactical data layer and the interactive UI components, providing a seamless user experience for formation and squad management.