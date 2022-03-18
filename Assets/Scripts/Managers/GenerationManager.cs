using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nevergreen;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

/// <summary>
/// Handles the main generation control of the world.
/// </summary>
public class GenerationManager : SingletonBehaviour<GenerationManager>
{
    [Header("General Components")]
    public SurfaceGenerationData surfaceData;

    [ReadOnly]
    public int chunksGeneratingCurrently = 0;

    [NonSerialized]
    public int[] SurfaceHeights;
    
    private int surfaceHeightAverage;
    private Vector2 perlinOffset;
    private float perlinAddition;
    
    private FastNoiseLite noise;
    
    private void Awake()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        SurfaceHeights = new int[Settings.WorldWidth];
        surfaceHeightAverage = (int)(Settings.WorldHeight * Settings.AvgSurfaceHeightFactor);

        noise = new FastNoiseLite();
    }

    /// <summary>
    /// Changes the seed of the game to a new seed.
    /// </summary>
    /// <param name="newSeed"></param>
    public void SetSeed(int newSeed = -1)
    {
        int seed = newSeed == -1 ?
            (int)DateTime.Now.Ticks :
            newSeed;
        
        noise = new FastNoiseLite(seed);
        UnityEngine.Random.InitState(seed);
    }

    /// <summary>
    /// Generates surface height data based on the current seed.
    /// </summary>
    public void PrepareGeneration(bool preGenerateWorld)
    {
        perlinOffset = new Vector2(
            UnityEngine.Random.Range(0f, Settings.NoiseOffsetMax),
            UnityEngine.Random.Range(0f, Settings.NoiseOffsetMax));
        perlinAddition = 0;

        int index = 0;
        while (true)
        {
            // Stop if out of bounds
            if (index < 0 || index >= Settings.WorldWidth)
                break;

            // Sample
            float noiseX = perlinOffset.x + perlinAddition;
            float noiseY = perlinOffset.y + perlinAddition;

            /* Sets an average height to continue on and manipulate this height with additional Perlin noise for hills.
             * NOTE: PerlinNoise may return values higher than 1f sometimes as stated in the Unity documentation, so we 
             * have to compensate slightly for an approximate average height with Perlin noise. */
            surfaceHeightAverage += (int)((Mathf.Clamp(Mathf.PerlinNoise(noiseX, noiseY), 0f, 1f) - 0.475f) *
                surfaceData.SurfaceAvgHeightMultiplier);
            SurfaceHeights[index] = surfaceHeightAverage + (int)(Mathf.PerlinNoise(-noiseX, -noiseY) *
                surfaceData.SurfaceHeightMultiplier);
            perlinAddition += surfaceData.SurfacePerlinSpeed;
            index++;
        }

        if (preGenerateWorld)
        {
            ChunkLoadManager.Singleton.PreLoadChunks();
        }
        else
        {
            GameManager.Singleton.OnWorldGenerated.Invoke();
        }
    }

    /// <summary>
    /// A quick check whether the given PerlinNoise parameters exceeds the given threshold.
    /// Used to check whether a type of ore can spawn depending on the perlin height for example.
    /// NOTE: Double sampling is used to avoid straight line syndrome on certain Perlin coordinates.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="perlinSpeed"></param>
    /// <param name="perlinLevel"></param>
    /// <returns></returns>
    //WARN: Change to use simplex noise for far better performance, and optimize!!!
    private bool CheckPerlinLevel(Vector3Int tilePosition, float perlinSpeed, float perlinLevel)
    {
        return (Mathf.PerlinNoise(
                    perlinOffset.x + tilePosition.x * perlinSpeed,
                    perlinOffset.y + tilePosition.y * perlinSpeed) +
                Mathf.PerlinNoise(
                    perlinOffset.x - tilePosition.x * perlinSpeed,
                    perlinOffset.y - tilePosition.y * perlinSpeed)) / 2f >= perlinLevel;
    }
    private bool CheckPerlinLevel(int x, int y, float perlinSpeed, float perlinLevel)
    {
        return (Mathf.PerlinNoise(
                    perlinOffset.x + x * perlinSpeed,
                    perlinOffset.y + y * perlinSpeed) +
                Mathf.PerlinNoise(
                    perlinOffset.x - x * perlinSpeed,
                    perlinOffset.y - y * perlinSpeed)) / 2f >= perlinLevel;
    }

    /// <summary>
    /// Uses multiple CheckPerlinLevel calls to extensively check whether a certain type of ore or
    /// block can be spawned.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="depthMin"></param>
    /// <param name="depthMax"></param>
    /// <param name="perlinSpeed"></param>
    /// <param name="perlinLevel"></param>
    /// <param name="zonePerlinSpeed"></param>
    /// <param name="zonePerlinLevel"></param>
    /// <param name="mapPerlinSpeed"></param>
    /// <param name="mapPerlinLevel"></param>
    /// <returns></returns>
    public bool CheckPerlinEligibility(Vector3Int tilePosition, float depthMin, float depthMax, float perlinSpeed, float perlinLevel,
        float zonePerlinSpeed = 0f, float zonePerlinLevel = 0f, float mapPerlinSpeed = 0f, float mapPerlinLevel = 0f)
    {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (depthMin != -1f && 
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            depthMax != -1f)
        {
            int depth = SurfaceHeights[tilePosition.x] - tilePosition.y;
            if (!(depth >= depthMin && depth < depthMax))
                return false;
        }

        if ((mapPerlinSpeed == 0f && mapPerlinLevel == 0f) ||
            CheckPerlinLevel(tilePosition, mapPerlinSpeed, mapPerlinLevel))
        {
            if ((zonePerlinSpeed == 0f && zonePerlinLevel == 0f) ||
                CheckPerlinLevel(tilePosition, zonePerlinSpeed, zonePerlinLevel))
            {
                if (perlinSpeed == 0f && perlinLevel == 0f)
                    return false;

                return (CheckPerlinLevel(tilePosition, perlinSpeed, perlinLevel));
            }
        }
        return false;
    }
    
    public bool CheckPerlinEligibility(int x, int y, float depthMin, float depthMax, float perlinSpeed, float perlinLevel,
        float zonePerlinSpeed = 0f, float zonePerlinLevel = 0f, float mapPerlinSpeed = 0f, float mapPerlinLevel = 0f)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (depthMin != -1f && 
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            depthMax != -1f)
        {
            int depth = SurfaceHeights[x] - y;
            if (!(depth >= depthMin && depth < depthMax))
                return false;
        }

        if ((mapPerlinSpeed == 0f && mapPerlinLevel == 0f) ||
            CheckPerlinLevel(x, y, mapPerlinSpeed, mapPerlinLevel))
        {
            if ((zonePerlinSpeed == 0f && zonePerlinLevel == 0f) ||
                CheckPerlinLevel(x, y, zonePerlinSpeed, zonePerlinLevel))
            {
                if (perlinSpeed == 0f && perlinLevel == 0f)
                    return false;

                return (CheckPerlinLevel(x, y, perlinSpeed, perlinLevel));
            }
        }
        return false;
    }


    /// <summary>
    /// Create a brand new chunk using Perlin data combined with the given seed.
    /// </summary>
    public IEnumerator GenerateChunk(Chunk chunk)
    {
        while (chunksGeneratingCurrently >= Settings.MaxSyncChunkWrites)
        {
            yield return null;
        }

        chunksGeneratingCurrently++;
        
        //TileBase[] bgTilesToPlace = new TileBase[chunkSize * chunkSize];

        //uint[,] bgBlocks = new uint[chunkSize, chunkSize];

        int tileIndex = -1;

        
        TileBase[] tilesToPlace = new TileBase[Settings.ChunkSize * Settings.ChunkSize];
        uint[,] blocks = new uint[Settings.ChunkSize, Settings.ChunkSize];
        
        for (int y = 0; y < Settings.ChunkSize; y++)
        {
            for (int x = 0; x < Settings.ChunkSize; x++)
            {
                tileIndex++;
                
                int tilePosX = chunk.Position.x + x;
                int tilePosY = chunk.Position.y + y;
                
                //WARN: Possibly useless if statement
                if (tilePosX < 0 ||
                    tilePosX >= Settings.WorldWidth ||
                    tilePosY < 0 ||
                    tilePosY >= Settings.WorldHeight) continue;

                if (tilePosY <= SurfaceHeights[tilePosX])
                {
                    // Start with the default block
                    BlockData blockToPlace = DataLoader.Singleton.Database.BlockDataLookup[Settings.DefaultBlockID];

                    // Loop through the blocks and overwrite the default block if that block can be spawned
                    foreach (BlockData data in DataLoader.Singleton.Database.BlockDataLookup.Values)
                    {
                        if (data.ID == blockToPlace.ID) continue;
                        
                        if(!data.GenerationData.CanGenerate()) continue;

                        if (!CheckPerlinEligibility(tilePosX, tilePosY,
                            data.GenerationData.MinimumDepth,
                            data.GenerationData.MaximumDepth,
                            data.GenerationData.PerlinSpeed,
                            data.GenerationData.PerlinLevel,
                            data.GenerationData.ZonePerlinSpeed,
                            data.GenerationData.ZonePerlinLevel,
                            data.GenerationData.MapPerlinSpeed,
                            data.GenerationData.MapPerlinLevel)) continue;

                        blockToPlace = data;
                        break;
                    }

                    // First set ID block, then set tile block
                    //World.SetBlockAt(tilePosX, tilePosY, World.WorldLayer.FOREGROUND_AND_BACKGROUND, blockToPlace.ID);
                    
                    tilesToPlace[tileIndex] = blockToPlace.Tile;
                    blocks[x, y] = blockToPlace.ID;
                }
                //tileIndex++;
            }
        }

        yield return null;
        chunk.FillChunk(blocks, tilesToPlace);
            
        //yield return null;
        //
        //chunk.FillChunk(
        //    fgBlocks,
        //    fgTilesToPlace);
            
        //yield return null;

        chunksGeneratingCurrently--;
    }
}

