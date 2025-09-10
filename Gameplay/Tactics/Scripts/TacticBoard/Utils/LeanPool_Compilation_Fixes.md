# LeanPool Integration - Compilation Fixes

## Summary of Fixed Errors

### 1. Event Access Errors in PlayerItemViewPoolManager
**Error**: 
```
CS0070: The event 'PlayerItemView.OnFormationStatusChanged' can only appear on the left hand side of += or -= (except when used from within the type 'PlayerItemView')
```

**Fix**: 
- Removed event cleanup code from `PlayerItemViewPoolManager.ResetViewState()`
- Event cleanup is properly handled within `PlayerItemView.CleanupForPool()` method
- Added comment explaining that events cannot be accessed from outside the class

### 2. LeanPool.GetPool Method Error in LeanPoolTestHelper
**Error**:
```
CS0117: 'LeanPool' does not contain a definition for 'GetPool'
```

**Fix**: 
- Replaced `LeanPool.GetPool()` with `LeanGameObjectPool.TryFindPoolByPrefab()`
- Updated `ShowPoolStatistics()` method to use the correct LeanPool API
- Added pool properties display (Capacity, Preload) when pool exists

### 3. Transform/PlayerItemView Conversion Errors
**Error**:
```
CS1503: Argument 1: cannot convert from 'UnityEngine.Transform' to 'UltimateFootballSystem.Gameplay.Tactics.PlayerItemView'
```

**Files Fixed**:
- `BoardInitializationManager.cs` (2 locations)
- `BoardViewRefreshManager.cs` (1 location)

**Fix**: 
- Changed `_controller.playerItemViewPrefab` to `_controller.playerItemViewPrefab.GetComponent<PlayerItemView>()`
- This correctly extracts the PlayerItemView component from the Transform prefab reference

## Code Changes Made

### PlayerItemViewPoolManager.cs
```csharp
// OLD - Caused compilation error
if (view.OnFormationStatusChanged != null)
{
    System.Delegate[] delegates = view.OnFormationStatusChanged.GetInvocationList();
    foreach (var d in delegates)
    {
        view.OnFormationStatusChanged -= (System.Action)d;
    }
}

// NEW - Proper approach
// Event cleanup is handled within PlayerItemView's CleanupForPool method
// We cannot access events directly from outside the class
```

### LeanPoolTestHelper.cs
```csharp
// OLD - Method doesn't exist
var pool = LeanPool.GetPool(testPrefab.gameObject, false);

// NEW - Using correct API
LeanGameObjectPool pool = null;
bool poolExists = LeanGameObjectPool.TryFindPoolByPrefab(testPrefab.gameObject, ref pool);
```

### BoardInitializationManager.cs & BoardViewRefreshManager.cs
```csharp
// OLD - Type mismatch
PlayerItemViewPoolManager.SpawnPlayerItemView(_controller.playerItemViewPrefab, parent);

// NEW - Correct component extraction
PlayerItemViewPoolManager.SpawnPlayerItemView(_controller.playerItemViewPrefab.GetComponent<PlayerItemView>(), parent);
```

## Verification Steps

1. **Syntax Check**: All files now have correct C# syntax
2. **API Usage**: Using proper LeanPool API methods
3. **Type Safety**: Correct type conversions for all method calls
4. **Event Handling**: Proper event cleanup within the owning class

## Key Learnings

1. **Event Access**: C# events can only be accessed directly from within the declaring class
2. **LeanPool API**: Use `LeanGameObjectPool.TryFindPoolByPrefab()` instead of non-existent `GetPool()`
3. **Component References**: Always extract components when the field is declared as Transform but component is needed
4. **Pool Management**: LeanPool handles most pooling logic automatically, minimal manual intervention needed

## Next Steps

The compilation errors have been resolved. The system should now:
- Compile without errors
- Properly instantiate/destroy objects using LeanPool
- Provide better performance through object pooling
- Maintain proper memory management with event cleanup