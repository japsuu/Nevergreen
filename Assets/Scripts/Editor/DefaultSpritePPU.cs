using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DefaultSpritePPU : AssetPostprocessor
{
    private void OnPreprocessTexture ()
    {
        TextureImporter textureImporter  = (TextureImporter) assetImporter;
        textureImporter.spritePixelsPerUnit = 16;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.isReadable = true;
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
    }
}
