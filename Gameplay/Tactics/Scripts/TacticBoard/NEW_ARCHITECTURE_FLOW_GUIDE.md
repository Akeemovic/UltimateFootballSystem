# TacticBoard New Architecture Flow Guide

## Architecture Overview

The new architecture consists of 3 main components:

1. **TacticBoardModel** - Pure data layer (no Unity dependencies)
2. **TacticsBoardController** - Thin mediator between model and view
3. **TacticsPitchView** - View management and UI updates

## Core Data Flow Rules

### Rule 1: Model is Source of Truth
- `TacticBoardModel.StartingPositionPlayerMapping` contains EXACTLY 11 positions in the active formation
- Views display what's in the model
- The model dictionary should NEVER grow beyond 11 positions

### Rule 2: Formation State Lives in Views
- Each `PlayerItemView` has `InUseForFormation` boolean
- Views with `InUseForFormation=true` represent the active formation (11 positions)
- Views with `InUseForFormation=false` are hidden/inactive positions

### Rule 3: Formation Changes Must Sync Model
- When `InUseForFormation` changes, the model's dictionary must be updated
- Use `Model.SyncFormationFromViews()` to rebuild the dictionary with only active positions
- This keeps the dictionary at exactly 11 positions

---

## Flow 1: Swapping Two Positions BOTH In Formation

**Example:** Swap STC (Lopez) with MCR (De Bruyne) - both are in the active formation

### Steps:
1. **User clicks** STC (Lopez)
2. **SelectionSwapManager.SelectItem()** - Store selected item
3. **User clicks** MCR (De Bruyne)
4. **SelectionSwapManager.PerformSwap()**:
   - Detect: Both have `InUseForFormation=true` → NOT a formation change
   - Call `SwapInModel(sourceData, targetData)`
5. **SwapInModel()**:
   - Extract positions: sourcePos=STC, targetPos=MCR
   - Call `Model.SwapPlayers(STC, MCR, Lopez, De Bruyne)`
6. **Model.SwapPlayers()**:
   - Check `StartingPositionPlayerMapping.ContainsKey(STC)` → TRUE
   - Check `StartingPositionPlayerMapping.ContainsKey(MCR)` → TRUE
   - Update: `StartingPositionPlayerMapping[STC] = De Bruyne`
   - Update: `StartingPositionPlayerMapping[MCR] = Lopez`
   - Dictionary still has 11 positions ✓
7. **UpdateViewAfterSwap()**:
   - Read from model: `Model.StartingPositionPlayerMapping[STC]` → De Bruyne
   - Read from model: `Model.StartingPositionPlayerMapping[MCR]` → Lopez
   - Update views: `sourceView.SetPlayerData(De Bruyne)`
   - Update views: `targetView.SetPlayerData(Lopez)`
   - NO formation status changes (both stay `InUseForFormation=true`)

### Result:
- ✅ Players swapped positions
- ✅ Formation unchanged (still 11 positions)
- ✅ Model dictionary still has exactly 11 entries

---

## Flow 2: Swapping In-Formation with Not-In-Formation (Formation Change)

**Example:** Swap MCL (Neymar, in formation) with MC (empty, not in formation)

### Steps:
1. **User clicks** MCL (Neymar) - `InUseForFormation=true`
2. **SelectionSwapManager.SelectItem()**:
   - Store selected item
   - Call `View.ShowUsablePlayerItemViews()` - Shows ALL position views including MC
3. **User clicks** MC (empty) - `InUseForFormation=false`
4. **SelectionSwapManager.PerformSwap()**:
   - Detect: `sourceView.InUseForFormation=true`, `targetView.InUseForFormation=false`
   - **FORMATION CHANGE DETECTED** ← Critical point!

   **Step 4a: Change Formation FIRST (before data swap)**
   - Call `sourceView.SetInUseForFormation(false)` - MCL goes out
   - Call `targetView.SetInUseForFormation(true)` - MC goes in

   **Step 4b: Sync Model Dictionary**
   - Call `SyncModelFormationFromViews()`
   - Iterate all views where `InUseForFormation=true`
   - Collect positions: [GK, DCL, DC, DCR, ML, MC, MCR, MR, STCL, STC, STCR] (11 total, MCL removed, MC added)
   - Collect players: Get player from old dictionary for each position
   - Call `Model.SyncFormationFromViews(activePositions, positionPlayerMap)`

   **Step 4c: Model Rebuilds Dictionary**
   - `Model.StartingPositionPlayerMapping.Clear()`
   - Add 11 new positions including MC (with null or old player data)
   - Dictionary now has MC position with space for data ✓

   **Step 4d: Now Swap Data**
   - Call `SwapInModel(sourceData, targetData)`
   - Extract: sourcePos=MCL, targetPos=MC
   - Call `Model.SwapPlayers(MCL, MC, Neymar, null)`

