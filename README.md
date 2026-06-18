# Frightless Birds
The FrightlessBirds project is a 2-4 player party game made in Unity for the arcade table located in the Enversed Experience Center.
The game is intended to be played with the generic arcade controllers provided by Enversed.

# Content
- [Contributors](#contributors)  
- [Engine Version](#engine-version)
- [Project Structure](#project-structure)
  - [Animations/](#animations)
  - [Audio/](#audio)
  - [ConfigData/](#configdata)
  - [Fonts/](#fonts)
  - [Materials/](#materials)
  - [Models/](#models)
  - [Prefabs](#prefabs)
  - [Scenes/](#scenes)
  - [Scripts](#scripts)
  - [Settings/](#settings)
  - [Shaders/](#shaders)
  - [TextMeshPro/](#textmeshpro)
  - [Textures/](#textures)
- [Custom Features](#custom-featuress)
  - [Game Event System](#game-event-system)
    - [Game Event (ScriptableObjects)](#game-event-scriptableobjects)
    - [GameEventListener.cs](#gameeventlistenercs)
  - [Minigame Framework](#minigame-framework)
  - [Operator Menu](#operator-menu)
- [Conventions](#conventions)
  - [Commit messages](#commit-messages)
  - [Branching strategy](#branching-strategy)
    - [main](#main)
    - [dev](#dev)
    - [feature/{feature-name}](#featurefeature-name)
    - [release/{build-version}](#releasebuild-version)
    - [release/feature/{feature-name}](#releasefeaturefeature-name)
    - [hotfix/hotfix-name](#hotfixhotfix-name)
  - [Naming conventions](#naming-conventions)
  - [Versioning](#versioning)
- [How to build/test](#how-to-buildtest)
  - [How to build](#how-to-build)
  - [How to initialize controllers](#how-to-initilize-controllers)
  - [How to test in the Unity editor](#how-to-test-in-the-unity-editor)

# Contributors
## Sugar Glider Studios (Iteration 1 Spring 2026, Final version 26.6.3)
Demiën van Laarhoven | Programming Lead, Game designer  
Mick Sieben | Art Lead, Team Lead, Game Designer  
Cas Bouwmans | Music/SFX Lead, Game designer  
Max van der Spek | Programmer, Game designer  
Jesper Boersma | Creative Lead, Programmer, Game designer  
Flore van Buul | Artist

# Engine Version
Frightless Birds is developed in version ``6000.3.9f1``

# Project structure
The Enversed Arcade project has the following ``Assets/`` folder structure, 
most assets are located within nested folders divided by category

### ``Animations/``
- Contains both animators and animations, 
In most cases animations are kept inside the FBX of the model however there might be some exceptions

### ``Audio/``
- Contains all the Music and SFX for the project

### ``ConfigData/``
- Contains custom ScriptableObjects containing configuration data for various things such as Minigames and GameEvents.

### ``Fonts/``
- Contains Fonts used in the game

### ``Materials/``
- Contains all the materials of the game

### ``Models/``
- Contains all the game's models

### ``Prefabs/``
- Contains all the prefabs of the game divided by responsibility.
  Configuration for most of the project is contained within ``PRE_Player``, ``PRE_GameManager`` and it's children

### ``Scenes/``
- Core scenes like the initialization or the lobby are located in ``Scenes/Core``
- Feature branch scenes are contained in ``Scenes/Feature/``
- Finished minigames are contained in ``Scenes/Minigames``
- Scenes containing examples for documentation are contained in ``Scenes/Documentation``

### ``Scripts/``
- Contains every C# script used within the game

### ``Settings/``
- Unity project settings file, should not be touched in most cases 

### ``Shaders/``
- Contains the custom shaders used within the project, these shaders are made using Unity's shader graph

### ``TextMeshPro/``
- Package files for the TextMeshPro package

### ``Textures/``
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

## Operator menu
A hidden menu in the PRE_GameManager designed to tweak the gameplay on the fly without having to change values in the inspector. This menu can help with playesting the game to optimize the experience. 
Saved- and default settings are stored in external JSON files named Save.Json & Defaults.Json.

### Using the menu
The menu is made to be navigated by only a mouse.
The menu is activated by pressing  `[Middle mouse + Middle mouse]`.
Changed settings must be saved before exiting the menu, otherwise they will not be changed in the game. The same goes for settings that have been returned to defaults.


### Adding more settings
New settings can be added with these steps:
* Add UI boxes in required menu page.
* Add reference to setting in SettingsData.cs.
* Connect the setting’s variable to the SettingsManager object in the LoadFromSettings function.
* Add references for the setting changer and value indicator text to the settings page’s script, for example OnGeneralChanged.cs, and drag the changer and the text into the references in the inspector.
* Connect the slider’s value to the setting you made in step 2 in SettingsData.cs, also connect the text to the slider’s value.
* Do the same in SettingsManger.cs's ReloadSettings function.
* Make a function in the required setting page’s script to update the setting’s value and connect the function to the slider’s OnValueChanged() in the inspector.



# Conventions
## Commit messages
Commit messages are written in imperative sense and start with a capital.  
Commits are single responsibility however if multiple things have been done within a commit it can be seperated with a -.  
An example of a commit message is:  
``Add character controller - Fix gravity not being applied``

## Branching strategy
The Frightless Birds project has the following branches:  
### ``main``
This branch contains the latest stable released versions of the project. This branch can only recieve commits from ``Release`` branches.  

### ``dev``
This branch contains the stable development builds of the project and can only recieved commits from ``Release`` and ``Feature`` branhces.  

### ``feature/{feature-name}``
These branches contain features currently in development and are not required to be stable. These branches can get merged into dev.
These branches are created from ``dev``

### ``release/{build version}``
These branches are staging evironments for releases of the game, the name of these branches are dictated by the [versioning](#versioning) logic below.
These branches are created from ``dev`` and can only recieve commits from ``release/feature/{feature-name}``.

### ``release/feature/{feature-name}``
These branches contain fixes and minor features intended for a release. These branches are created from ``release/{build-version}`` and can only be merged into ``release/{build-version``.

### ``hotfix/{hotfix-name}``
These branches are rarely used when something is terribly wrong with main and must immidiately be addressed,
these branches are directly merged into main. In most cases the commits here can be done within a release branch instead.

## Naming conventions
The naming convention for files is specified in the files "Enversed assset naming conventions.pdf" and "Sugar glider studio additional naming conventions"

## Versioning
The version on the game(which is set in Player Settings in the build menu) corresponds to the year of the build, 
month of the build and version of the build. 
A optional minor version number for things such as bugfixes is also allowed.

This would look like
``26.6.2.3``
- Where ``26`` is the year 2026
- 6 is the 6th month of the year (June)
- 2 means the second release this month
- 3 means the third minor patch(or hotfix) of the release

# How to build/test
## How to build
Every build of the Frightless Birds project **must** contain ``Scenes/Core/SC_initialization_CO`` 
as it's first entry in the scenes list. ``SC_Core/SC_Lobby_CO`` is also required for the game to function.

Any minigames included in the gamemanager also need to be added to the scene list.

## How to initilize controllers
In ``Scenes/Core/SC_initialization_CO`` you can initialize the controllers by pressed the ``start`` button which is located on the right side of the 2 top buttons.
After all 4 controllers are initialized you can confirm initialization by pressing start twice. To reset initiliazation press ``select`` twice which is left of the ``start`` button.

## How to test in the Unity editor
In unity when on ``Scenes/Core/SC_initialization_CO`` you can initilize their keyboard by pressing ``enter``, 
to initialize the other 3 players you can press ``\`` and finally confirm initialization by pressing the ``space`` twice.
