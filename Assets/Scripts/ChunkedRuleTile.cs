using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/Chunked Rule Tile", fileName = "ChunkedRuleTile")]
[Obsolete("Not finished")]
public class ChunkedRuleTile : RuleTile<ChunkedRuleTile.Neighbor>
{
    public class Neighbor : RuleTile.TilingRuleOutput.Neighbor
    {
        public const int Null = 3;
        public const int NotNull = 4;
    }
    /*
    private Chunk.TilemapType typeDefinition;
    
    /// <summary>
    /// StartUp is called on the first frame of the running Scene.
    /// </summary>
    /// <param name="position">Position of the Tile on the Tilemap.</param>
    /// <param name="tilemap">The Tilemap the tile is present on.</param>
    /// <param name="instantiatedGameObject">The GameObject instantiated for the Tile.</param>
    /// <returns>Whether StartUp was successful</returns>
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
    {
        TilemapTypeDefinition definition = tilemap.GetComponent<TilemapTypeDefinition>();

        if (definition == null)
        {
            Debug.LogWarning("Tilemap " + tilemap.GetComponent<Transform>().gameObject.name + " is missing a " +
                             "TypeDefinition -component. Defaulting to foreground!");

            typeDefinition = Chunk.TilemapType.FOREGROUND;
        }
        else
        {
            typeDefinition = definition.TypeDefinition;
        }
        
        if (instantiatedGameObject == null) return true;
        
        Tilemap tmpMap = tilemap.GetComponent<Tilemap>();
        Matrix4x4 orientMatrix = tmpMap.orientationMatrix;

        var iden = Matrix4x4.identity;
        Vector3 gameObjectTranslation = new Vector3();
        Quaternion gameObjectRotation = new Quaternion();
        Vector3 gameObjectScale = new Vector3();

        bool ruleMatched = false;
        foreach (TilingRule rule in m_TilingRules)
        {
            Matrix4x4 transform = iden;
            if (RuleMatches(rule, position, tilemap, ref transform))
            {
                transform = orientMatrix * transform;

                // Converts the tile's translation, rotation, & scale matrix to values to be used by the instantiated GameObject
                gameObjectTranslation = new Vector3(transform.m03, transform.m13, transform.m23);
                gameObjectRotation = Quaternion.LookRotation(new Vector3(transform.m02, transform.m12, transform.m22), new Vector3(transform.m01, transform.m11, transform.m21));
                gameObjectScale = transform.lossyScale;

                ruleMatched = true;
                break;
            }
        }
        if (!ruleMatched)
        {
            // Fallback to just using the orientMatrix for the translation, rotation, & scale values.
            gameObjectTranslation = new Vector3(orientMatrix.m03, orientMatrix.m13, orientMatrix.m23);
            gameObjectRotation = Quaternion.LookRotation(new Vector3(orientMatrix.m02, orientMatrix.m12, orientMatrix.m22), new Vector3(orientMatrix.m01, orientMatrix.m11, orientMatrix.m21));
            gameObjectScale = orientMatrix.lossyScale;
        }

        instantiatedGameObject.transform.localPosition = gameObjectTranslation + tmpMap.CellToLocalInterpolated(position + tmpMap.tileAnchor);
        instantiatedGameObject.transform.localRotation = gameObjectRotation;
        instantiatedGameObject.transform.localScale = gameObjectScale;

        return true;
    }

    /// <summary>
    /// Does a Rule Match given a Tiling Rule and neighboring Tiles.
    /// </summary>
    /// <param name="rule">The Tiling Rule to match with.</param>
    /// <param name="position">Position of the Tile on the Tilemap.</param>
    /// <param name="tilemap">The tilemap to match with.</param>
    /// <param name="transform">A transform matrix which will match the Rule.</param>
    /// <returns>True if there is a match, False if not.</returns>
    public override bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
    {
        //TODO: Check if position is inside the current chunk. If not, set Tilemap to that side's chunk.
        
        if (RuleMatches(rule, position, tilemap, 0))
        {
            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
            return true;
        }

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
        }

        return false;
    }
    
    /// <summary>
    /// Checks if there is a match given the neighbor matching rule and a Tile with a rotation angle.
    /// </summary>
    /// <param name="rule">Neighbor matching rule.</param>
    /// <param name="position">Position of the Tile on the Tilemap.</param>
    /// <param name="tilemap">Tilemap to match.</param>
    /// <param name="angle">Rotation angle for matching.</param>
    /// <returns>True if there is a match, False if not.</returns>
    private new bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, int angle)
    {
        int minCount = Math.Min(rule.m_Neighbors.Count, rule.m_NeighborPositions.Count);
        for (int i = 0; i < minCount ; i++)
        {
            int neighbor = rule.m_Neighbors[i];
            Vector3Int positionOffset = GetRotatedPosition(rule.m_NeighborPositions[i], angle);
            
            //TODO: Check if tile is inside the tilemap (chunk)
            //Debug.Log(GetOffsetPosition(position, positionOffset));
            
            //TileBase other = tilemap.GetTile(GetOffsetPosition(position, positionOffset));

            if (ChunkLoadManager.Instance == null)
            {
                Debug.LogWarning("Could not find ChunkLoadManager.Instance!");
                return false;
            }

            Vector3Int pos = GetOffsetPosition(position, positionOffset);
            
            Tilemap otherMap = ChunkLoadManager.Instance.GetTilemapAt(pos, typeDefinition);

            TileBase other = otherMap == null ? null : otherMap.GetTile(pos);
            
            if (!RuleMatch(neighbor, other))
            {
                return false;
            }
        }
        return true;
    }*/
}