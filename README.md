# BattleCOD 3: Eastern Front 2


[Final Report](https://github.com/aual1780/OOP_BigProject/blob/master/Documents/OOAD%20Project%206.pdf)

[Demo Video](https://drive.google.com/file/d/1tWUO1-6G1WzQOAK0woFV_VDLbZuW1VRk/view?usp=sharing)

![TankSim 1](https://github.com/aual1780/OOP_FinalProject/blob/master/Screenshots/tankSim1.PNG)

### Quick Start

To play the game, download the latest release version and run. Its that easy.  For best results, run the gamehost on a separate computer from the controllers (and invite friends)

### About the Game

BattleCOD 3: Eastern Front 2 is a top-down, 2D survival tank shooter.  Players attempt to stay alive for as long as possible while battling an unrelenting horde of enemies from the protection of a Panzer mk VI Tiger.  Points will be earned for destroying enemies and surviving waves.  Tank teams will be able to compare themselves and evaluate their performance with the game’s scoreboard.

The game has 2 main components: the gamehost and the controllers.  The host will display the game (game field, tank, enemies, health, points, etc) while the controllers allow players to operate the tank remotely.  To force teams to work together, the tank controls will be distributed across multiple controllers.  The game will support 1-6 players, and each player will run their controller from their own device.  Possible controls include tank movement, weapon aiming, weapon loading, and shooting.  Each controller will be assigned roles and can only command those specific tank features.  Additionally, controllers will only show information relative to their roles - this prevents any one team member from knowing everything about the tank.  In this way, the players will have to communicate to operate the tank and avoid blowing themselves up.

However, we have not forgotten our player base - we know that not everyone can pull together 5 (or even 1) friends for a game.  The gamehost will dynamically allocate tank operator roles based on the player count.  If only a single player is present, then all roles will be allocated to a single controller.  To streamline the process, a single player will be able to control the game directly from the gamehost.

This action-packed shooter is guaranteed to ruin friendships and leave players bickering like married couples.  It’s minutes of fun!

![TankSim 2](https://github.com/aual1780/OOP_FinalProject/blob/master/Screenshots/tankSim2.PNG)

### Contributors

Austin Albert

Ian Meadows

Andrew Hack

### Supported Platforms

The game will run on Windows.  The CLI controller will run on any platform that has .Net 5.0 runtime support, but the GUI controller is only available for Windows.

### Networking

This project makes extensive use of the [ArdNet](https://dev.azure.com/tipconsulting/ArdNet) messaging protocol.  It is a multiplatform, high performance, TCP-based network communication library.  We configure the library using floating network ports to allow multiple games on the same network, or even the same machine.  This may cause problems with firewalls since each new game will have a new port number.

### Build Instructions
There are 2 separate solutions with separate build steps.  Project/TankSim can be built with the .Net 5 SDK using standard settings.  This can be done using either the dotnet cli or the latest version of Visual Studio.  Project/TankSim.GameHost.Unity requires Unity 2019.4.14f1 to build.

![TankSim 4](https://github.com/aual1780/OOP_FinalProject/blob/master/Screenshots/tankSim4.PNG)
