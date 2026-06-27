# Project FF — Architecture Diagrams


## 1. Scene Flow

```mermaid
flowchart TD
    Bootstrap["0_Bootstrap.unity<br/>AppBootstrapper"] -->|LoadSceneAdditive| Core["1_Core.unity<br/>(Persistent Layer)"]

    Core -->|Contains DontDestroyOnLoad| GM[GameManager]
    Core -->|Contains DontDestroyOnLoad| SL[SceneLoader]
    Core -->|Contains DontDestroyOnLoad| LM[LocalizationManager]
    Core -->|Contains DontDestroyOnLoad| DRM[DeviceRoleManager]
    Core -->|Contains DontDestroyOnLoad| LSS[LocalSaveSystem]

    Bootstrap -->|DetermineNextScene| HubScene["2_HubWorld.unity"]
    Bootstrap -->|DetermineNextScene| PGScene["Parent Gate.unity"]
    Bootstrap -->|DetermineNextScene| PDScene["ParentDashboard.unity"]

    PGScene -->|Parent PIN valid| PDScene
    PGScene -->|Child role selected| HubScene

    HubScene -->|Settings gear button| PGScene
    PDScene -->|Back to game| HubScene
    PDScene -->|Switch to player mode| HubScene

    HubScene -->|Tap unlocked building| MiniGame["_CosmicHopper.unity<br/>(non-additive, replaces all)"]
    MiniGame -->|CompleteGame| HubScene

    style Bootstrap fill:#f9f,stroke:#333
    style Core fill:#bbf,stroke:#333
    style MiniGame fill:#bfb,stroke:#333
```

---

## 2. Core Class Dependencies

```mermaid
classDiagram
    class ISaveSystem {
        <<Interface>>
        +Initialize(Action onComplete)
        +SaveInventory(InventoryData, Action)
        +LoadInventory(InventoryData, Action)
    }

    class LocalSaveSystem {
        +Initialize()
        +SaveInventory()
        +LoadInventory()
    }

    class GameManager {
        +Instance
        -InventoryData _inventoryData
        -ISaveSystem _saveSystem
        +SaveGame()
    }

    class ServiceLocator {
        +Register&lt;T&gt;(T)$
        +Get&lt;T&gt;()$ T
    }

    class TweenableMonoBehaviour {
        <<Abstract>>
        #OnDestroy()
    }

    class CameraUtility {
        +ResolveCamera(Camera, Component)$ Camera
    }

    class InventoryData {
        -int SoftCurrency
        -List~string~ UnlockedChunkIds
        +AddCurrency()
        +TrySpendCurrency()
        +UnlockChunk()
        +HasUnlockedChunk()
        +OnCurrencyChanged
        +OnChunkUnlocked
    }

    class InventorySnapshot {
        +int softCurrency
        +List~string~ unlockedChunkIds
        +FromInventory(InventoryData)
        +ApplyTo(InventoryData)
    }

    class GameSceneConfig {
        +HubSceneName
        +CoreSceneName
        +ParentGateSceneName
        +ParentDashboardSceneName
    }

    class HubWorldManager {
        -InventoryData _inventoryData
        -ChunkController[] _chunks
        -HubCameraController _cameraController
        -HubInputHandler _inputHandler
        -UnlockConfirmationBubble _activeBubble
        +HandleChunkUnlocked(string)
    }

    class ChunkController {
        +Initialize(InventoryData)
        +PlayUnlockAnimation()
        -HandleGlobalChunkUnlocked()
    }

    class HubCameraController {
        -Camera _hubCamera
        -bool _isSequenceRunning
        +PlayUnlockSequence(Transform, Action)
        +ResetCamera()
        -HandleCameraDrag()
    }

    class HubInputHandler {
        +OnInteractableTapped
    }

    class UIHelper {
        +IsPointerOverUI()$
    }

    class ParentGateUI {
        -GameSceneConfig _sceneConfig
        -PinValidationView _pinValidation
        +SelectParentRole()
        +SelectChildRole()
        +OnCancelPressed()
    }

    class PinValidationView {
        -PinKeypadUI _keypad
        +OnPinValidated
        +OnPinRejected
        +ResetInput()
        +HideError()
        -HandlePinSubmitted(string)
    }

    class PinKeypadUI {
        +OnPinSubmitted
        +OnInputChanged
        +ResetInput()
    }

    class BaseMiniGameManager {
        #InventoryData _inventoryData
        #GameSceneConfig _sceneConfig
        +StartGame()
        +CompleteGame(int)
    }

    class UniversalGameController {
        -GameRuleConfig _ruleConfig
        -IGameRule _gameRule
        +ReportScore()
        +ReportPlayerDeath()
    }

    class IGameRule {
        <<Interface>>
        +IsGameOver
        +CalculateReward()
    }

    GameManager --> ISaveSystem : uses
    GameManager --> ServiceLocator : registers/resolves
    LocalSaveSystem ..|> ISaveSystem
    GameManager --> InventoryData : owns
    InventorySnapshot ..> InventoryData : serializes
    LocalSaveSystem --> InventorySnapshot : uses

    ChunkController --|> TweenableMonoBehaviour
    BuildingController --|> TweenableMonoBehaviour
    UnlockConfirmationBubble --|> TweenableMonoBehaviour

    HubCameraController --> CameraUtility : resolves camera
    HubInputHandler --> CameraUtility : resolves camera

    HubWorldManager --> InventoryData : reads/writes
    HubWorldManager --> ChunkController : owns[]
    HubWorldManager --> HubCameraController : orchestrates
    HubWorldManager --> HubInputHandler : listens

    ChunkController ..> InventoryData : listens events
    HubCameraController ..> UIHelper : uses
    HubInputHandler ..> UIHelper : uses

    ParentGateUI --> PinValidationView : delegates
    PinValidationView --> PinKeypadUI : owns
    PinValidationView ..> DeviceRoleManager : validates

    BaseMiniGameManager --> InventoryData : writes
    BaseMiniGameManager --> GameSceneConfig : reads
    UniversalGameController --|> BaseMiniGameManager
    UniversalGameController --> IGameRule : delegates
```