5. **Model.SwapPlayers()**:
   - Check `StartingPositionPlayerMapping.ContainsKey(MC)` → TRUE (added in step 4c!)
   - Check `StartingPositionPlayerMapping.ContainsKey(MCL)` → FALSE (removed in step 4c!)
   - Update: `StartingPositionPlayerMapping[MC] = Neymar` ✓
   - MCL is not in dictionary, so ignore it ✓
   - Dictionary still has 11 positions ✓

6. **UpdateViewAfterSwap()**:
   - `formationAlreadyChanged=true` so skip formation logic
   - Read from model: `Model.StartingPositionPlayerMapping[MC]` → Neymar
   - Update views: `targetView.SetPlayerData(Neymar)`
   - Update views: `sourceView.SetPlayerData(null)` (MCL not in model anymore)

### Result:
- ✅ Neymar moved to MC position
- ✅ MCL position is now inactive (not in formation)
- ✅ MC position is now active (in formation)
- ✅ Model dictionary has exactly 11 entries (GK, DCL, DC, DCR, ML, MC, MCR, MR, STCL, STC, STCR)
- ✅ Formation visually changed

---

## Flow 3: Drag-and-Drop Formation Change

**Example:** Drag MCL (Neymar) to MC (empty)

### Steps:
1. **User starts dragging** MCL (Neymar)
2. **PlayerItemDragSupport** - Create drag data
3. **User drops on** MC (empty)
4. **PlayerItemDropSupport.CanReceiveDrop()** - Validates swap
5. **PlayerItemDropSupport.Drop()**:

   **Step 5a: Detect Formation Change**
   - Check: `sourceView.InUseForFormation != targetView.InUseForFormation`
   - If true → formation change detected

   **Step 5b: Change Formation FIRST (if detected)**
   - Call `dragSource.SetInUseForFormation(false)` - MCL goes out
   - Call `dropTarget.SetInUseForFormation(true)` - MC goes in

   **Step 5c: Sync Model Dictionary**
   - Call `SyncModelFormationFromViews()`
   - Iterate all views where `InUseForFormation=true`
   - Collect positions: [GK, DCL, DC, DCR, ML, MC, MCR, MR, STCL, STC, STCR] (11 total)
   - Collect players from old dictionary
   - Call `Model.SyncFormationFromViews(activePositions, positionPlayerMap)`
   - Dictionary now has MC position with space for data ✓

   **Step 5d: Now Swap Data**
   - Call `Controller.SwapPlayersDropped(dragData, dropData)`

6. **Controller.SwapPlayersDropped()**:
   - Extract positions from CORRECT view references:
     - For dragged: `dragData.DragSourceView.ParentPositionZoneView.tacticalPositionOption`
     - For target: `dropData.DropTargetView.ParentPositionZoneView.tacticalPositionOption` (NOT DragSourceView!)
   - Call `Model.SwapPlayers(MCL, MC, Neymar, null)`

7. **Model.SwapPlayers()**:
   - Check `StartingPositionPlayerMapping.ContainsKey(MC)` → TRUE (added in step 5c!)
   - Check `StartingPositionPlayerMapping.ContainsKey(MCL)` → FALSE (removed in step 5c!)
   - Update: `StartingPositionPlayerMapping[MC] = Neymar` ✓
   - MCL is not in dictionary, so ignore it ✓

8. **Controller.UpdateViewsAfterSwap()**:
   - Read fresh data from model
   - Update both views with `SetPlayerData()`

### Result:
- ✅ Neymar moved to MC position
- ✅ MCL position is now inactive (not in formation)
- ✅ MC position is now active (in formation)
- ✅ Model dictionary has exactly 11 entries
- ✅ Formation visually changed and model is in sync

---

## Flow 4: Saving Formation

**Example:** User presses 'S' to save

### Steps:
1. **User presses** 'S' key
2. **Controller.SaveTacticWithTimestamp()**:
   - Iterate all `view.startingPlayersViews`
   - Filter: Only include views where `InUseForFormation=true`
   - For each active view:
     - Get position from `ParentPositionZoneView.tacticalPositionOption`
     - Get player from `Model.StartingPositionPlayerMapping[position]`
     - Create `TacticalPosition` and add to `CurrentTactic.ActivePositions`
   - Generate filename from positions
   - Save tactic

### Result:
- ✅ Only the 11 active positions are saved
- ✅ Filename matches visual formation
- ✅ Player assignments are correct

---

## Flow 5: Clearing Players

**Example:** User presses 'C' to clear all players

