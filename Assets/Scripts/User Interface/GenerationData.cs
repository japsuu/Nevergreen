using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// Handles the data of GridItems as scriptable objects.
/// </summary>
[CreateAssetMenu(menuName = "Blocks/SliderData")]
[Obsolete("Use BlockGenerationData")]
public class GenerationData : ScriptableObject
{
    /*
    // Data storage
    [FormerlySerializedAs("sliderData")]
    public List<float> generationData;

    // Use a single enum as retrieval label for any slider by using a splitter
    // value as interval.
    private const int ENUM_INTERVAL = 16;
    public enum SliderField
    {
        // Customizer sliders
        DEPTH_MIN,
        DEPTH_MAX,
        PERLIN_SPEED,
        PERLIN_LEVEL,
        ZONE_PERLIN_SPEED,
        ZONE_PERLIN_LEVEL,
        MAP_PERLIN_SPEED,
        MAP_PERLIN_LEVEL,

        // Surface        
        SURFACE_PERLIN_SPEED = ENUM_INTERVAL,
        SURFACE_HEIGHT_MULTIPLIER,
        SURFACE_AVG_HEIGHT_MULTIPLIER,

        // Chunks
        CHUNK_SIZE = ENUM_INTERVAL * 2,
        CHUNK_RADIUS_HORIZONTAL,
        CHUNK_RADIUS_VERTICAL
    }


    /// <summary>
    /// Retrieves the value of the slider belonging to the given field constant.
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public float GetSliderData(SliderField field)
    {
        return generationData[(int)field % ENUM_INTERVAL];
    }


    /// <summary>
    /// Sets the value of the slider belonging to the given field constant.
    /// </summary>
    /// <param name="field"></param>
    /// <param name="value"></param>
    public void SetSliderData(SliderField field, float value)
    {
        generationData[(int)field % ENUM_INTERVAL] = value;
    }*/
}