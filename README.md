# Frightless Birds
The FrightlessBirds project is a 2-4 player party game made for the arcade table located in the Enversed game cafe.
The game is intended to be played with the generic arcade controllers provided by Enversed.

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
The FrightLess Birds project contains a custom event system to handle communication between diffrent parts of the project.
This system consists of 2 parts: a GameEvent(ScriptableObject) and GameEventListener.cs
### Game Event (ScriptableObjects)
This object is created by opening Unity's context menu and using ``Create->EventSystem->GameEvent``, 
these GameEvents should be stored in ``ConfigData/Events``.

In scripts you can use a GameEvent SerializedField to call the event with ``gameEvent.Raise(sender, objects)``
where ``sender`` is a ``component``(usually ``this``) 
and objects is an ``object[]`` of data the listener should recieve. Validation is done on the method listening.
### GameEventListener.cs
This script can be attached to objects containing scripts that need to listen to events. 
In the Listener you need to specify which event it needs to listen to and which method it needs to call.
## Minigame Framework
The Framework used to manage the game, inside of PRE_GameManager you can find the config fields related to the game logic.
New Minigames can be added by expanding the Minigames list with a new Minigame(``Create->MinigameFramework->Minigame``).
Minigame specific configs or overrides are kept withing the Minigame scriptable objects.

# Conventions
## Commit messages

## Branching strategy

## Naming conventions

## Versioning
The version on the game(which is set in Player Settings in the build menu) corresponds to the year of the build, 
month of the build and version of the build. 
A optional minor version number for things such as bugfixes is also allowed.

This would look like
``26.6.2.3``
- Where ``26`` is the year 2026
- 6 is the 6th month of the year (June)
- 2 means the second release this month
- 3 means the third minor patch of the release

## How to build
Every build of the Frightless Birds project **must** contain ``Scenes/Core/SC_initialization_CO`` 
as it's first entry in the scenes list. ``SC_Core/SC_Lobby_CO`` is also required for the game to function.

Any minigames included in the gamemanager also need to be added to the scene list.
