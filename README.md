# Nevergreen

Terraria in Unity, rewrite of [Admortem](https://github.com/japsuu/Admortem).

### Development currently paused, as I found out it's much more interesting to simulate every pixel on their own.
You can follow the progress on this [here](https://github.com/japsuu/Grit)

---

## Features
Custom grid based "flood fill" lighting (bilinear smoothed & point).
[Custom tilemap system](https://github.com/japsuu/Nevergreen/blob/master/Assets/CustomTilemap/CustomTilemap.cs) allowing near infinite tiles, with next to no performance cost.
Extensive [terrain generation system](https://github.com/japsuu/Nevergreen/blob/master/Assets/Scripts/Managers/GenerationManager.cs).
Performant [chunking system](https://github.com/japsuu/Nevergreen/blob/master/Assets/Scripts/Managers/ChunkLoadManager.cs) for streaming the world.
[Custom rule tile](https://github.com/japsuu/Nevergreen/blob/master/Assets/Scripts/NevergreenRuleTile.cs) system, for Unity built-in Tilemaps.

### Built to be modded, with [content getting dynamically loaded from JSON](https://github.com/japsuu/Nevergreen/blob/master/Assets/Scripts/Helpers/LoadHelper.cs) datastores. This can be seen in action [here](https://www.youtube.com/watch?v=GJunjVF2tL8).
