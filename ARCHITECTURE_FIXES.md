# Architecture Fixes

## Executive Summary

This document is a ready-to-fix guide for improving the scalability and maintainability of the Unity project.

The current codebase is functional, but it is starting to show classic growth limits for a Unity prototype that has become a larger game project:

- all scripts compile into one assembly because there are no `.asmdef` boundaries
- several responsibilities are duplicated across managers and UI components
- some systems still rely on direct singleton calls and manual scene wiring
- a few minigame classes have grown into monolithic runtime UI builders
- prefab usage exists, but it is not yet consistently used as the primary composition model

The goal of the fixes below is not to rewrite the project. The goal is to reduce coupling, make features easier to extend, and give the team a clearer structure for adding more scenes, minigames, UI screens, and narrative content.

## Fix Catalog

### 1. Assembly Definitions

**Problem**

The project currently has no `.asmdef` files. That means every script compiles into a single assembly, which increases compile times, hides architecture violations, and makes it difficult to test or isolate systems.

**How to fix**

1. Create an `asmdef` for core domain code, for example under `Assets/Scripts/Core`.
2. Create separate `asmdef` files for `Data`, `UI`, `Managers`, and `Gameplay` or `Minigames`.
3. Keep `Core` and `Data` as the lowest-level assemblies.
4. Make `UI` depend on `Core` and `Data`, but not on gameplay-specific internals.
5. Make `Managers` depend on `Core`, `Data`, and only the UI types they truly orchestrate.
6. Move any shared helpers into a small `Shared` or `Common` assembly only if a real reuse case appears.
7. Add test assemblies later once the runtime boundaries are in place.

**Expected Result**

- compile times improve as the project grows
- illegal cross-layer references become easier to spot
- future refactors are smaller and safer
- unit testing becomes more realistic

---

### 2. Screen System Duplication

**Problem**

The project contains both `ScreenManager` and `ScreenRouter`, which appear to solve the same screen-switching problem. This creates ambiguity about the owning abstraction and makes future UI work harder to reason about.

**How to fix**

1. Choose one screen orchestration class as the canonical implementation.
2. Keep the class that best matches the current scene flow and remove the other from active use.
3. Update scene references so all screen transitions route through the chosen class.
4. Search the project for any direct calls into the deprecated screen controller and replace them.
5. If needed, extract shared logic into a small `IScreenNavigator` interface or a helper class.
6. Document which component owns desk-shell visibility, screen transitions, and screen lookup.

**Expected Result**

- there is one obvious path for changing screens
- screen behavior is easier to debug
- onboarding for new contributors becomes simpler

---

### 3. Manager-to-Manager Tight Coupling

**Problem**

Several systems still call each other directly through singleton accessors such as `GameManager.I` and `UIManager.I`. This works for a small project, but it increases coupling and makes behavior harder to test or reuse.

**How to fix**

1. Identify all direct singleton calls between managers.
2. Replace the most important runtime links with event-driven notifications where possible.
3. Keep a small number of orchestration references in scene-level controllers only.
4. Prefer serialized dependencies in composition roots over global lookups in feature code.
5. Reserve singletons for truly global services, and keep that list short.
6. Make sure each manager has one clear responsibility and does not become a proxy for other systems.

**Expected Result**

- fewer hidden dependencies
- easier feature isolation
- lower risk when changing UI or flow logic
- better testability for individual systems

---

### 4. UI Responsibility Overlap

**Problem**

The UI layer has already been refactored once to separate player stats from the speaker strip and transition overlay, but the project still contains signs of responsibility overlap and legacy usage patterns. That makes the UI architecture look cleaner than it actually is.

**How to fix**

1. Keep `PlayerPanelView` responsible only for player state presentation.
2. Keep `UIManager` focused only on shared presentation elements that are genuinely global.
3. Remove any remaining duplicate UI writes to the same labels, fills, or indicators.
4. Make all screen-specific UI behavior belong to the relevant screen component.
5. Avoid letting managers directly mutate screen internals unless the screen is explicitly acting as a view adapter.
6. Review inspector wiring so each view owns only the widgets it needs.

**Expected Result**

- no UI element is written by two different systems
- ownership of each visual element becomes obvious
- future UI changes stop causing side effects in unrelated screens

---

### 5. Monolithic Minigame Classes

**Problem**

Some minigame managers, especially the sculpting-related ones, have grown into large classes that combine gameplay logic, runtime UI construction, rendering-related work, and state handling. This is the clearest scalability risk in the project.

**How to fix**

