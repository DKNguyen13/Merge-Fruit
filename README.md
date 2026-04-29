# Merge Fruit

**Merge Fruit** is a casual *merge & upgrade* game built with **Unity** (C# + shaders). The goal is simple: **combine identical fruits to create higher-tier fruits**, chase high scores, and keep the board from filling up.

> Repo: https://github.com/DKNguyen13/Merge-Fruit

## Highlights (CV-friendly)
- **Unity gameplay programming (C#):** grid/board logic, merging rules, spawning, and scoring.
- **Polished visuals with custom shaders:** ShaderLab/HLSL used for 2D effects and rendering polish.
- **Clean, modular architecture:** reusable components for game state, UI, and input.
- **Fast iteration:** designed as a small but complete project suitable for showcasing core game dev skills.

## Gameplay
- Drop/spawn fruits onto the board.
- When **two identical fruits touch**, they **merge** into the next fruit tier.
- Merges can **chain** and quickly change the board state.
- The game ends when the board is full (or no more valid placements remain, depending on the rules).

## Tech Stack
- **Engine:** Unity
- **Language:** C#
- **Shaders:** ShaderLab, HLSL

## What I Worked On
This repository is a personal project that demonstrates:
- Game loop & state management
- Merge system + progression design
- UI updates (score, next fruit, restart)
- Effects and shader-driven visuals

## How to Run (Unity)
1. Install **Unity Hub**.
2. Open the project folder in Unity (use the Unity version specified in `ProjectSettings/ProjectVersion.txt` if available).
3. Open the main scene (commonly located under `Assets/Scenes/`).
4. Press **Play**.

## Controls
- **Mouse / Touch:** place or drop fruit
- **R (optional):** restart (if implemented)

## Project Structure (typical)
- `Assets/` — game assets, scripts, scenes
- `ProjectSettings/` — Unity project configuration

## Screenshots / Demo
Add a GIF or screenshots here to make it stand out on your CV/portfolio.

## License
No license file is included yet. If you plan to share or reuse this project broadly, consider adding an open-source license (MIT, Apache-2.0, etc.).
