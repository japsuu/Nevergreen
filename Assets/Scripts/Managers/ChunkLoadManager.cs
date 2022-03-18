using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Nevergreen;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

/// <summary>
/// Handles the loading and unloading of chunks in the game.
/// </summary>
public class ChunkLoadManager : SingletonBehaviour<ChunkLoadManager>
{
    [Header("Chunks")]
    public GameObject chunkPrefab;
    public GameObject chunkRoot;

    public long LoadMS = 0;
    public long UnloadMS = 0;

    private bool isUpdatingChunks = false;

    private Camera mainCam;
    //private Rect loadBoundaries;
    private BoundsInt loadBoundaries;
    
    private Stopwatch preloadStopwatch;

    // Use a 2D-array instead of a Dict for better performance and memory usage:
    //private Dictionary<Vector3Int, Chunk> generatedChunks = new Dictionary<Vector3Int, Chunk>();
    private Chunk[,] generatedChunks;

    private Coroutine refreshCoroutine;
    [Button(ButtonSizes.Large, Name = "Refresh all tiles")]
    private void RefreshAllTiles()
    {
        refreshCoroutine = StartCoroutine(RefreshAll());
    }

    [Button(ButtonSizes.Large, Name = "Stop Refresh")]
    private void StopRefreshAllTiles()
    {
        StopCoroutine(refreshCoroutine);
    }

    private IEnumerator RefreshAll()
    {
        foreach (Chunk chunk in generatedChunks)
        {
            if(chunk == null) continue;
            chunk.RefreshAllTiles();

            yield return null;
        }
    }

    private void Awake()
    {
        mainCam = Camera.main;
        
        generatedChunks = new Chunk[
            Settings.WorldWidth / Settings.ChunkSize, 
            Settings.WorldWidth / Settings.ChunkSize];
    }

    public void InitializeCamera()
    {
        // Sets the camera in the horizontal center of the world, at the surface
        float cameraX = 0.5f * Settings.WorldWidth;
        Transform camTransform = mainCam.transform;
        camTransform.position = new Vector3(
            cameraX,
            GenerationManager.Singleton.SurfaceHeights[(int)cameraX] * 1,//WARN: * 2 for debug stuff
            camTransform.position.z);
    }


    /// <summary>
    /// Removes all chunks currently in game.
    /// </summary>
    public void ClearAllChunks()
    {
        StopAllCoroutines();

        // Get all chunks
        List<Chunk> chunksToUnload = new List<Chunk>();
        foreach (Transform child in chunkRoot.transform)
        {
            Chunk chunk = child.GetComponent<Chunk>();
            if (chunk != null)
                chunksToUnload.Add(chunk);
        }

        // Delete them
        foreach (Chunk chunk in chunksToUnload)
            if (chunk != null)
                Destroy(chunk.gameObject);

        StartCoroutine(LoadChunks());
        StartCoroutine(UnloadChunks());
    }


    /// <summary>
    /// Returns the chunk at the given index.
    /// </summary>
    /// <returns>Null if no chunk exists</returns>
    public Chunk GetChunkByIndex(int x, int y)
    {
        return generatedChunks[x, y];
    }
    /// <summary>
    /// Returns the chunk at the given position.
    /// </summary>
    /// <returns>Null if no chunk exists</returns>
    public Chunk GetChunkByPosition(int x, int y)
    {
        return generatedChunks[x / Settings.ChunkSize, y / Settings.ChunkSize];
    }
    /*
    /// <summary>
    /// Returns the BlockData of the block at the given World-space position.
    /// </summary>
    /// <param name="position">World-space position</param>
    /// <param name="mapType"></param>
    /// <returns></returns>
    public TileBase GetTileAt(Vector3Int position, World.WorldLayer mapType)
    {
        //TODO: Convert world-space pos to local pos
        Chunk chunk = GetChunkByIndex(
            position.x / Settings.ChunkSize,
            position.y / Settings.ChunkSize);
        
        return chunk == null ?
            null :
            chunk.GetTileAt(position, mapType);
    }

    /// <summary>
    /// Returns the BlockData of the block at the given World-space position.
    /// </summary>
    /// <param name="position">World-space position</param>
    /// <param name="mapType"></param>
    /// <returns></returns>
    public BlockData GetBlockAt(Vector3Int position, World.WorldLayer mapType)
    {
        //TODO: Convert world-space pos to local pos
        Chunk chunk = GetChunkByIndex(
            position.x / Settings.ChunkSize,
            position.y / Settings.ChunkSize);
        
        return chunk == null ?
            null :
            chunk.GetBlockAt(position, mapType);
    }
    
    public void SetBlockAt(Vector3Int position, World.WorldLayer mapType, BlockData block)
    {
        Chunk chunk = GetChunkByIndex(
            position.x / Settings.ChunkSize,
            position.y / Settings.ChunkSize);
        
        if (chunk != null)
            chunk.SetBlockAt(position, mapType, block);
    }*/
    
    /*public Tilemap GetTilemapAt(Vector3Int position, GenerationManager.TilemapType mapType)
    {
        Chunk chunk = GetChunkByIndex(
            position.x / GenerationManager.Instance.chunkSize,
            position.y / GenerationManager.Instance.chunkSize);
        
        return chunk == null ?
            null :
            GenerationManager.GetMapByType(mapType);
    }*/

