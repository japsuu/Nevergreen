using System;
using System.Collections;
using System.Collections.Generic;
using Nevergreen;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
// ReSharper disable Unity.InefficientPropertyAccess

/// <summary>
/// Describes a chunk in the world that can load and unload itself.
/// </summary>
//TODO: This class could be changed into a non-MonoBehaviour class which references the in-game objects when needed.
//TODO: Implement a working unloadLifetime system
//TODO: Support for MIDGROUND -layer
//TODO: Update documentation
public class Chunk : MonoBehaviour
{
    public TileBase collisionTile;
    
    public Vector3Int Position { get; private set; }
    public Vector3Int ChunkPosition { get; private set; }
    
    public Tilemap collisionTilemap;

    public Tilemap VisualForeground;
    public Tilemap VisualMidground;
    public Tilemap VisualBackground;
    
    public bool IsUnloaded { get; private set; }

    private GameObject tilemapsRoot;

    private BoundsInt chunkBounds;
    
    //TODO: Can be moved to a single location, does not need to be chunk specific
    private Vector3Int[] fgPositions;//WARN: Above note!!!
    //private Vector3Int[] bgPositions;

    /// <summary>
    /// Returns the right Tilemap for the given TilemapType.
    /// </summary>
    /// <returns></returns>
    public Tilemap GetMapByType(World.WorldLayer mapType)
    {
        return mapType switch
        {
            World.WorldLayer.FOREGROUND => VisualForeground,
            World.WorldLayer.BACKGROUND => VisualBackground,
            World.WorldLayer.MIDGROUND => VisualMidground,
            _ => null
        };
    }

    private void Start()
    {
        IsUnloaded = true;
        
        Position = new Vector3Int((int)transform.position.x, (int)transform.position.y);
        ChunkPosition = new Vector3Int(
            Position.x / Settings.ChunkSize,
            Position.y / Settings.ChunkSize);
        
        fgPositions = new Vector3Int[Settings.ChunkSize * Settings.ChunkSize];
        //bgPositions = new Vector3Int[Settings.ChunkSize * Settings.ChunkSize];

        int index = 0;
        for (int y = 0; y < Settings.ChunkSize; y++)
        {
            for (int x = 0; x < Settings.ChunkSize; x++)
            {
                fgPositions[index] = new Vector3Int(x, y, World.ForegroundLayer);
                //bgPositions[index] = new Vector3Int(x, y, World.BackgroundLayer);

                index++;
            }
        }

        Transform root = transform.GetChild(0);

        if (root == null)
        {
            Debug.LogError("Tilemap prefab setup incorrectly!");
        }
        else
        {
            tilemapsRoot = root.gameObject;
        }
        
        chunkBounds = new BoundsInt(
            Position.x,
            Position.y,
            0,
            Settings.ChunkSize,
            Settings.ChunkSize,
            1);
        
        //VisualForeground.transform.position = Vector3.zero;
        //VisualMidground.transform.position = Vector3.zero;
        //VisualBackground.transform.position = Vector3.zero;
        
        StartCoroutine(GenerationManager.Singleton.GenerateChunk(this));
       
        LoadChunk();
    }

    public void RefreshAllTiles()
    {
        VisualForeground.RefreshAllTiles();
        VisualMidground.RefreshAllTiles();
        VisualBackground.RefreshAllTiles();
    }

    /// <summary>
    /// Loads this chunk.
    /// </summary>
    public void LoadChunk()
    {
        tilemapsRoot.SetActive(true);

        IsUnloaded = false;
    }

    /// <summary>
    /// Unloads this chunk from the world.
    /// </summary>
    public void UnloadChunk()
    {
        /* Destroying the chunk doesn't happen instantly. DestroyImmediate works, but it is
         * safer to just track a bool until it is loaded out of memory. */
        //Destroy(gameObject);
        
        tilemapsRoot.SetActive(false);

        IsUnloaded = true;
    }
    
    public void FillChunk(uint[,] foregroundBlocks, TileBase[] foregroundTiles/*, uint[,] backgroundBlocks, TileBase[] backgroundTiles*/)
    {
        World.SetIDBlock(
            World.WorldLayer.FOREGROUND_AND_BACKGROUND,
            Position.x,
            Position.y,
            Settings.ChunkSize,
            Settings.ChunkSize,
            foregroundBlocks);
        
        //WARN: NevergreenRuleTile is not SetTiles/SetTilesBlock -safe.
        //VisualForeground.SetTiles(fgPositions, foregroundTiles);
        //VisualBackground.SetTiles(bgPositions, foregroundTiles);

        int index = 0;
        for (int y = 0; y < Settings.ChunkSize; y++)
        {
            for (int x = 0; x < Settings.ChunkSize; x++)
            {
                VisualForeground.SetTile(new Vector3Int(x, y, World.ForegroundLayer), foregroundTiles[index]);
                index++;
            }
        }
        
        //VisualForeground.SetTilesBlock(new BoundsInt(0, 0, 0, Settings.ChunkSize, Settings.ChunkSize, 1), foregroundTiles);
        //VisualBackground.SetTilesBlock(chunkBounds, foregroundTiles);
        
        /*
        TileBase[] collisions = new TileBase[foregroundBlocks.Length];
        
        for (int i = 0; i < foregroundTiles.Length; i++)
        {
            if (foregroundTiles[i] == null)
            {
                collisions[i] = null;
            }
            else
            {
                collisions[i] = collisionTile;
            }
        }
        
        collisionTilemap.SetTilesBlock(chunkBounds, collisions);*/
    }