### Steps:
1. **User presses** 'C' key
2. **Controller.ClearAllPlayers()**:
   - Iterate `Model.StartingPositionPlayerMapping`
   - Set all values to `null` (keep positions, remove players)
   - Call `View.RefreshAllViews()`
3. **View updates**:
   - All position views show empty
   - `InUseForFormation` states unchanged (formation preserved)

### Result:
- ✅ All players removed
- ✅ Formation intact (11 positions still active)
- ✅ Model dictionary still has 11 entries (all null)

---

## Critical Patterns

### Pattern 1: Formation Change Detection
```csharp
bool isFormationChange = false;
if (sourceView.InUseForFormation != targetView.InUseForFormation)
{
    isFormationChange = true;
}
```

### Pattern 2: Formation Change Sequence
```csharp
// 1. Change formation status FIRST
sourceView.SetInUseForFormation(!sourceView.InUseForFormation);
targetView.SetInUseForFormation(!targetView.InUseForFormation);

// 2. Sync model dictionary
SyncModelFormationFromViews();

// 3. THEN swap data
Model.SwapPlayers(...);
```

### Pattern 3: Reading Fresh Data After Swap
```csharp
// ALWAYS read from model after swap
if (Model.StartingPositionPlayerMapping.TryGetValue(position, out var player))
{
    view.SetPlayerData(player);
}
```

### Pattern 4: Keeping Dictionary at 11 Positions
```csharp
// NEVER directly add to StartingPositionPlayerMapping without removing old positions
// ALWAYS use SyncFormationFromViews() after formation changes
```

---

## Common Issues and Fixes

### Issue 1: "Data disappears after formation change"
**Cause:** Calling `Model.SwapPlayers()` before `SyncFormationFromViews()`
**Fix:** Always sync formation BEFORE swapping data

### Issue 2: "Dictionary has more than 11 positions"
**Cause:** Adding positions without removing old ones
**Fix:** Call `SyncFormationFromViews()` after every formation change

### Issue 3: "Formation name mismatch when saving"
**Cause:** Saving from model dictionary instead of from views
**Fix:** Only save positions where `view.InUseForFormation=true`

### Issue 4: "Duplicate players appearing"
**Cause:** Using stale drag data instead of reading from model
**Fix:** Always read fresh data from `Model.StartingPositionPlayerMapping` after swap

### Issue 5: "Target position returns TacticalPositionOption.None"
**Cause:** `PlayerItemDragData.TacticalPositionOption` only reads from `DragSourceView`
**Fix:** For target, read from `DropTargetView.ParentPositionZoneView.tacticalPositionOption`

---

## Verification Checklist

After any swap operation, verify:
- [ ] `Model.StartingPositionPlayerMapping.Count == 11`
- [ ] Number of views with `InUseForFormation=true` == 11
- [ ] All positions in model dictionary match positions with `InUseForFormation=true`
- [ ] All views display correct player data from model
- [ ] No data loss
- [ ] No duplicate players

---

## Completed Fixes

1. ✅ **Drag-and-Drop Formation Changes** (FIXED)
   - `PlayerItemDropSupport.Drop()` now detects formation changes
   - Calls `SyncModelFormationFromViews()` after `SetInUseForFormation`
   - Formation changes happen BEFORE data swap
   - Drag-and-drop now follows same pattern as click-to-swap

2. ✅ **Controller.SwapPlayersDropped()** (FIXED)
   - Fixed position extraction to use correct view references:
     - Dragged position from `DragSourceView.ParentPositionZoneView`
     - Target position from `DropTargetView.ParentPositionZoneView` (not DragSourceView!)
   - Added debug logging for verification

## Current Implementation Status

### ✅ Completed Features

1. **Unified Swap System** - Both drag-and-drop and click-to-swap use the same `Controller.SwapPlayers()` method
2. **Formation Change Detection** - Properly detects when swaps involve formation changes
3. **Model-View Sync** - `SyncModelFormationFromViews()` keeps dictionary at exactly 11 positions
4. **Proper Position Extraction** - Using `SourceTacticalPosition` and `TargetTacticalPosition` properties
5. **View Updates** - Targeted view updates after swaps for better performance

### 📋 Implementation Details

#### Key Classes and Their Responsibilities

**TacticBoardModel** (Pure Data Layer)
- `StartingPositionPlayerMapping` - Dictionary with exactly 11 positions (formation)
- `SubstitutesPlayersItems` - Observable list of substitutes
- `ReservePlayersItems` - Observable list of reserves
- `SwapPlayers()` - Performs data swap operation
- `SyncFormationFromViews()` - Rebuilds dictionary based on active formation
- `CompactSubstitutes()` - Removes null entries from substitutes
- `CompactReserves()` - Removes null entries from reserves

