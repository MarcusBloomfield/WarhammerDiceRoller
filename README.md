# Warhammer Dice Roller

This is a Unity project designed to simulate dice rolls for the Warhammer tabletop game.

## Project Structure

The project follows a standard Unity project structure:

*   **`Assets/`**: Contains all game assets, scripts, and scenes.
    *   **`Scripts/`**: Houses the C# scripts for game logic. Key scripts include:
        *   `WarhammerDice.cs`: Likely contains the core dice rolling simulation logic.
        *   `Attack.cs`: Potentially defines attack-related data or logic.
        *   `Results.cs`: A data class to store the outcome of dice rolls.
        *   `Defence.cs`: Potentially defines defence-related data or logic.
        *   `Dice.cs`: May contain generic dice rolling utilities.
        *   `AttackListEntry.cs` & `DefenceListEntry.cs`: Likely related to UI elements for displaying attack/defence information.
    *   **`Scenes/`**: Contains the game scenes.
    *   **`Prefabs/`**: Stores pre-configured GameObjects.
    *   **`Materials/`**: Contains materials for 3D models.
    *   **`Models/`**: Stores 3D models.
    *   `UI.uxml` & `UI.uss`: UI definition files, indicating the use of Unity's UI Toolkit.
*   **`ProjectSettings/`**: Contains settings for the Unity project.
*   **`Packages/`**: Manages project dependencies.

## Overview

The application appears to simulate Warhammer combat by allowing users to define attacks and see the resulting hits, wounds, saves, and damage. The UI is likely built using UI Toolkit.

## How to Use (Presumed)

1.  Open the project in Unity Hub.
2.  Open the main scene from the `Assets/Scenes/` folder.
3.  Enter attack parameters into the UI.
4.  Initiate the dice roll simulation.
5.  View the results displayed in the UI.

*(This README is based on an initial project scan. Further details will be added as the project is explored more thoroughly.)* 