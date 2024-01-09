# Neon Abyss ModLoader [WIP]

## Prerequisite
- .NetFramework 4.5
- 0harmony.dll in game root
- doorstop dll loader in game root
- build the modloader and set the post build step to copy to your game folder
- add reference to your game's dll and unity (replace the paths in csproj for yours)
- create a folder mods in your game root and a folder inside with the name of your mod eg: TestMod

## Repository content
- The ModLoader which will load every mod following this structure:
    - C:/YOUR_OWN_PATH/NeonAbyss/mods/YOUR_OWN_MOD.dll
    - You have namespace named the same has the dll name
    - You have class named the same has the dll name inside the namespace
    - You have a function public void Initialize(){} inside the class
    - You need to use the .Net 4.5 framwork configuration for your dll
- A sample mod (TestMod.dll)

## TODO
- Remove hardcoded built paths (which copies the modloader to the game folder)
- Remove hardcoded built paths (which copies the mod dll to the game folder)
- Package assets
- Use reflection to find the entrypoint
- Make a tutorial for everything (Unity Doorstep, Harmony, the modloader, creating a mod)
- Create an installer for the modloader
- Find an easyway to toggle mods on and off
- See if it's possible to redistribute the unstripped dlls here
- Find a better way to log than log file (eg. Allocate console or use ingame console)

## IMPORTANT NOTICE
- The game's dll are stripped (a setting in Unity Engine that removes some required symbols for patching the dlls). You need an unstripped version of these dlls to get the modloader working correctly. To get these unstripped dlls, you just need copy them from a fresh install of Unity. Here is the path to those dll `2018.4.21f1/Editor/Data/MonoBleedingEdge/lib/mono/4.5/*.dll`. You copy these to the game managed dll folder `NeonAbyss/NeonAbyss_Data/Managed`.
- Not affiliated at all with `Veewo Games` all rights are theirs, it's their game.
