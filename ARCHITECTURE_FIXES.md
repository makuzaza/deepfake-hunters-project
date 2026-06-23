# Architecture Fixes

This file tracks implementation status for the architecture cleanup plan.

## Status

- DONE: 1. Assembly Definitions
- DONE: 2. Screen System Duplication
- DONE: 3. Manager-to-Manager Tight Coupling (high-priority singleton couplings)
- TODO: 4. UI Responsibility Overlap
- TODO: 5. Monolithic Minigame Classes
- TODO: 6. Prefab-First Composition
- TODO: 7. Scene Flow Ownership
- TODO: 8. Legacy and Stub Cleanup

## Notes

### 1. Assembly Definitions (DONE)

Implemented asmdef boundaries and references for:

- Puppeteer.Core
- Puppeteer.Data
- Puppeteer.Events
- Puppeteer.UI
- Puppeteer.Managers
- Puppeteer.Player
- Puppeteer.Minigame1

### 2. Screen System Duplication (DONE)

- Canonical orchestration path is ScreenManager.
- ScreenRouter converted to a compatibility adapter that forwards to ScreenManager.
- New and existing routing should use ScreenManager as the owning abstraction.

### 3. Manager-to-Manager Tight Coupling (DONE for this phase)

Removed key singleton manager couplings:

- TaskManager -> GameManager.I replaced with serialized GameManager reference.
- GameFlowController -> GameManager.I replaced with serialized GameManager reference.
- DialogueManager -> UIManager.I replaced with serialized UIManager reference.
- GameManager -> UIManager.I replaced with GameEvents transition request event.

Introduced event-based transition handoff:

- GameEvents.OnTransitionRequested
- GameEvents.RequestTransition(label, duration)
- UIManager subscribes and renders transition overlays.