    public void SetTileAt(int x, int y, TileBase tile, World.WorldLayer layer)
    {
        Vector3Int pos = new Vector3Int(x - Position.x, y - Position.y, (int) layer);
        
        switch (layer)
        {
            case World.WorldLayer.FOREGROUND:
                VisualForeground.SetTile(pos, tile);
                
                break;
            case World.WorldLayer.MIDGROUND:
                VisualMidground.SetTile(pos, tile);
                
                break;
            case World.WorldLayer.BACKGROUND:
                VisualBackground.SetTile(pos, tile);

                break;
            case World.WorldLayer.FOREGROUND_AND_BACKGROUND:
                VisualForeground.SetTile(new Vector3Int(x - Position.x, y - Position.y, World.ForegroundLayer), tile);
                VisualBackground.SetTile(new Vector3Int(x - Position.x, y - Position.y, World.BackgroundLayer), tile);
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
        }
    }
    
    public void RefreshTileAt(Vector3Int position, World.WorldLayer layer)
    {
        Vector3Int pos = position - Position;
        
        //TODO: Ensure the z-position is correct?
        switch (layer)
        {
            case World.WorldLayer.FOREGROUND:
                if(VisualForeground.GetTile(pos) == null) return;
                VisualForeground.RefreshTile(pos);
                
                break;
            case World.WorldLayer.MIDGROUND:
                if(VisualMidground.GetTile(pos) == null) return;
                VisualMidground.RefreshTile(pos);
                
                break;
            case World.WorldLayer.BACKGROUND:
                if(VisualBackground.GetTile(pos) == null) return;
                VisualBackground.RefreshTile(pos);
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
        }
        
        //Debug.Log($"[{Position}]: Refresh at " + pos);
    }
    
    /*
    public void SetBlockAt(Vector3Int position, World.WorldLayer mapType, BlockData block)
    {
        uint[,] data = GetIDMap(mapType);
        if (data == null)
            return;

        Vector3Int relativePosition = position - Position;
        data[relativePosition.x, relativePosition.y] = block.ID;
        
        Tilemap visualMap = GetMapByType(mapType);
        if (visualMap == null)
            return;

        /*switch (mapType)
        {
            // If block is placed on foreground and is not AIR, set a collider too.
            case TilemapType.FOREGROUND:
                collisionTilemap.SetTile(relativePosition, block.ID == 0 ? null : collisionTile);
                break;
            case TilemapType.BACKGROUND:
            case TilemapType.MIDGROUND:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mapType), mapType, null);
        }

        visualMap.SetTile(relativePosition, block.Tile);
    }
    
    public BlockData GetBlockAt(Vector3Int position, World.WorldLayer mapType)
    {
        Vector3Int relativePosition = position - Position;
        
        //uint[,] data = GetIDMap(mapType);
        uint ID = World.BlocksBg
        if (ID == 0)
            return DataLoader.Singleton.Database.BlockDataLookup[0];

        return DataLoader.Singleton.Database.BlockDataLookup[data[relativePosition.x, relativePosition.y]];
    }*/

    public TileBase GetTileAt(Vector3Int position, World.WorldLayer mapType)
    {
        Vector3Int relativePosition = position - Position;
        
        Tilemap visualMap = GetMapByType(mapType);
        
        return visualMap == null ?
            null :
            visualMap.GetTile(relativePosition);
    }
    
    /*private uint[,] GetIDMap(World.WorldLayer mapType)
    {
        return mapType switch
        {
            World.WorldLayer.FOREGROUND => blocksFg,
            World.WorldLayer.MIDGROUND => blocksMg,
            World.WorldLayer.BACKGROUND => blocksBg,
            _ => null
        };
    }*/
    /*
    /// <summary>
    /// Sets the color of the tile in the given Tilemap at the given position.
    /// 
    /// NOTE: Tiles by default have TileFlags that prevent color changes, rotation
    /// lock etc. Make sure you unlock these tiles by changing its TileFlags or go 
    /// into the inspector and visit your tiles, turn on debug mode and change it there,
    /// before using this function.    
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <param name="mapType"></param>
    public void SetTileColorAt(Vector3Int position, Color color, World.WorldLayer mapType = World.WorldLayer.FOREGROUND)
    {
        Tilemap targetMap = GetMapByType(mapType);
        if (targetMap == null)
            return;

        Vector3Int relativePosition = position - (Vector3Int)Position;
        targetMap.SetColor(relativePosition, color);
    }

    /// <summary>
    /// Sets the rotation of a given tile at the given position in the given Tilemap.
    /// 
    /// NOTE: Tiles by default have TileFlags that prevent color changes, rotation
    /// lock etc. Make sure you unlock these tiles by changing its TileFlags or go 
    /// into the inspector and visit your tiles, turn on debug mode and change it there,
    /// before using this function.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mapType"></param>
    /// <param name="rotation"></param>
    public void SetChunkTileRotation(Vector3Int position, World.WorldLayer mapType, Vector3 rotation)
    {
        Tilemap targetMap = GetMapByType(mapType);
        if (targetMap == null)
            return;

        Vector3Int relativePosition = position - (Vector3Int)Position;
        Quaternion matrixRotation = Quaternion.Euler(rotation);
        Matrix4x4 matrix = Matrix4x4.Rotate(matrixRotation);
        targetMap.SetTransformMatrix(relativePosition, matrix);
    }*/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        if(IsUnloaded)
            Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(chunkBounds.center, chunkBounds.size);
    }
}