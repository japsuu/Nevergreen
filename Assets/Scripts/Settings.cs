using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the user-configurable & non-configurable setting entries.
/// </summary>
public static class Settings
{
    #region USER_CONFIGURABLE_SETTINGS

    /// <summary>
    /// Maximum amount of chunks that can be synchronously generated at the same time.
    /// </summary>
    public static int MaxSyncChunkWrites = 3;

    #endregion

    #region USER_NON_CONFIGURABLE_SETTINGS

    /// <summary>
    /// Size of a single chunk, for both x- and y-axis.
    /// </summary>
    public const int ChunkSize = 32;
    
    /// <summary>
    /// Width of the world, in blocks. Should be a multiple of ChunkSize. 
    /// </summary>
    public const int WorldWidth = 512;
    
    /// <summary>
    /// Height of the world, in blocks. Should be a multiple of ChunkSize. 
    /// </summary>
    public const int WorldHeight = 512;

    /// <summary>
    /// Width of the world, in chunks.
    /// </summary>
    public const int WorldChunkWidth = WorldWidth / ChunkSize;    //TODO: Use

    /// <summary>
    /// Height of the world, in chunks.
    /// </summary>
    public const int WorldChunkHeight = WorldHeight / ChunkSize;    //TODO: Use
    
    /// <summary>
    /// The horizontal area inside which chunks get loaded.
    /// </summary>
    public const int ChunkRadiusHorizontal = 128;
    
    /// <summary>
    /// The vertical area inside which chunks get loaded.
    /// </summary>
    public const int ChunkRadiusVertical = 96;
    
    /// <summary>
    /// Average height percentage relative to the world height, where the surface is placed. Range: 0-1
    /// </summary>
    public const float AvgSurfaceHeightFactor = 0.6f;
    
    /// <summary>
    /// Maximum offset used for the noise.
    /// </summary>
    public const float NoiseOffsetMax = 10000f;
    
    /// <summary>
    /// ID of the default block placed by the world generator.
    /// </summary>
    public const uint DefaultBlockID = 1;

    #endregion

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("MaxSyncChunkWrites", MaxSyncChunkWrites);
    }
    
    
    public static void LoadSettings()
    {
        MaxSyncChunkWrites = PlayerPrefs.GetInt("MaxSyncChunkWrites", MaxSyncChunkWrites);
    }
}
