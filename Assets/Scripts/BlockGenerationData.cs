using System;
using System.Collections.Generic;

namespace Nevergreen
{
    [Serializable]
    public struct BlockGenerationData
    {
        public float MinimumDepth;
        public float MaximumDepth;
        public float PerlinSpeed;
        public float PerlinLevel;
        public float ZonePerlinSpeed;
        public float ZonePerlinLevel;
        public float MapPerlinSpeed;
        public float MapPerlinLevel;

        /// <summary>
        /// If this block can be generated in the world.
        /// </summary>
        /// <returns>If all the generation parameters are 0.</returns>
        public bool CanGenerate()
        {
            return
                !(MinimumDepth == 0 &&
                MaximumDepth == 0 &&
                PerlinSpeed == 0 &&
                PerlinLevel == 0 &&
                ZonePerlinSpeed == 0 &&
                ZonePerlinLevel == 0 &&
                MapPerlinSpeed == 0 &&
                MapPerlinLevel == 0);
        }
        
        public enum Setting
        {
            MinimumDepth,
            MaximumDepth,
            PerlinSpeed,
            PerlinLevel,
            ZonePerlinSpeed,
            ZonePerlinLevel,
            MapPerlinSpeed,
            MapPerlinLevel
        }
    }
}