**TacticsBoardController** (Mediator)
- `SwapPlayers()` - UNIFIED swap logic for both drag-and-drop and click-to-swap
- `SyncModelFormationFromViews()` - Collects active positions from views and syncs model
- `UpdateViewsAfterSwap()` - Targeted view updates after swap
- `SetFormation()` - Changes formation and updates views

**TacticsPitchView** (View Layer)
- `startingPlayersViews` - Array of 24 position zone views (some inactive)
- `UpdateFormationZones()` - Shows/hides zones based on formation
- `UpdateStartingPlayers()` - Updates views from model mapping
- `ShowUsablePlayerItemViews()` - Shows all zones during selection
- `HideUnusedPlayerItemViews()` - Hides inactive zones after swap

**SelectionSwapManager** (Click-to-Swap)
- `HandleItemClicked()` - Manages click selection state
- `PerformSwap()` - Validates and triggers swap via `Controller.SwapPlayers()`
- `CanSwap()` - Validation logic for click swaps

**PlayerItemDropSupport** (Drag-and-Drop)
- `CanReceiveDrop()` - Validation logic for drag-and-drop
- `Drop()` - Triggers swap via `Controller.SwapPlayers()`

**PlayerItemDragData** (Data Transfer)
- `SourceTacticalPosition` - Reads position from `DragSourceView`
- `TargetTacticalPosition` - Reads position from `DropTargetView`
- `TacticalPositionOption` - DEPRECATED (only reads from DragSourceView)

### 🔄 Data Flow Patterns

#### Pattern A: Same-Formation Swap (No Formation Change)
```
User Action → SelectionSwapManager/DropSupport
    ↓
Controller.SwapPlayers()
    ↓ Detect: Both in formation OR both not in formation
    ↓
Model.SwapPlayers() - Direct dictionary swap
    ↓
UpdateViewsAfterSwap() - Targeted view refresh
```

#### Pattern B: Formation-Change Swap
```
User Action → SelectionSwapManager/DropSupport
    ↓
Controller.SwapPlayers()
    ↓ Detect: One in formation, one not
    ↓
Step 1: Change InUseForFormation flags FIRST
    sourceView.SetInUseForFormation(!current)
    targetView.SetInUseForFormation(!current)
    ↓
Step 2: SyncModelFormationFromViews()
    - Collect all views with InUseForFormation=true
    - Build new dictionary with exactly 11 positions
    - Clear old dictionary, populate new one
    ↓
Step 3: Model.SwapPlayers() - Swap data in new dictionary
    ↓
Step 4: CompactSubstitutes/Reserves if needed
    ↓
Step 5: UpdateViewsAfterSwap() - Targeted view refresh
```

### 🎯 Critical Implementation Rules

1. **Formation Changes MUST happen BEFORE data swaps**
   - Change `InUseForFormation` flags first
   - Call `SyncModelFormationFromViews()` to rebuild dictionary
   - Then swap data in the new dictionary structure

2. **Always use proper position extraction**
   - For source: Use `dragData.SourceTacticalPosition`
   - For target: Use `dragData.TargetTacticalPosition`
   - NEVER use deprecated `dragData.TacticalPositionOption` for target

3. **Dictionary must always have exactly 11 positions**
   - After formation change: Call `SyncFormationFromViews()`
   - After formation selection: Call `SetFormation()`
   - Verify count in debug logs

4. **View updates read from model**
   - NEVER cache player data in views
   - Always read fresh from `Model.StartingPositionPlayerMapping`
   - Views are just displays of model state

5. **Reserves and Substitutes compacting**
   - Substitutes: Can have nulls (padded to allowed count)
   - Reserves: Should NEVER have nulls (compact immediately)
   - Call compact methods after swaps involving these lists

### 🐛 Common Pitfalls to Avoid

❌ **DON'T:** Swap data before changing formation
✅ **DO:** Change formation → Sync model → Then swap data

❌ **DON'T:** Use `dragData.TacticalPositionOption` for target position
✅ **DO:** Use `dragData.TargetTacticalPosition` for target

❌ **DON'T:** Directly modify `StartingPositionPlayerMapping` during formation changes
✅ **DO:** Call `SyncFormationFromViews()` to rebuild dictionary

❌ **DON'T:** Clear player data in `SetInUseForFormation()`
✅ **DO:** Let `UpdateViewsAfterSwap()` handle data clearing

❌ **DON'T:** Call `RefreshAllViews()` after every swap
✅ **DO:** Use targeted `UpdateViewsAfterSwap()` for better performance

### 📝 Future Enhancements

1. **AutoPick Integration** - Verify AutoPick fills only the 11 positions in the formation
2. **Undo/Redo System** - Track swap history for undo functionality
3. **Formation Presets** - Save/load common formations with player assignments
4. **Drag Preview** - Show formation preview during formation-change drags
