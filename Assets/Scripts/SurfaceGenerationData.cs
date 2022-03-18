using System;
using UnityEngine;

namespace Nevergreen
{
    [CreateAssetMenu(menuName = "Generation/Surface Generation Data", fileName = "SurfaceGenerationData")]
    public class SurfaceGenerationData : ScriptableObject
    {
        public float SurfacePerlinSpeed = 0.022f;
        public float SurfaceHeightMultiplier = 38f;
        public float SurfaceAvgHeightMultiplier = 6;

        public enum Setting
        {
            SurfacePerlinSpeed,
            SurfaceHeightMultiplier,
            SurfaceAvgHeightMultiplier
        }
        
        public float GetGenerationDataSetting(Setting setting)
        {
            return setting switch
            {
                Setting.SurfacePerlinSpeed => SurfacePerlinSpeed,
                Setting.SurfaceHeightMultiplier => SurfaceHeightMultiplier,
                Setting.SurfaceAvgHeightMultiplier => SurfaceAvgHeightMultiplier,
                _ => throw new ArgumentOutOfRangeException(nameof(setting), setting, null)
            };
        }
        
        public void SetGenerationDataSetting(Setting setting, float value)
        {
            switch (setting)
            {
                case Setting.SurfacePerlinSpeed:
                    SurfacePerlinSpeed = value;
                    break;
                case Setting.SurfaceHeightMultiplier:
                    SurfaceHeightMultiplier = value;
                    break;
                case Setting.SurfaceAvgHeightMultiplier:
                    SurfaceAvgHeightMultiplier = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(setting), setting, null);
            }
        }
    }
}