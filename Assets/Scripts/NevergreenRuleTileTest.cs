using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

/// <summary>
/// 
/// </summary>
public class NevergreenRuleTileTest : SingletonBehaviour<NevergreenRuleTileTest>
{
    public struct TilePair
    {
        public uint ID;
        public Tilemap Tilemap;

        public TilePair(uint id, Tilemap tilemap)
        {
            ID = id;
            Tilemap = tilemap;
        }
    }
    
    public NevergreenRuleTile tile;
    public RuleTile oTile;

    public Tilemap tileMap1;
    public Tilemap tileMap2;
    
    public TilePair[,] tileData;

    public int testAreaWidth = 300;
    public int testAreaHeight = 300;

    public int refreshCalls = 0;
    
    [Button(ButtonSizes.Large, Name = "Set dirty")]
    private void SetThisDirty()
    {
        UnityEditor.EditorUtility.SetDirty(tile);
    }


    [Button(ButtonSizes.Large, Name = "Execute")]
    private void Execute()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        tileData = new TilePair[testAreaWidth, testAreaHeight];
        TileBase[] tiles1 = new TileBase[testAreaWidth * testAreaHeight];
        TileBase[] tiles2 = new TileBase[testAreaWidth * testAreaHeight];

        int index = -1;
        
        for (int y = 0; y < testAreaHeight; y++)
        {
            for (int x = 0; x < testAreaWidth; x++)
            {
                index++;
                
                bool place = Random.Range(0, 2) == 0;
                
                if(!place) continue;

                Tilemap tilemap = Random.Range(1, 3) == 1 ?
                    tileMap1 :
                    tileMap2;
                
                tileData[x, y] = new TilePair(1, tilemap);
                
                tilemap.SetTile(new Vector3Int(x, y), oTile == null ? tile : oTile);
                
                //if(tilemap == tileMap1)
                //    tiles1[index] = oTile == null ? tile : oTile;
                //else
                //    tiles2[index] = oTile == null ? tile : oTile;
            }
        }

        // BoundsInt bounds = new BoundsInt(0, 0, 0, testAreaWidth, testAreaHeight, 1);
        //
        // tileMap1.SetTilesBlock(bounds, tiles1);
        // tileMap2.SetTilesBlock(bounds, tiles2);
        
        sw.Stop();
        Debug.Log("Execute took " + sw.ElapsedMilliseconds + " ms.");
    }
    
    [Button(ButtonSizes.Large, Name = "Refresh")]
    private void Refresh()
    {
        refreshCalls = 0;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        tileMap1.RefreshAllTiles();
        tileMap2.RefreshAllTiles();
        
        sw.Stop();
        Debug.Log("Refresh took " + sw.ElapsedMilliseconds + " ms.");
    }
    
    [Button(ButtonSizes.Large, Name = "Clear")]
    private void Clear()
    {
        tileData = null;
        
        tileMap1.ClearAllTiles();
        tileMap1.CompressBounds();
        
        tileMap2.ClearAllTiles();
        tileMap2.CompressBounds();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(testAreaWidth / 2f, testAreaHeight / 2f), new Vector3(testAreaWidth, testAreaHeight));
    }
}
