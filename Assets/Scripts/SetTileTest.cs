using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class SetTileTest : MonoBehaviour
{
    public TileBase tile;
    public Tilemap targetTilemap;

    public Vector2Int originOffset = new Vector2Int(5, 3);
    
    public int tilesToGenerateX = 500;
    public int tilesToGenerateY = 500;

    public int chunksX = 30;
    public int chunksY = 30;
    public int chunkSize = 32;

    public Mode mode;
    
    public enum Mode
    {
        SetTile,
        SetTiles,
        SetTilesBlock
    }

    [Button(ButtonSizes.Large, Name = "Test Chunked performance")]
    private void Test()
    {
        StartCoroutine(TestChunked());
    }
    
    private IEnumerator TestChunked()
    {
        Stopwatch sw = new Stopwatch();
        
        sw.Start();

        for (int cy = 0; cy < chunksY; cy++)
        {
            for (int cx = 0; cx < chunksX; cx++)
            {
                TileBase[] tiles = new TileBase[tilesToGenerateX * tilesToGenerateY];

                BoundsInt bounds = new BoundsInt(
                    cx * chunkSize, cy * chunkSize, 0,
                    chunkSize, chunkSize, 1);
                
                int index = 0;
                for (int y = 0; y < bounds.size.y; y++)
                {
                    for (int x = 0; x < bounds.size.x; x++)
                    {
                        tiles[index] = tile;
                        
                        index++;
                    }
                }
                targetTilemap.SetTilesBlock(bounds, tiles);
                
                yield return null;
            }
        }

        sw.Stop();
        
        Debug.Log("MS: " + sw.ElapsedMilliseconds);
    }

    [Button(ButtonSizes.Large, Name = "Execute")]
    private void Execute()
    {
        Stopwatch sw = new Stopwatch();
        
        sw.Start();
        
        switch (mode)
        {
            case Mode.SetTile:
                
                for (int y = 0; y < tilesToGenerateY; y++)
                {
                    for (int x = 0; x < tilesToGenerateX; x++)
                    {
                        targetTilemap.SetTile(new Vector3Int(x + originOffset.x, y + originOffset.y, 0), tile);
                    }
                }
                break;
            
            
            case Mode.SetTiles:
                
                Vector3Int[] positions = new Vector3Int[tilesToGenerateX * tilesToGenerateY];
                
                TileBase[] tiles1 = new TileBase[tilesToGenerateX * tilesToGenerateY];
                
                int index1 = 0;
                
                for (int y = 0; y < tilesToGenerateY; y++)
                {
                    for (int x = 0; x < tilesToGenerateX; x++)
                    {
                        positions[index1] = new Vector3Int(x + originOffset.x, y + originOffset.y, 0);
                        tiles1[index1] = tile;
                        
                        index1++;
                    }
                }
                targetTilemap.SetTiles(positions, tiles1);
                break;
            
            
            case Mode.SetTilesBlock:
                
                TileBase[] tiles2 = new TileBase[tilesToGenerateX * tilesToGenerateY];

                BoundsInt bounds = new BoundsInt(
                    originOffset.x, originOffset.y, 0,
                    tilesToGenerateX, tilesToGenerateY, 1);
                
                int index2 = 0;
                
                for (int y = 0; y < tilesToGenerateY; y++)
                {
                    for (int x = 0; x < tilesToGenerateX; x++)
                    {
                        tiles2[index2] = tile;
                        
                        index2++;
                    }
                }
                targetTilemap.SetTilesBlock(bounds, tiles2);
                break;
            
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        sw.Stop();
        
        Debug.Log("Mode: " + mode + " MS: " + sw.ElapsedMilliseconds);
    }

    [Button(ButtonSizes.Large, Name = "Clear")]
    private void Clear()
    {
        targetTilemap.ClearAllTiles();
        
        targetTilemap.CompressBounds();
    }
    
    [Button(ButtonSizes.Large, Name = "StopAllCoroutines")]
    private void StopAll()
    {
        StopAllCoroutines();
    }
}
