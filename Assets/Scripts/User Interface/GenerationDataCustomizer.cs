using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Extends SliderData to allow more detailed data for Customizer sliders.
/// </summary>
[CreateAssetMenu(menuName = "Blocks/SliderDataCustomizer")]
[Obsolete("Use BlockData")]
public class GenerationDataCustomizer : GenerationData
{
    public string itemName;
    public Tile itemTile;
    public uint itemType;
    public Sprite gridSprite;
}
