# Enversed arcade
The Enversed Arcade project is a game made for the arcade table located in the Enversed game cafe.

# Contributors
## Iteration 1 (Spring 2026)
Demiën van Laarhoven | Programming Lead, Game designer  
Mick Sieben | Art Lead, Team Lead, Game Designer  
Cas Bouwmans | Music/SFX Lead, Game designer  
Max van der Spek | Programmer, Game designer  
Jesper Boersma | Creative Lead, Programmer, Game designer  
Flore van Buul | Artist

## Project structure
The Enversed Arcade project has the following ``Assets/`` folder structure, 
most assets are located within nested folders divided by category

``Animations/``
- Contains both animators and animations, 
In most cases animations are kept inside the FBX of the model however there might be some exceptions

``Audio/``
- Contains all the Music and SFX for the project

``ConfigData/``
- Contains custom ScriptableObjects containing configuration data for various things such as Minigames and GameEvents.

``Fonts/``
- Contains Fonts used in the game

``Materials/``
- Contains all the materials of the game

``Models/``
- Contains all the game's models

``Prefabs/``
- Contains all the prefabs of the game divided by responsibility.
  Configuration for most of the project is contained within ``PRE_Player``, ``PRE_GameManager`` and it's children

``Scenes/``
- Core scenes like the initialization or the lobby are located in ``Scenes/Core``
- Feature branch scenes are contained in ``Scenes/Feature/``
- Finished minigames are contained in ``Scenes/Minigames``
- Scenes containing examples for documentation are contained in ``Scenes/Documentation``

``Scripts/``
- Contains every C# script used within the game

``Settings/``
- Unity project settings file, should not be touched in most cases 

``Shaders``
- Contains the custom shaders used within the project, these shaders are made using Unity's shader graph

``TextMeshPro``
- Package files for the TextMeshPro package

``Textures/``
- Contains all 2d assets of the game

# Custom features
## Game Event System
## Minigame Framework

# Conventions
## Commit messages

## Branching strategy

## Naming conventions

## How to build