1. Split minigame logic into smaller components by responsibility.
2. Move runtime UI construction into prefabs whenever possible.
3. Extract repeated button, text, and HUD construction into reusable view prefabs or helper components.
4. Move algorithmic or pixel-processing logic into dedicated utility classes or services.
5. Keep the minigame controller focused on state transitions and player input coordination.
6. If a feature is used by multiple minigames, extract it before adding the next minigame.

**Expected Result**

- each minigame class becomes easier to read and modify
- reuse increases across minigames
- future bugs are easier to localize
- adding a new minigame stops requiring copy-paste from an old one

---

### 6. Prefab-First Composition

**Problem**

Prefab usage exists and is already helpful in UI list/card rendering, but some systems still build UI and objects directly in code. That weakens prefab-first composition and makes scene setup harder to maintain.

**How to fix**

1. Keep reusable cards, list items, and modal elements as prefabs.
2. Move repeated runtime-created UI pieces into prefab assets where they can be authored visually.
3. Use code to configure prefabs, not to recreate them from scratch unless the object is truly dynamic.
4. Normalize prefab naming and folder placement so related prefabs live in one obvious location.
5. Audit the `Prefabs` folder for duplicates, legacy copies, and oddly named assets.
6. Prefer one canonical prefab for each reusable UI concept.

**Expected Result**

- scene setup becomes clearer
- UI becomes easier to iterate in the editor
- prefab reuse improves consistency across screens
- code is smaller and more focused on behavior

---

### 7. Scene Flow Ownership

**Problem**

The game flow is already centralized in a controller, which is good, but the controller still holds many serialized references and acts as a large orchestration hub. That is acceptable for now, but it will become brittle as the number of scenes and branching paths grows.

**How to fix**

1. Keep the orchestration concept, but reduce the number of direct references in the main controller.
2. Move screen-specific setup into the screen components themselves.
3. Treat the flow controller as a coordinator, not as a repository of all game knowledge.
4. Move repeated day/progression logic into a dedicated progression service if the flow expands further.
5. Keep `TaskChannelSO` as the additive-scene bridge for task completion.
6. Add a clear handoff contract between main flow and task scenes.

**Expected Result**

- the main flow class stays readable
- scene transitions remain explicit
- task scenes stay decoupled from the main scene
- branching narrative paths become easier to add

---

### 8. Legacy and Stub Cleanup

**Problem**

Some managers are still stubs or appear to be transitional code. Leaving these in the active architecture without marking them clearly increases maintenance risk and makes it hard to know what is production-ready.

**How to fix**

1. Identify all stub managers and placeholder methods.
2. Decide whether each stub is active, deprecated, or intentionally postponed.
3. Remove dead code paths that are no longer used.
4. Mark temporary code clearly and keep the list of temporary items short.
5. If a stub is required for the current build, document its intended replacement.

**Expected Result**

- fewer false assumptions during maintenance
- clearer ownership of incomplete features
- easier planning for the next iteration

## Definition of Done

A fix is complete when all of the following are true:

- the change has a single owning system and no competing duplicate path
- new code does not introduce fresh cross-layer dependencies without reason
- scene references are explicit and understandable in the inspector
- the project continues to run in-editor after the change
- the updated system is easier to extend than the previous version
- any related TODOs or temporary notes are either resolved or intentionally documented

## Priority List

### Quick Wins

1. Remove or disable the duplicate screen orchestration path.
2. Clean up prefab naming and folder placement.
3. Document which managers are active, stubbed, or legacy.
4. Audit UI views so each element has one owner.

### Medium Effort

1. Add `.asmdef` files for `Core`, `Data`, `UI`, and `Managers`.
2. Reduce direct singleton usage in feature code.
3. Move repeated UI construction into prefabs.
4. Tighten scene flow boundaries around the main controller.

### Architecture Refactor

1. Split the monolithic minigame managers into smaller units.
2. Extract reusable gameplay and rendering helpers from minigame code.
3. Introduce more formal boundaries between gameplay, flow, and presentation.
4. Add test assemblies after the runtime boundaries are in place.

## Suggested Ownership Order

If the team wants the best return on effort, this is the recommended sequence:

1. Assembly definitions
2. Screen system deduplication
3. Prefab normalization
4. UI ownership cleanup
5. Minigame decomposition
6. Further flow/service extraction

## Notes for Contributors

- Keep changes small and reversible.
- Prefer removing ambiguity before adding new features.
- If two systems seem to own the same behavior, that is a sign to stop and define one owner.
- When in doubt, keep orchestration thin and push behavior into focused components.
