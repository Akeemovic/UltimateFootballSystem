# LeanPool Integration for Ultimate Football System

## Overview

This document describes the integration of LeanPool for optimized object instantiation and destruction in the Ultimate Football System, specifically for `PlayerItemView` objects in the tactics board system.

## What Changed

### 1. PlayerItemView Class
- **Added**: `IPoolable` interface implementation
- **Added**: `OnSpawn()` and `OnDespawn()` methods for pool lifecycle management
- **Added**: `ResetPooledState()` and `CleanupForPool()` private methods
- **Added**: `using Lean.Pool;` import

### 2. PlayerItemViewPoolManager (NEW)
- **Created**: Static utility class for managing PlayerItemView pooling
- **Methods**:
  - `SpawnPlayerItemView()` - Spawns objects using LeanPool
  - `DespawnPlayerItemView()` - Despawns objects back to pool
  - `DespawnAllInContainer()` - Batch despawn all objects in a container
  - `ClearAllPools()` - Cleanup method (delegates to LeanPool)

### 3. BoardInitializationManager
- **Updated**: `InitializeSubstitutePlayers()` to use LeanPool
- **Updated**: `InitializeReservePlayers()` to use LeanPool
- **Added**: `using Lean.Pool;` import

### 4. BoardViewRefreshManager
- **Updated**: `RefreshReserveViews()` to use LeanPool
- **Removed**: Custom pooling logic (replaced with LeanPool)
- **Simplified**: `Cleanup()` method
- **Added**: `using Lean.Pool;` import

### 5. TacticsBoardController
- **Updated**: Drag info view instantiation to use LeanPool
- **Added**: `using Lean.Pool;` import

## Benefits

### Performance Improvements
- **Reduced GC Pressure**: Objects are reused instead of constantly created/destroyed
- **Faster Instantiation**: Pooled objects spawn much faster than `Object.Instantiate()`
- **Memory Efficiency**: Fewer memory allocations and deallocations
- **Smoother Gameplay**: Reduced frame drops during object spawning

### Code Simplification
- **Unified Pooling**: All PlayerItemView pooling now uses the same system
- **Automatic Cleanup**: LeanPool handles pool management automatically
- **Memory Leak Prevention**: Proper event unsubscription in pool cleanup

## Usage Examples

### Spawning Objects
```csharp
// Old way
GameObject obj = Object.Instantiate(prefab, parent);
PlayerItemView view = obj.GetComponent<PlayerItemView>();

// New way (LeanPool)
PlayerItemView view = PlayerItemViewPoolManager.SpawnPlayerItemView(prefab, parent);
```

### Despawning Objects
```csharp
// Old way
Object.Destroy(view.gameObject);

// New way (LeanPool)
PlayerItemViewPoolManager.DespawnPlayerItemView(view);
```

### Batch Operations
```csharp
// Clear all objects in a container
PlayerItemViewPoolManager.DespawnAllInContainer(container);
```

## Testing

### LeanPoolTestHelper
A test helper script is provided for testing the pooling system:

- **File**: `LeanPoolTestHelper.cs`
- **Keys**:
  - `Space`: Spawn test objects
  - `Backspace`: Despawn test objects
  - `Delete`: Clear all pools
- **Context Menu**: Right-click methods for manual testing

### Verification Steps
1. Attach `LeanPoolTestHelper` to a GameObject in your scene
2. Assign a `PlayerItemView` prefab to the test helper
3. Use keyboard shortcuts or context menu to test pooling
4. Monitor the Console for pool statistics and verification logs

## Best Practices

### When to Use
- Objects that are frequently created/destroyed (PlayerItemViews, UI elements)
- Objects with expensive initialization (complex UI setups)
- Temporary objects (drag indicators, tooltips, effects)

### When NOT to Use
- Unique objects that persist for the entire scene lifetime
- Objects that are created once and never destroyed
- Very simple objects where pooling overhead exceeds benefits

## Troubleshooting

### Common Issues
1. **Objects not resetting properly**: Check `OnSpawn()` and `OnDespawn()` implementations
2. **Memory leaks**: Ensure proper event unsubscription in `CleanupForPool()`
3. **Pool not found errors**: Verify prefab references are correct

### Debug Information
- Use `LeanPoolTestHelper.ShowPoolStatistics()` to monitor pool states
- Check Unity Console for LeanPool debug messages
- Monitor memory usage in Unity Profiler

## Future Improvements

### Potential Extensions
- **Auto-warming**: Pre-populate pools with common objects
- **Pool size limits**: Configure maximum pool sizes per prefab
- **Statistics tracking**: Monitor pool usage for optimization
- **Custom pool strategies**: Implement specific pooling logic for different object types

### Integration Points
- Consider pooling other frequently instantiated objects (effects, particles)
- Integrate with save/load system for proper pool state management
- Add pooling support for multiplayer synchronization