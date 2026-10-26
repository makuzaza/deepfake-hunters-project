# The Puppeteer

> A narrative game about deepfakes, misinformation, and the ethical cost of digital manipulation.

[![Demo – click to watch](https://img.shields.io/badge/▶%20Watch%20Demo-blue?style=for-the-badge)](https://github.com/user-attachments/assets/e8abe407-148d-4744-a58f-7081b8af4af6)

---

## Project Overview

The Puppeteer is a 2D narrative game developed as part of the **Erasmus+ Blended Intensive Programme: Games Against Misinformation and Deepfakes**, organised by Lodz University of Technology. Presented at the **ZTGK Challenge 2026** (Poland).

You play as a new employee at a digital marketing agency who gets assigned tasks involving deepfake creation. You can accept, do poorly, or refuse — each choice affects your risk level and the story.

---

## Features

- Narrative story set across multiple in-game days
- 3 player motivation profiles: Money, Impact, Recognition
- 6 unique characters with distinct roles
- Risk meter that tracks how ethically you behave
- Simulated social media feed showing consequences after each task
- 4 interactive minigames (QTE, Face Sculpting, Face Painting)
- Onboarding flow: name, avatar, motivation
- Dashboard inbox with tasks, messages, and HR form

---

## Technologies

- **Unity 6000.4.8f1**
- Universal Render Pipeline (URP)
- TextMesh Pro
- Unity New Input System
- C# with ScriptableObject-driven data model

---

## Goals

The game puts the player in the role of the manipulator — not the victim — to build understanding of how deepfake content is created and spread. Players experience the social consequences of their choices through a simulated news feed and character reactions.

Learning outcomes:
- Understand how misinformation and synthetic media spread
- Practice ethical decision-making under pressure
- Recognise manipulation tactics like emotional bait and echo chambers

---

## Gameplay Loop

1. Start the day on the Dashboard
2. Complete optional onboarding (HR form, chat with a colleague)
3. Pick a task from the Brief Queue (or refuse)
4. Play the minigame
5. See the result and client feedback
6. Check your phone — view the social media fallout
7. Repeat for the next day
8. After all tasks are done, the game ends with an epilogue screen

---

## Running the Project

1. Open the project in **Unity 6000.4.8f1**
2. Open `Assets/Scenes/MainMenu.unity`
3. Press Play or build for PC via **File > Build Settings**

---

## Team

**DeepFake Hunters** — international student team:

| Role | Member(s) |
|---|---|
| Executive Producer | Shishir |
| Gameplay Programming | Maria, Bran |
| Art & Design | Anders, Santtu |
| Audio | Patrick |
| Writers, Script Editors | Patrick, Bran |

---

## Future Improvements

- **Branching endings** — the logic is built (risk + refusal count determine the ending type) but the ending screen always shows the same text. Connecting these would give players 3 distinct outcomes: Complicit, Whistleblower, or Passive Resistance.
- **Richer dialogue** — motivation profiles filter dialogue but most lines are placeholder; writing real content would make replays feel different
- **Refusal consequences** — refusing tasks currently just skips the day silently; boss/HR reactions would make it feel meaningful
- **Escalating social feed** — Diane's posts could get harsher as risk increases
- **Harder minigames at high risk** — riskier tasks could mean faster needles, smaller zones, shorter timers
- **Debrief screen** — a post-game summary of earnings, risk, and choices as a media-literacy reflection
- **Persistent story consequences** — early choices could visibly change later days (inbox tone, office environment)
