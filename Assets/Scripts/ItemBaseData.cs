using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Nevergreen
{
    /// <summary>
    ///
    /// Describes and item which can be held in hand, or in inventory.
    /// 
    /// Provides the base data for any time of an item.
    /// Should only be instantiated once, inside ItemDatabase.cs.
    /// Values should not be modified externally, for modifiers modify the ItemBase.
    ///
    /// Sprite(s) are fetched from a folder, and the sprite name should match the ID.
    /// 
    /// There should be ID_ingame.png & ID_ui.png
    /// 
    /// </summary>
    [Serializable]
    public abstract class ItemBaseData
    {
        [SuffixLabel("UI sprite", true)]
        [HorizontalGroup("Split", 55), PropertyOrder(-2)]
        [PreviewField(50, ObjectFieldAlignment.Left), HideLabel]
        [JsonIgnore]
        public Sprite UISprite;
        
        /// <summary>
        /// Unique ID of this item.
        /// </summary>
        //[FoldoutGroup("Split/Item Properties", false)]
        [JsonProperty(Order = -100)]
        public uint ID = 1;
        
        /// <summary>
        /// Name of this item.
        /// </summary>
        [JsonProperty(Order = -100)]
        public string Name = "Unknown";

        /// <summary>
        /// Description of this item.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public string Description = "I have no idea what this is";

        /// <summary>
        /// Rarity of the item.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public ItemRarityClass Rarity = ItemRarityClass.Common;

        /// <summary>
        /// The maximum number of items in a stack of this type.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public uint MaxStackSize = 1;

        /// <summary>
        /// Whether this item is consumed when used.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public bool Consumable = false;

        /// <summary>
        /// ID of the use sound played when item is used.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public uint UseSoundID = 0;

        /// <summary>
        /// Whether the item can be used continuously while the use key is down.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public bool AutoReuse = true;

        /// <summary>
        /// Length of the use-animation.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public float UseAnimationLength = 0.5f;

        /// <summary>
        /// How often can this item be used.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public float UseTime = 0.5f;

        /// <summary>
        /// Style of animation played when the item is used.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public ItemUseStyle UseStyle = ItemUseStyle.Swing;

        /// <summary>
        /// Type of buff given to player on use/when walked on.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public BuffType Buff = BuffType.None;

        /// <summary>
        /// Time the buff is given for.
        /// </summary>
        [JsonProperty(Order = -100)]
        [FoldoutGroup("Split/Item Properties", false)]
        public int BuffTime = 3;
    }
}