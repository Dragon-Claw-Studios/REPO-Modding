## Damp Mine - 0.0.14
- 1 New normal module
- 1 New passage module
- Overhauled overall level brightness by tweaking textures and lighting settings
- List of possible valuables customized to include a larger variety of valuables, instead of just arctic and generic
- Fixed minimap visual bugs and custom solution
- Added a "safe zone" to the casting tray, so avoiding damaging items when casting is reduced
- Modified a normal module to better accommodate dirt floor transitions between rooms
- Fixed some light leaking through geometry
- Fixed a negative scale collider
- Fixed minor visual mistakes
- Removed some debug logging

## Damp Mine - 0.0.13
- 1 New passage module
- Network optimizations for randomized objects (the room (user 0) is out of 'room' viewIDs)
- Drawers now work like carts, so valuables wont take collision damage when inside and moved
- Implemented checks for casting pot, in case it gets destroyed or detached from its hinge

## Damp Mine - 0.0.12
- Refined textures for some objects
- Implemented some new assets
- Tweaked some material settings
- Fixed some mesh issues
- Made an extraction module smaller to avoid rare overlap of modules
- Fixed the casting pot random selector from being misconfigured, hopefully for the last time
- Added new casting metal types
- Tweaked casting pot spawn weights
- Tweaked casting metal value modifiers
- Casting pot will now only work on Tiny, Small, Medium and Big valuables
- Made garbage container slightly wider

## Damp Mine - 0.0.11
- New textures/materials for remaining objects
- Some mesh fixes
- Pallet texture is less grainy
- Metal casting pot will always spawn now

## Damp Mine - 0.0.10
- 1 New passage module
- New textures/materials for various objects
- Fixed a light clipping issue with hanging lanterns
- Adjusted some colors on wall materials

## Damp Mine - 0.0.9
- 1 New normal module
- 1 New passage module
- Fixed extraction module disabled volume colliders
- Adjusted some lights to make the levels more bright
- Adjusted some floor properties relating to material types across all modules

## Damp Mine - 0.0.8
- 1 New dead end module
- 2 New extraction modules
- Fixed some navmesh issues on previous fix for missing planks
- Re-added missing plank used as table
- Moved a box to make it easier to get the valuable out
- Adjusted some lights to make the levels a bit brighter

## Damp Mine - 0.0.7
- 1 New normal module
- Fixed some z-fighting on shelves and catwalks
- Fixed casting pot randomizer not being set up correctly, causing it to never spawn in
- Adjusted probability of the selection to have a 10% chance of not spawning a casting pot, rather than 40%
- Fixed an occational disconnected navmesh in one of the lobby modules
- Fixed missing planks in passage module
- Fixed networking issue related to the casting pot randomizer. It should sync properly across players now
- Adjusted casting pot molten metal materials to make iron color more distinguishable from silver

## Damp Mine - 0.0.6
- 2 New dead end modules
- Adjusted some navmesh surfaces
- Removed some pillars for better door opening
- Had to redo some minecart placements in one of the modules

## Damp Mine - 0.0.5
- Addressed some network syncing issues (should fix objects not syncing across in multiplayer)
- 2 New normal modules
- Fixed small door audio
- Adjusted collision for small door (it can open wider now)
- Fixed some visual glitches
- Fixed some collision issues between 2 conveyor belts
- Adjusted some killbox settings (hopefully they wont stop working now)
- New surface materials to some module rooms
- Shader fixes/adjustments

## Damp Mine - 0.0.4
- Implemented changelog
- Updated readme

## Damp Mine - 0.0.3
- 1 New passage module
- Adjusted main door colliders (they should open further now)
- Replaced main door UV mapping, texture and material
- Fixed extraction point outline visuals

## Damp Mine - 0.0.2
- Implemented readme

## Damp Mine - 0.0.1
- Initial release