/*for (int i = 0; i < scriptableObjects.Count; i++)
{
    BlockGenerationData blockGenData = scriptableObjects[i] as GenerationDataCustomizer;
    if (blockGenData != defaultBlockID)
    {
        if (CheckPerlinEligibility(tilePosition,
            blockGenData.GetSliderData(GenerationData.SliderField.DEPTH_MIN),
            blockGenData.GetSliderData(GenerationData.SliderField.DEPTH_MAX),
            blockGenData.GetSliderData(GenerationData.SliderField.PERLIN_SPEED),
            blockGenData.GetSliderData(GenerationData.SliderField.PERLIN_LEVEL),
            blockGenData.GetSliderData(GenerationData.SliderField.ZONE_PERLIN_SPEED),
            blockGenData.GetSliderData(GenerationData.SliderField.ZONE_PERLIN_LEVEL),
            blockGenData.GetSliderData(GenerationData.SliderField.MAP_PERLIN_SPEED),
            blockGenData.GetSliderData(GenerationData.SliderField.MAP_PERLIN_LEVEL)))
        {
            blockToPlace = blockGenData;
            break;
        }
    }
}*/

// Set the desired tile
//chunk.SetChunkTile(tilePosition, Chunk.TilemapType.BLOCKS_FRONT, blockToPlace.itemTile);
//chunk.SetChunkTile(tilePosition, Chunk.TilemapType.BLOCKS_BACK,
//    blockToPlace.itemType == 0 ? defaultBlockID.itemTile : blockToPlace.itemTile);
//chunk.SetTileColorAt(tilePosition, Color.white, Chunk.TilemapType.BLOCKS_FRONT);
//chunk.SetTileColorAt(tilePosition, new Color(backLayerShadowFactor, backLayerShadowFactor, backLayerShadowFactor),
//    Chunk.TilemapType.BLOCKS_BACK);
//chunk.SetChunkBlockID(tilePosition, Chunk.TilemapType.BLOCKS_FRONT, blockToPlace.itemType);
//chunk.SetChunkBlockID(tilePosition, Chunk.TilemapType.BLOCKS_BACK, 0);    //Was DIRT

//chunk.SetTileColorAt(tilePosition, Color.white, Chunk.TilemapType.BLOCKS_FRONT);
//chunk.SetTileColorAt(tilePosition, new Color(backLayerShadowFactor, backLayerShadowFactor, backLayerShadowFactor),
//    Chunk.TilemapType.BLOCKS_BACK);