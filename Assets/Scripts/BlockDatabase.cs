using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nevergreen;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Blocks/Block Database", fileName = "BlockDatabase")]
public class BlockDatabase : ScriptableObject
{
    public Dictionary<uint, BlockData> BlockDataLookup;
    
#if UNITY_EDITOR
    [InfoBox("Conflicting IDs detected!", InfoMessageType.Error, "hasConflictingIDs")]
    [Searchable(FilterOptions = SearchFilterOptions.ValueToString)]
    [SerializeField]
    private List<BlockData> officialBlocks;

    private bool hasConflictingIDs = false;

    [Button(ButtonSizes.Large, Name = "Save Database")]
    private void SaveDatabase()
    {
        LoadHelper.SaveOfficialBlockData(officialBlocks);
    }

    //TODO: Optimize
    private void OnValidate()
    {
        foreach (BlockData block in officialBlocks)
        {
            hasConflictingIDs = officialBlocks.Any(b => (b.ID == block.ID) && (b != block));
        }
    }
    
#endif
}
