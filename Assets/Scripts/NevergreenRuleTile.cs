using System;
using System.Collections;
using System.Collections.Generic;
using Nevergreen;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/Nevergreen Rule Tile", fileName = "NevergreenRuleTile")]
public class NevergreenRuleTile : RuleTile<NevergreenRuleTile.Neighbor>
{
    public class Neighbor : RuleTile.TilingRuleOutput.Neighbor
    {
        public const int Nothing = 3;
        public const int Something = 4;
    }

    private static Vector3Int[] AllNeighbours = new[]
    {
        new Vector3Int(-1, 0),
        new Vector3Int(-1, -1),
        new Vector3Int(0, -1),
        new Vector3Int(1, -1),
        new Vector3Int(1, 0),
        new Vector3Int(1, 1),
        new Vector3Int(0, 1),
        new Vector3Int(-1, 1)
    };

    public override bool RuleMatches(TilingRule rule, Vector3Int localPosition, ITilemap tilemap, ref Matrix4x4 transform)
    {
        Vector3 tilemapWorldPos = tilemap.GetComponent<Transform>().position;
        
        Vector3Int worldPos = localPosition + new Vector3Int((int)tilemapWorldPos.x, (int)tilemapWorldPos.y, 0);
        if (NevergreenRuleMatches(rule, worldPos))
        {
            //WARN: Possibly useless?
            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
            return true;
        }

        /*
        // Check rule against rotations of 0, 90, 180, 270
        if (rule.m_RuleTransform == TilingRuleOutput.Transform.Rotated)
        {
            for (int angle = m_RotationAngle; angle < 360; angle += m_RotationAngle)
            {
                if (RuleMatches(rule, position, tilemap, angle))
                {
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
                    return true;
                }
            }
        }
        // Check rule against x-axis, y-axis mirror
        else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorXY)
        {
            if (RuleMatches(rule, position, tilemap, true, true))
            {
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, -1f, 1f));
                return true;
            }
            if (RuleMatches(rule, position, tilemap, true, false))
            {
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
                return true;
            }
            if (RuleMatches(rule, position, tilemap, false, true))
            {
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
                return true;
            }
        }
        // Check rule against x-axis mirror
        else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorX)
        {
            if (RuleMatches(rule, position, tilemap, true, false))
            {
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
                return true;
            }
        }
        // Check rule against y-axis mirror
        else if (rule.m_RuleTransform == TilingRuleOutput.Transform.MirrorY)
        {
            if (RuleMatches(rule, position, tilemap, false, true))
            {
                transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
                return true;
            }
        }*/

        return false;
    }

    private bool NevergreenRuleMatches(TilingRule rule, Vector3Int worldPosition)
    {
        int minCount = Math.Min(rule.m_Neighbors.Count, rule.m_NeighborPositions.Count);
        for (int i = 0; i < minCount; i++)
        {
            int neighbourRule = rule.m_Neighbors[i];
            
            //WARN: Useless if we don't use rotations
            //Vector3Int positionOffset = GetRotatedPosition(rule.m_NeighborPositions[i], angle);
            //Vector3Int offsetPos = GetOffsetPosition(position, positionOffset);

            Vector3Int offsetPos = GetOffsetPosition(worldPosition, rule.m_NeighborPositions[i]);

            uint neighbourID = World.GetBlockAtSafe(offsetPos.x, offsetPos.y, worldPosition.z);
            
            // Get the block at this position with the z-pos as the layer.
            uint myID = World.GetBlockAtUnsafe(worldPosition.x, worldPosition.y, worldPosition.z);

            if (!NevergreenMatch(neighbourRule, myID, neighbourID))
            {
                //Debug.Log("False match");
                return false;
            }
        }
        
        return true;
    }

    private static bool NevergreenMatch(int rule, uint myID, uint neighbourID)
    {
        switch (rule)
        {
            case TilingRuleOutput.Neighbor.This:
                return myID == neighbourID;
            
            case TilingRuleOutput.Neighbor.NotThis:
                return myID != neighbourID;
            
            case Neighbor.Nothing:
                return neighbourID == 0;
            
            case Neighbor.Something:
                return neighbourID != 0;
        }
        return true;
    }
    
    // Tilemap origin is at the center of the chunk. Position is relative to the tilemap origin.
    // Could GetComponent<Chunk>().Position to get the real pos.
    // Could maybe also set the Chunk's tilemap transform position to 0,0 at chunk start?
    public override void RefreshTile(Vector3Int tileLocalPos, ITilemap tilemap)
    {
        tilemap.RefreshTile(tileLocalPos);

        Vector3 tilemapWorldPos = tilemap.GetComponent<Transform>().position;
        
        Vector3Int worldPos = tileLocalPos + new Vector3Int((int)tilemapWorldPos.x, (int)tilemapWorldPos.y, 0);

        if (true)
        {
            // Get the block at this position with the z-pos as the layer.
            uint myID = World.GetBlockAtUnsafe(worldPos.x, worldPos.y, worldPos.z);
            
            foreach (Vector3Int neighbour in AllNeighbours)
            {
                Vector3Int offsetPos = worldPos + neighbour;
                
                // Faster with checks, tested.
                if (World.GetBlockAtSafe(offsetPos, worldPos.z) == myID)
                {
                    World.RefreshTileAtSafe(worldPos + neighbour, worldPos.z);
                }
            }
        }
        /*else if(false)
        {
            //Warn: No idea if needed
            Tilemap baseTilemap = tilemap.GetComponent<Tilemap>();
            
            ReleaseDestroyedTilemapCacheData(); // Prevent memory leak
    
            //TODO: Determine if we are at the edge of an chunk, if we are, perform below func for that chunk's tilemap?
            //Warn: No idea if needed
            if (IsTilemapUsedTilesChange(baseTilemap, out KeyValuePair<HashSet<TileBase>, HashSet<Vector3Int>> neighborPositionsSet))
                neighborPositionsSet = CachingTilemapNeighborPositions(baseTilemap);
    
            HashSet<Vector3Int> neighborPositionsRuleTile = neighborPositionsSet.Value;
            
            /*Vector3Int[] neighborPositionsRuleTile = new[]
            {
                new Vector3Int(1, 0),
                new Vector3Int(-1, 0),
                new Vector3Int(0, 1),
                new Vector3Int(0, -1)
            };#1#
            
            foreach (Vector3Int offset in neighborPositionsRuleTile)
            {
                //Vector3Int offsetPosition = GetOffsetPositionReverse(tileLocalPos, offset);
                Vector3Int wPosition = GetOffsetPositionReverse(worldPos, offset);
                
                World.RefreshTileAtSafe(wPosition, wPosition.z);
                
                /*Tilemap tMap = World.GetTilemapAtSafe(wPosition.x, wPosition.y, wPosition.z);
                
                if(tMap == null) continue;
                
                //WARN: GetTile 's can be fully replaced with BlockData dict access.
                TileBase tile = tMap.GetTile(offsetPosition);   //TODO: Offset position might not be correct for the neighbouring chunk, test if better with the custom func
                //TileBase tile = tilemap.GetTile(offsetPosition);
                
                //tMap.RefreshTile(offsetPosition);
                
                RuleTile ruleTile = null;
    
                if (tile is RuleTile rTile)
                    ruleTile = rTile;
                else if (tile is RuleOverrideTile oTile)
                    ruleTile = oTile.m_Tile;
    
                if (ruleTile == null) continue;
                
                if (ruleTile == this || ruleTile.neighborPositions.Contains(offset))
                    tMap.RefreshTile(offsetPosition);#1#
            }
        }*/
    }
}