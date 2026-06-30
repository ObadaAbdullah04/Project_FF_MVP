# Project FF MVP — Architecture & Flow Diagrams

This document visualizes the current structural solidity, connections, and workflows of **Project FF MVP**. It is designed to assist software engineers in reviewing the architecture, data pipelines, and scene management strategies implemented up to Phase 4.

---

## 1. Core Architecture & Dependency Layers

The project strictly adheres to a one-way dependency rule: `Architecture → Core → Data → UI/Hub/MiniGames`. 
- **Rule:** No file in `Core` imports from `UI` or `Hub`. No file in `Data` imports from `MiniGames`.

```mermaid
graph TD
    subgraph ArchitectureLayer [Architecture Layer]
        ServiceLocator
        ISaveSystem
        SceneLoader
        LocalizationManager
    end

    subgraph CoreLayer [Core Layer]
        GameManager
        DeviceRoleManager
        SessionTimer
        AppBootstrapper
    end

    subgraph DataLayer [Data Layer]
        InventoryData
        ChildProgressData
        MiniGameProfile
        GameResult
    end

    subgraph PresentationLayer [Presentation & Gameplay Layer]
        subgraph UI [Unified Core UI]
            MenuManager
            ParentDashboardUI
            ParentGateUI
            SessionLockScreenUI
        end
        subgraph HubEnvironment [Hub Environment]
            HubWorldManager
            ChunkController
            BuildingController
        end
        subgraph MiniGameEnvironment [MiniGame Environment]
            UniversalGameController
            BaseMiniGameManager
            Adapters[Specific Game Adapters]
        end
    end

    ArchitectureLayer --> CoreLayer
    CoreLayer --> DataLayer
    DataLayer --> PresentationLayer
```

---

## 2. Scene Architecture & Navigation (Unified Core UI)

As of Phase 4 (Step 6), additive UI scenes are **banned**. All core UI panels live in a single persistent `1_Core` scene managed by the `MenuManager`. The environments (Hub vs MiniGames) are cleanly transitioned using `SceneLoader` to prevent additive memory leaks or camera clashes.

```mermaid
graph TD
    subgraph PersistentBase [1_Core Scene - DontDestroyOnLoad]
        Bootstrapper[AppBootstrapper]
        Locator[ServiceLocator]
        Loader[SceneLoader]
        
        subgraph UnifiedUICanvas [Unified UI Canvas]
            Menu[MenuManager state machine]
            Role[RoleSelectionUI]
            Age[AgeEntryUI]
            Gate[ParentGateUI]
            Dash[ParentDashboardUI]
            Lock[SessionLockScreenUI]
        end
        Menu -->|Toggles Panels| Role
        Menu -->|Toggles Panels| Age
        Menu -->|Toggles Panels| Gate
        Menu -->|Toggles Panels| Dash
        Menu -->|Toggles Panels| Lock
    end

    subgraph EnvironmentScenes [Loaded exclusively via SceneLoader]
        Hub(Hub World Scene)
        MG1(Mini-Game: Cosmic Hopper)
        MG2(Mini-Game: Color Cube)
    end

    Bootstrapper -->|Requests UI State| Menu
    Hub -.->|Play Game Transition| Loader
    MG1 -.->|Return Transition| Loader
    MG2 -.->|Return Transition| Loader
    Loader -->|Unloads previous, Loads next| EnvironmentScenes
```

---

## 3. App Initialization & Bootstrapping Flow

The `AppBootstrapper` routes the initial launch directly through the `MenuManager` panel toggles based on role, age configuration, and session timer constraints.

```mermaid
stateDiagram-v2
    [*] --> AppBootstrapper
    AppBootstrapper --> CheckRole: Read DeviceRoleManager
    
    CheckRole --> RoleSelectionPanel: No Role Set
    RoleSelectionPanel --> AppBootstrapper: Set Role
    
    CheckRole --> ParentGatePanel: Role = Parent
    ParentGatePanel --> ParentDashboardPanel: PIN Valid
    ParentDashboardPanel --> HubWorldScene: Exit Dashboard
    
    CheckRole --> CheckAge: Role = Child
    CheckAge --> AgeEntryPanel: No Age Set
    AgeEntryPanel --> AppBootstrapper: Set Age
    
    CheckAge --> CheckSession: Age is Set
    CheckSession --> SessionLockPanel: Session Expired (via SessionTimer)
    CheckSession --> HubWorldScene: Session Valid
```

---

## 4. Mini-Game Data & Scoring Pipeline

The scoring system avoids generic engine over-engineering. Each mini-game has its own scoring/timing logic within its `Adapter`. The `UniversalGameController` serves strictly as an event receiver pipeline mapping `GameResult` data to the `ChildProgressData` system.

```mermaid
sequenceDiagram
    participant Adapter as Game Adapter (e.g., ColorCubeAdapter)
    participant UGC as UniversalGameController
    participant Manager as BaseMiniGameManager
    participant Data as Data Layer (Progress & Inventory)
    participant Save as ISaveSystem (GameManager)

    Note over Adapter: Game loop runs natively
    Adapter->>UGC: ReportDecisionTime(float seconds)
    Adapter->>Adapter: Computes custom Score & Currency
    Note over Adapter,UGC: End Game condition met
    Adapter->>UGC: CompleteGame(GameResult struct)
    UGC->>Manager: Pass GameResult
    
    Manager->>Data: Add Currency (InventoryData)
    Manager->>Data: Record GameResult (ChildProgressData)
    Manager->>Save: Trigger SaveAll()
    Manager->>UGC: Show HUD Summary View
```

---

## 5. Child User Core Loop

The psychological loop driving engagement and educational value. Instant positive feedback and ownership drive progression.

```mermaid
flowchart LR
    Hub(Hub World) --> |Tap Play Button| MiniGame(Play Mini-Game)
    MiniGame --> |Educational/Pedagogical Play| Earn(Earn Coins)
    Earn --> |Return Scene| Hub
    Hub --> |Spend Coins| Unlock(Unlock Map Chunk)
    Unlock --> |Place Building| Expand(Building Placed & Animated)
    Expand --> Hub
```
