# Project FF MVP — Agent Guide

This guide defines the style guidelines, behavioral constraints, and general instructions for AI agents working on this project. Refer to [PLAN.md](file:///O:/Unity%20Projects/Project_FF_MVP/PLAN.md) for the product roadmap, user flows, and current development status.

---

## Code Quality Rules (Must Follow)

### 1. Deprecation-first, not deletion-first
Old code gets `[Obsolete]` with a message pointing to the replacement. Remove only after the replacement is verified working. No silent breaks.

### 2. Compilation checkpoint after every step
After each step, verify the project compiles AND Cosmic Hopper + Hub scenes load without errors. No step leaves the project in a broken state.

### 3. Single-writer rule per file
A file is refactored in exactly one step then frozen. Check `PLAN.md` before editing — if a file is listed in a different step's refactor list, do not touch it.

### 4. Interface contracts before implementation
Define structs/interfaces (`GameResult`, result-reporting API) before touching any adapter code. Both sides implement against an agreed contract.

### 5. Dependency direction
```
Architecture → Core → Data → UI
Architecture → Core → Data → Hub
Architecture → Core → Data → MiniGames
```
No file in Core imports from UI or Hub. No file in Data imports from MiniGames.

### 6. One responsibility per new file
Each new file does exactly one thing. If a file needs "and" in its description, split it.

### 7. Dead-code check before removal
Before deleting any file, grep for remaining references across the entire project.

### 8. No blind deletions
Everything in the "Remove" section of `PLAN.md` must be confirmed unreferenced first.

---

## Foundational Principles

1. **Voice + icons over text** — kids under 6 may not read. Every instruction must work without text.
2. **Feedback is fuel** — every action (coin earned, chunk unlocked, building placed) must have instant positive feedback.
3. **Forced variety** — each chunk demands games from multiple categories so the child learns across domains.
4. **Ownership drives retention** — visible progress makes the child want to come back and expand.
5. **Parent trust is the product** — without safety controls, the app won't be installed.

---

## What NOT to Do

- Do not add gameplay complexity before the core loop works end-to-end.
- Do not build multiple mini-games until the data pipeline is solid.
- Do not invest in animations, audio, or polish until the playable loop is solid and Cloud integrations are stable (Phase 6).
- Do not ship without a parent gate — it is a hard requirement.
- Do not add text-dependent UI. Assume the child cannot read.
- Do not start Phase 6 (character, juice, UX) until Phase 5 is tested and stable.
- Do not edit a file across multiple steps (single-writer rule).
- Do not delete code before its replacement is verified.

### 9. UI & Scene Architecture (The "Core UI" Standard)
- **NO ADDITIVE UI SCENES:** Do NOT use SceneManager.LoadSceneAdditively for menus. All UI panels (Parent Gate, Dashboard, Session Lock, Age Entry) exist in a single Canvas inside the persistent 1_Core scene.
- **UI State Machine:** Toggle menus exclusively using Project.UI.MenuManager.Instance.ShowPanelName().
- **Environment Transitions:** Hub and MiniGames are strictly transitioned using SceneLoader.Instance.TransitionToScene(). This guarantees the previous environment is unloaded, preventing the "Additive Stack of Death" and camera/audio clashes.
- **Brittle Strings:** Do NOT use UIHelper.FindChildButton("StringName"). Use direct `[SerializeField] private Button` assignments.