---

## 3. Save / Load Lifecycle

```mermaid
sequenceDiagram
    participant Player
    participant GameManager
    participant LocalSaveSystem
    participant InventoryData as InventoryData SO
    participant ChunkController
    participant CoinsDisplayUI

    Note over GameManager,CoinsDisplayUI: GAME START
    GameManager->>LocalSaveSystem: Initialize()
    LocalSaveSystem-->>GameManager: onComplete
    GameManager->>LocalSaveSystem: LoadInventory(inventory, null)
    LocalSaveSystem->>InventoryData: ResetData() + AddCurrency() + UnlockChunk()
    InventoryData-->>CoinsDisplayUI: OnCurrencyChanged
    InventoryData-->>ChunkController: OnChunkUnlocked (per chunk)

    Note over GameManager,CoinsDisplayUI: DURING PLAY
    Player->>HubWorldManager: Tap locked chunk
    HubWorldManager->>InventoryData: TrySpendCurrency(cost)
    InventoryData-->>CoinsDisplayUI: OnCurrencyChanged
    HubWorldManager->>InventoryData: UnlockChunk(id)
    InventoryData-->>ChunkController: OnChunkUnlocked
    ChunkController->>ChunkController: PlayUnlockAnimation()
    InventoryData-->>HubWorldManager: HandleChunkUnlocked
    HubWorldManager->>HubCameraController: PlayUnlockSequence()

    Note over GameManager,CoinsDisplayUI: SAVE TRIGGERS
    Player->>GameManager: OnApplicationPause / OnApplicationQuit
    GameManager->>LocalSaveSystem: SaveInventory(inventory, null)
    LocalSaveSystem->>LocalSaveSystem: InventorySnapshot.FromInventory()
    LocalSaveSystem->>LocalSaveSystem: PlayerPrefs.Save()

    Player->>UniversalGameController: CompleteGame(coins)
    UniversalGameController->>InventoryData: AddCurrency(coins)
    UniversalGameController->>GameManager: SaveGame()
    GameManager->>LocalSaveSystem: SaveInventory(inventory, null)
```

---

## 4. Chunk Unlock Flow (Detail)

```mermaid
sequenceDiagram
    participant Bubble as UnlockConfirmationBubble
    participant HWM as HubWorldManager
    participant Inv as InventoryData
    participant Chunk as ChunkController
    participant Camera as HubCameraController
    participant Input as HubInputHandler

    Player->>Bubble: Tap confirm button
    Bubble->>HWM: onConfirm(chunk)
    HWM->>Inv: TrySpendCurrency(cost)
    alt Insufficient coins
        Inv-->>HWM: false
        HWM->>Bubble: PlayValidationError()
        Bubble->>Bubble: Shake + show rejection text
    else Enough coins
        Inv-->>HWM: true
        HWM->>Inv: UnlockChunk(chunkId)
        Inv-->>Chunk: OnChunkUnlocked(event)
        Chunk->>Chunk: PlayUnlockAnimation()
        HWM->>Input: enabled = false
        HWM->>Camera: enabled = false
        HWM->>Camera: PlayUnlockSequence(target, callback)
        Camera->>Camera: Pan to chunk, hold
        Note over Camera: Stores _resetFocusPosition
        Camera->>HWM: callback
        HWM->>Input: enabled = true
        HWM->>Camera: enabled = true
    end
```

---

## 5. Namespace Organization

```mermaid
flowchart LR
    subgraph Architecture["Project.Architecture"]
        ISaveSystem
        NetworkDispatcher
        ServiceLocator
        TweenableMonoBehaviour
        CameraUtility
    end

    subgraph Core["Project.Core"]
        GameManager
        SceneLoader
        AppBootstrapper
        DeviceRoleManager
        LocalizationManager
        LocalSaveSystem
        FirebaseManager
    end

    subgraph Data["Project.Data"]
        InventoryData
        InventorySnapshot
        GameSceneConfig
        ChildSessionData
        LocalizationData
    end

    subgraph Hub["Project.Hub"]
        HubWorldManager
        ChunkController
        BuildingController
        HubCameraController
        HubInputHandler
    end

    subgraph MiniGames["Project.MiniGames"]
        BaseMiniGameManager
        UniversalGameController
    end

    subgraph Rules["Project.MiniGames.Rules"]
        IGameRule
        GameRuleConfig
        ScoreTargetRule
        SurvivalRule
        StepRule
    end

    subgraph UI["Project.UI"]
        ParentGateUI
        ParentDashboardUI
        PinKeypadUI
        PinValidationView
        CoinsDisplayUI
        UnlockConfirmationBubble
        UniversalMiniGameHUD
        LocalizedRTLText
        UIHelper
    end

    Hub --> Architecture
    Hub --> Data
    Hub --> UI
    MiniGames --> Data
    MiniGames --> UI
    MiniGames --> Rules
    UI --> Core
    UI --> Data
    Core --> Architecture
    Core --> Data
```
