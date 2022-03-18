using System;
using UnityEngine;

namespace Nevergreen
{
    public class CustomRuleTile
    {
        public Sprite activeSprite;
        
        private Sprite[] Sprites;

        private NeighbourPositions currentNeighbours;
        
        [Flags]
        public enum NeighbourPositions
        {
            UpLeft = 1 << 0,    // = 0
            Up = 1 << 1,        // = 1
            UpRight = 1 << 2,   // = 2
            Left = 1 << 3,      // = 4
            Right = 1 << 4,     // = 8
            DownLeft = 1 << 5,  // = 16
            Down = 1 << 6,      // = 32
            DownRight = 1 << 7, // = 64
        }

        public CustomRuleTile(Sprite[] sprites)
        {
            if (sprites.Length != 49)
            {
                Debug.LogWarning("Tried to create a CustomRuleTile with invalid Sprite array length.");
                
                return;
            }
            
            Sprites = sprites;
        }

        /// <summary>
        /// Updates this tile's sprite, and also calls this function on all the neighbouring tiles if needed.
        /// </summary>
        public void Update(bool fromOtherTile)
        {
            NeighbourPositions neighbours =
                (upLeft ? NeighbourPositions.UpLeft : 0) |
                (upLeft ? NeighbourPositions.Up : 0) |
                (upLeft ? NeighbourPositions.UpRight : 0) |
                (upLeft ? NeighbourPositions.Left : 0) |
                (upLeft ? NeighbourPositions.Right : 0) |
                (upLeft ? NeighbourPositions.DownLeft : 0) |
                (upLeft ? NeighbourPositions.Down : 0) |
                (upLeft ? NeighbourPositions.DownRight : 0);
            // Now directions contains number 0 - 15 for all 16 possible combinations (256 after you add remaining 4 directions)
            activeSprite = Sprites[(int)neighbours];
        }
    }
}