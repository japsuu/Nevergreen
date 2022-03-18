using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nevergreen
{
    public static class World
    {          
        public static uint[,] BlocksFg;
        public static uint[,] BlocksMg;
        public static uint[,] BlocksBg;

        public const int ForegroundLayer = 0;
        public const int MidgroundLayer = 1;
        public const int BackgroundLayer = 2;
        
        public enum WorldLayer
        {
            FOREGROUND = 0,
            MIDGROUND = 1,
            BACKGROUND = 2,
            FOREGROUND_AND_BACKGROUND = 3
        }

        
        public static void Initialize()
        {
            // Used to store the block IDs for each Tilemap
            BlocksFg = new uint[
                Settings.WorldWidth,
                Settings.WorldHeight];
        
            BlocksMg = new uint[
                Settings.WorldWidth,
                Settings.WorldHeight];
        
            BlocksBg = new uint[
                Settings.WorldWidth,
                Settings.WorldHeight];
        }

        public static Tilemap GetTilemapAtSafe(int x, int y, int layer)
        {
            if (
                x < 0 ||
                y < 0 ||
                x >= Settings.WorldWidth ||
                y >= Settings.WorldHeight) return null;

            Chunk chunk = ChunkLoadManager.Singleton.GetChunkByPosition(x, y);

            if (chunk == null) return null;

            switch (layer)
            {
                case ForegroundLayer:
                    return chunk.VisualForeground;
                    
                case MidgroundLayer:
                    return chunk.VisualMidground;
                    
                case BackgroundLayer:
                    return chunk.VisualBackground;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
        }

        public static void SetBlockAt(int x, int y, WorldLayer layer, uint blockID)
        {
            switch (layer)
            {
                case WorldLayer.FOREGROUND:
                    BlocksFg[x, y] = blockID;
                    break;
                case WorldLayer.BACKGROUND:
                    BlocksBg[x, y] = blockID;
                    break;
                case WorldLayer.MIDGROUND:
                    BlocksMg[x, y] = blockID;
                    break;
                case WorldLayer.FOREGROUND_AND_BACKGROUND:
                    BlocksFg[x, y] = blockID;
                    BlocksBg[x, y] = blockID;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
            
            Chunk chunk = ChunkLoadManager.Singleton.GetChunkByPosition(x, y);
            TileBase tile = DataLoader.Singleton.Database.BlockDataLookup[blockID].Tile;
            chunk.SetTileAt(x, y, tile, layer);
        }
        
        public static uint GetBlockAtUnsafe(int x, int y, WorldLayer layer)
        {
            return layer switch
            {
                WorldLayer.FOREGROUND => BlocksFg[x, y],
                WorldLayer.MIDGROUND => BlocksMg[x, y],
                WorldLayer.BACKGROUND => BlocksBg[x, y],
                _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="layer">0 = FG, 1 = MG, 2 = BG.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static uint GetBlockAtUnsafe(int x, int y, int layer)
        {
            return layer switch
            {
                ForegroundLayer => BlocksFg[x, y],
                MidgroundLayer  => BlocksMg[x, y],
                BackgroundLayer => BlocksBg[x, y],
                _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
            };
        }
        
        public static uint GetBlockAtSafe(int x, int y, WorldLayer layer)
        {
            if (
                x < 0 ||
                y < 0 ||
                x >= Settings.WorldWidth ||
                y >= Settings.WorldHeight) return 0;

                return layer switch
            {
                WorldLayer.FOREGROUND => BlocksFg[x, y],
                WorldLayer.MIDGROUND => BlocksMg[x, y],
                WorldLayer.BACKGROUND => BlocksBg[x, y],
                _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
            };
        }
        public static uint GetBlockAtSafe(int x, int y, int layer)
        {
            if (
                x < 0 ||
                y < 0 ||
                x >= Settings.WorldWidth ||
                y >= Settings.WorldHeight) return 0;
            
            return layer switch
            {
                ForegroundLayer => BlocksFg[x, y],
                MidgroundLayer  => BlocksMg[x, y],
                BackgroundLayer => BlocksBg[x, y],
                _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
            };
        }
        public static uint GetBlockAtSafe(Vector3Int pos, int layer)
        {
            if (
                pos.x < 0 ||
                pos.y < 0 ||
                pos.x >= Settings.WorldWidth ||
                pos.y >= Settings.WorldHeight) return 0;
            
            return layer switch
            {
                ForegroundLayer => BlocksFg[pos.x, pos.y],
                MidgroundLayer  => BlocksMg[pos.x, pos.y],
                BackgroundLayer => BlocksBg[pos.x, pos.y],
                _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
            };
        }
        
        //WARN: Might have issues refreshing if the worldPosition has wrong z-coordinate?
        public static void RefreshTileAtSafe(Vector3Int worldPosition, int layer)
        {
            if (
                worldPosition.x < 0 ||
                worldPosition.y < 0 ||
                worldPosition.x >= Settings.WorldWidth ||
                worldPosition.y >= Settings.WorldHeight) return;

            switch (layer)
            {
                case ForegroundLayer:
                    Chunk chunk1 = ChunkLoadManager.Singleton.GetChunkByPosition(worldPosition.x, worldPosition.y); //WARN: Can be optimized by getting rid of the null checks.
                    if(chunk1 != null)
                        chunk1.RefreshTileAt(worldPosition, WorldLayer.FOREGROUND);
                    break;
                case MidgroundLayer:
                    Chunk chunk2 = ChunkLoadManager.Singleton.GetChunkByPosition(worldPosition.x, worldPosition.y);
                    if(chunk2 != null)
                        chunk2.RefreshTileAt(worldPosition, WorldLayer.MIDGROUND);
                    break;
                case BackgroundLayer:
                    Chunk chunk3 = ChunkLoadManager.Singleton.GetChunkByPosition(worldPosition.x, worldPosition.y);
                    if(chunk3 != null)
                        chunk3.RefreshTileAt(worldPosition, WorldLayer.BACKGROUND);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
        }
        
        
        public static void SetIDBlock(WorldLayer layer, int xMin, int yMin, int blockWidth, int blockHeight, uint[,] idBlock)
        {
            switch (layer)
            {
                case WorldLayer.FOREGROUND:
                    for (int y = 0; y < blockHeight; y++)
                    {
                        for (int x = 0; x < blockWidth; x++)
                        {
                            BlocksFg[xMin + x, yMin + y] = idBlock[x, y];
                        }
                    }
                    
                    break;
                case WorldLayer.BACKGROUND:
                    for (int y = 0; y < blockHeight; y++)
                    {
                        for (int x = 0; x < blockWidth; x++)
                        {
                            BlocksBg[xMin + x, yMin + y] = idBlock[x, y];
                        }
                    }
                    
                    break;
                case WorldLayer.MIDGROUND:
                    for (int y = 0; y < blockHeight; y++)
                    {
                        for (int x = 0; x < blockWidth; x++)
                        {
                            BlocksMg[xMin + x, yMin + y] = idBlock[x, y];
                        }
                    }
                    
                    break;
                case WorldLayer.FOREGROUND_AND_BACKGROUND:
                    for (int y = 0; y < blockHeight; y++)
                    {
                        for (int x = 0; x < blockWidth; x++)
                        {
                            BlocksFg[xMin + x, yMin + y] = idBlock[x, y];
                            BlocksBg[xMin + x, yMin + y] = idBlock[x, y];
                        }
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
        }
    }          
}