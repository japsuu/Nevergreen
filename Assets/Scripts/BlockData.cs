using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
// ReSharper disable InconsistentNaming

namespace Nevergreen
{
    /// <summary>
    /// Only one instance of this class will ever exist for each tile.
    ///
    /// Instances are loaded from the file directory.
    /// 
    /// Contains purely the block related data.
    /// </summary>
    [Serializable]
    public class BlockData : ItemBaseData
    {
        public BlockData()
        {
            MaxStackSize = 999;
            Consumable = true;
            AutoReuse = true;
            UseAnimationLength = 0.1f;
            UseTime = 0.1f;
            UseStyle = ItemUseStyle.Swing;
            CanBePlacedOnForeground = true;
            CanBePlacedOnBackground = true;
            CanBePlacedOnMidground = false;
        }

        [JsonIgnore]
        [HideInInspector]
        public TileBase Tile;

        [JsonIgnore]
        [FoldoutGroup("VSplit/Block properties", false)]
        public Texture2D TilesetTexture;   //TODO: Implement system where the type of tile (AutoRule, animated, normal) is saved.
        
        [VerticalGroup("VSplit", 55)]
        public BlockGenerationData GenerationData;

        public bool ShouldSerializeGenerationData()
        {
            return GenerationData.CanGenerate();
        }
        
        /*[SuffixLabel("Block\nsprite", true)]
        [HorizontalGroup("BlockSplit", 55), PropertyOrder(-1)]
        [PreviewField(50, ObjectFieldAlignment.Left), HideLabel]
        [JsonIgnore]
        public Sprite BlockSprite;*/
        
        [FoldoutGroup("VSplit/Block properties", false)]
        public bool CanBePlacedOnForeground = false;
        
        [FoldoutGroup("VSplit/Block properties", false)]
        public bool CanBePlacedOnMidground = false;
        
        [FoldoutGroup("VSplit/Block properties", false)]
        public bool CanBePlacedOnBackground = false;
        
        [FoldoutGroup("VSplit/Block properties", false)]
        public bool RequiresSupportFromBelow = false;

        /// <summary>
        /// Width in tiles, If not zero = is considered multi-tile.
        /// </summary>
        [FoldoutGroup("VSplit/Block properties", false)]
        public int TilesWidth = 1;
        
        /// <summary>
        /// Height in tiles, If not zero = is considered multi-tile.
        /// </summary>
        [FoldoutGroup("VSplit/Block properties", false)]
        public int TilesHeight = 1;
        
        public float GetGenerationDataSetting(BlockGenerationData.Setting setting)
        {
            return setting switch
            {
                BlockGenerationData.Setting.MinimumDepth => GenerationData.MinimumDepth,
                BlockGenerationData.Setting.MaximumDepth => GenerationData.MaximumDepth,
                BlockGenerationData.Setting.PerlinSpeed => GenerationData.PerlinSpeed,
                BlockGenerationData.Setting.PerlinLevel => GenerationData.PerlinLevel,
                BlockGenerationData.Setting.ZonePerlinSpeed => GenerationData.ZonePerlinSpeed,
                BlockGenerationData.Setting.ZonePerlinLevel => GenerationData.ZonePerlinLevel,
                BlockGenerationData.Setting.MapPerlinSpeed => GenerationData.MapPerlinSpeed,
                BlockGenerationData.Setting.MapPerlinLevel => GenerationData.MapPerlinLevel,
                _ => throw new ArgumentOutOfRangeException(nameof(setting), setting, null)
            };
        }
        
        public void SetGenerationDataSetting(BlockGenerationData.Setting setting, float value)
        {
            switch (setting)
            {
                case BlockGenerationData.Setting.MinimumDepth:
                    GenerationData.MinimumDepth = value;
                    break;
                    
                case BlockGenerationData.Setting.MaximumDepth:
                    GenerationData.MaximumDepth = value;
                    break;
                    
                case BlockGenerationData.Setting.PerlinSpeed:
                    GenerationData.PerlinSpeed = value;
                    break;
                    
                case BlockGenerationData.Setting.PerlinLevel:
                    GenerationData.PerlinLevel = value;
                    break;
                    
                case BlockGenerationData.Setting.ZonePerlinSpeed:
                    GenerationData.ZonePerlinSpeed = value;
                    break;
                    
                case BlockGenerationData.Setting.ZonePerlinLevel:
                    GenerationData.ZonePerlinLevel = value;
                    break;
                    
                case BlockGenerationData.Setting.MapPerlinSpeed:
                    GenerationData.MapPerlinSpeed = value;
                    break;
                    
                case BlockGenerationData.Setting.MapPerlinLevel:
                    GenerationData.MapPerlinLevel = value;
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(setting), setting, null);
            }
        }
    }
}