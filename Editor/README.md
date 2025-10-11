# UltimateFootballSystem Editor Tools

This folder contains all editor-only tools and windows for managing the Ultimate Football System.

## Database Manager

A comprehensive tool for managing football data definitions.

### Access the Tool

**Main Window:**
- `Tools > UltimateFootballSystem > Database Manager`

**Direct Tab Access:**
- `Tools > UltimateFootballSystem > Database Manager > Welcome`
- `Tools > UltimateFootballSystem > Database Manager > Setup`
- `Tools > UltimateFootballSystem > Database Manager > Definitions`
- `Tools > UltimateFootballSystem > Database Manager > Tools`

### Tabs Overview

#### 1. Welcome Tab
- Quick introduction to the Database Manager
- Quick actions to create or open databases
- Recent databases (coming soon)

#### 2. Setup Tab
- Select or create a Football Database asset
- View database information and metadata
- Statistics overview
- Validation status

#### 3. Definitions Tab
Sub-tabs for managing different entity types:
- **Roles** - Manage TacticalRoleDefinitions
- **Formations** - Coming soon

#### 4. Tools Tab
Sub-tabs for testing and maintenance:
- **Test** - Database integrity testing and debugging
- **Validation** - Advanced validation tools (coming soon)
- **Import/Export** - Data import/export utilities (coming soon)

## Getting Started

1. Open the Database Manager: `Tools > UltimateFootballSystem > Database Manager`
2. Create a new database or select an existing one in the Setup tab
3. Add and manage definitions in the Definitions tab
4. Use the Tools tab to test and validate your database

## File Structure

```
Editor/
├── DatabaseManager/
│   └── DatabaseEditorWindow.cs      # Main editor window
├── UI/                              # UI utilities
└── UltimateFootballSystem.Editor.asmdef  # Assembly definition
```

## Future Enhancements

- [ ] Role definition creation UI
- [ ] Formation template management
- [ ] Country/Club/Competition management
- [ ] Import/Export from JSON
- [ ] Advanced validation rules
- [ ] Recent databases tracking
- [ ] Batch operations
- [ ] Visual relationship editor

---

*Last updated: 2025-10-10*
