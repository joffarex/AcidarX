AcidarX
=======

AcidarX (a.k.a AX) is a modular Game Framework, built with C# and SILK.NET. It is aiming to be a complete package for game developers, who prefer to write code instead of using ui tools, but also usable as separate modules (For example, you can use `AcidarX.ECS` in your own game framework/engine which uses MonoGame).

> :warning: **Currently under development**: It has not even hit alpha stage yet, and all the previously claimed 'modularity' will probably come really later.

Features to add before starting making actual games: 
--------------------------------------------------
- [ ] Basic 2D renderer with batch rendering (for top-down/sidescroller pixel art games, basically whatever does not need lighting)
- [ ] Asset manager, we need to load everything on application/scene load
- [ ] ECS integrated with core renderer 
- [ ] Audio manager 
- [ ] Image processor
- [ ] Animation engine
- [ ] ImGui integration for debugging purposes
- [ ] Simple level editor with ImGui
- [ ] REALLY basic physics engine

Make some simple games for framework showcase: 
----------------------------------------------
> These projects are just really simple ones, to help figure out possible refactorings, possible features to add to the core framework, etc. Also this list could change with other simple games and it will also get updated with potential links to game repos with playable demos.

- [ ] Breakout with advanced shit, like level-ups
- [ ] Snake like seen in Chili videos
- [ ] Minesweeper clone
- [ ] Platformer with animations and basic physics

### Development
Feature development will definitely happen in separate branches, which will get rebased to the main branch and will keep track of changes with PRs. It will also help me do some self code reviews and potentially leave some comments about certain decisions made in said PRs. 

These features will need to get implemented before initial release (some of them could be ignored):
---------------------------------------------------------------------------------------------------
> This is not really a full set of features, just what I could think off top of my head. 

- [ ] Lighting to 2D renderer
- [ ] Built-in AI engine and Algorithm implementations
- [ ] Split code into modular architecture
- [ ] Better Level Editor which is NOT tied into the actual framework code but can be used with it
- [ ] Improved animation system with ImGui integration into separate tool maybe
- [ ] Improved physics engine
- [ ] More components into ECS, figured out by example projects
- [ ] Multithreading support
- [ ] Networking layer for co-op
- [ ] Improved debugging utilities, like adding framework level profiling, improved logging, snapshots, etc.
- [ ] Updated serialization system, maybe implement something faster than json, like ".axs" 
- [ ] Game UI framework
- [ ] File I/O, VFS (Virtual File System)