    private BoundsInt GetChunkLoadBounds()
    {
        Vector3 camPosition = mainCam.transform.position;
        Vector3 regionStart = camPosition + 
                              Vector3.left * Settings.ChunkRadiusHorizontal + 
                              Vector3.down * Settings.ChunkRadiusVertical;
        
        Vector3 regionEnd = camPosition +
                            Vector3.right * Settings.ChunkRadiusHorizontal + 
                            Vector3.up * Settings.ChunkRadiusVertical;

        // Convert to int for automatic flooring of coordinates
        int regionStartX = (int)regionStart.x / Settings.ChunkSize;
        int regionStartY = (int)regionStart.y / Settings.ChunkSize;
        int regionEndX = ((int)regionEnd.x + Settings.ChunkSize) / Settings.ChunkSize;
        int regionEndY = ((int)regionEnd.y + Settings.ChunkSize) / Settings.ChunkSize;
        
        return new BoundsInt(regionStartX, regionStartY, 0, regionEndX - regionStartX, regionEndY - regionStartY, 1);
    }

    public void PreLoadChunks()
    {
        preloadStopwatch = new Stopwatch();
        StartCoroutine(PerformLoadChunks(true));
    }

    public void StartLoading()
    {
        StartCoroutine(LoadChunks());
        StartCoroutine(UnloadChunks());
    }

    /// <summary>
    /// Starts and maintains the sequence for loading chunks.
    /// </summary>
    private IEnumerator LoadChunks()
    {
        Stopwatch sw = new Stopwatch();
        
        while (true)
        {
            sw.Start();
            
            isUpdatingChunks = true;
            yield return StartCoroutine(PerformLoadChunks());   //WARN: We might have new coroutines creating each frame...
            isUpdatingChunks = false;
            yield return null;

            LoadMS = sw.ElapsedMilliseconds;
        
            sw.Reset();
        }
        // ReSharper disable once IteratorNeverReturns
    }

    /// <summary>
    /// Starts and maintains the sequence for unloading chunks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator UnloadChunks()
    {
        Stopwatch sw = new Stopwatch();
        
        while (true)
        {
            sw.Start();
            
            if (!isUpdatingChunks)
                yield return StartCoroutine(PerformUnloadChunks());
            yield return null;
            
            if(!isUpdatingChunks)
                UnloadMS = sw.ElapsedMilliseconds;
        
            sw.Reset();
        }
        // ReSharper disable once IteratorNeverReturns
    }


    /// <summary>
    /// Scans and loads chunks in view using a Rect as bounds.
    /// </summary>
    private IEnumerator PerformLoadChunks(bool preGenerateWorld = false)
    {
        // If preloading, set the worldBounds to the whole world
        if (preGenerateWorld)
        {
            preloadStopwatch.Start();
            loadBoundaries = new BoundsInt(0, 0, 0,
                Settings.WorldWidth / Settings.ChunkSize,
                Settings.WorldWidth / Settings.ChunkSize,
                1);
        }
        else
        {
            loadBoundaries = GetChunkLoadBounds();
        }
        
        foreach (Vector3Int pos in loadBoundaries.allPositionsWithin)
        {
            // Check if chunkPosition is inside bounds
            if (pos.x < 0 ||
                pos.x >= Settings.WorldWidth / Settings.ChunkSize ||
                pos.y < 0 ||
                pos.y >= Settings.WorldHeight / Settings.ChunkSize) 
                continue;
            

            //if (!loadBoundaries.Contains(pos))    //WARN: Possibly useless..?
            //    continue;
                
            Chunk existingChunk = GetChunkByIndex(pos.x, pos.y);

            // Check if chunk does not exist
            if (existingChunk == null)
            {
                Vector3 worldPosition = new Vector3(
                    pos.x * Settings.ChunkSize,
                    pos.y * Settings.ChunkSize, 0);

                // Generate it. Chunk automatically loads itself upon creation
                generatedChunks[pos.x, pos.y] = Instantiate(
                    chunkPrefab,
                    worldPosition,
                    Quaternion.identity,
                    chunkRoot.transform).GetComponent<Chunk>();
                    
                yield return null;
            }
            else   // Chunk exists
            {
                // And is it currently unloaded?
                if (existingChunk.IsUnloaded)
                {
                    existingChunk.LoadChunk();
                }
            }
        }

        if (preGenerateWorld)
        {
            preloadStopwatch.Stop();
            Debug.Log("World pregenerated in " + preloadStopwatch.ElapsedMilliseconds);
            GameManager.Singleton.OnWorldGenerated.Invoke();
        }
    }


    /// <summary>
    /// Unloads chunks that are outside the view, when we're not loading chunks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformUnloadChunks()
    {
        loadBoundaries = GetChunkLoadBounds();
        List<Chunk> chunksToUnload = new();
        
        //WARN: Should be optimized further to not loop through all existing chunks every frame!
        for (int y = 0; y < generatedChunks.GetLength(1); y++)
        {
            for (int x = 0; x < generatedChunks.GetLength(0); x++)
            {
                Chunk chunk = generatedChunks[x, y];
            
                if (chunk == null || chunk.IsUnloaded) continue;
            
                if (!loadBoundaries.Contains(chunk.ChunkPosition))
                    chunksToUnload.Add(chunk);
            }
        }

        foreach (Chunk chunk in chunksToUnload)
        {
            while (isUpdatingChunks)
                yield return null;

            if (chunk != null)
                chunk.UnloadChunk();
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if(GenerationManager.Singleton == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            loadBoundaries.center * Settings.ChunkSize,
            loadBoundaries.size * Settings.ChunkSize);
    }
}