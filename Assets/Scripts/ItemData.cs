using Newtonsoft.Json;
using UnityEngine;

namespace Nevergreen
{
    public class ItemData : ItemBaseData
    {
        [JsonIgnore]
        public Sprite ItemSprite;
        
        /// <summary>
        /// How heavy/light this item is, affects run speed.
        /// </summary>
        public int Weight = 0;

        /// <summary>
        /// For how many coins can this item be bought from a shop.
        /// </summary>
        public int BuyPrice = 0;
        
        /// <summary>
        /// For how many coins can this item be sold to a shop.
        /// </summary>
        public int SellPrice = 0;

        /// <summary>
        /// Width of the hitbox when dropped to the ground.
        /// </summary>
        public float HitboxWidth = 0.5f;
        
        /// <summary>
        /// Height of the hitbox when dropped to the ground.
        /// </summary>
        public float HitboxHeight = 0.5f;

        /// <summary>
        /// The size multiplier of the item's sprite.
        /// </summary>
        public float ScaleMultiplier = 1f;
        
        /// <summary>
        /// Knockback inflicted by the item.
        /// </summary>
        public float KnockBack = 0f;

        /// <summary>
        /// ID of the ammunition this item will use.
        /// </summary>
        public int AmmoID = -1;

        /// <summary>
        /// Velocity of the projectiles fired from this item.
        /// </summary>
        public float ProjectileVelocity = 0;

        /// <summary>
        /// If true, the item will not inflict melee damage on use.
        /// </summary>
        public bool NoMeleeDamage = false;

        /// <summary>
        /// How efficiently is this item used as a pickaxe.
        /// </summary>
        public int PickaxePower = 0;
        
        /// <summary>
        /// How efficiently is this item used as a axe.
        /// </summary>
        public int AxePower = 0;

        //TODO: Might require fields for different equipment slots.
        /// <summary>
        /// Whether this item can be equipped as an accessory.
        /// </summary>
        public bool IsAccessory = false;

        /// <summary>
        /// Amount of defence this item provides when equipped.
        /// </summary>
        public int Defence = 0;

        /// <summary>
        /// Chance to inflict a critical attack.
        /// </summary>
        public float CritChance = 0f;

        /// <summary>
        /// If true, this item should have ammo specific behaviour (like whether it can be placed in the ammo slot).
        /// </summary>
        public bool IsAmmo = false;

        /// <summary>
        /// If true, this item should have potion specific behaviour (potion cooldown etc.)
        /// </summary>
        public bool IsPotion = false;

        /// <summary>
        /// How much stamina this item consumes/adds when used. Can be negative.
        /// </summary>
        public int StaminaChange = 0;

        /// <summary>
        /// How much this item heals/damages on use. Can be negative.
        /// </summary>
        public int HealAmount = 0;

        //TODO: Might have serialization issues? Implement a separate recipe system.
        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        /// ID's of the items required to craft this item. First dimension is required item ID, second is count.
        /// </summary>
        //public int[,] recipeRequiredItems;

        /// <summary>
        /// ID of the NPC that will be spawned on use.
        /// </summary>
        public int SpawnNpc = -1;
    }
}