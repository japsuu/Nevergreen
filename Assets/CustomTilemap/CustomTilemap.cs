using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 
/// </summary>
public class CustomTilemap : MonoBehaviour
{
    public bool gpuMethod = true;
    
    [Header("Texture atlas")]
    // Needs to be in the RGBA32-format!!!
    public Texture2D textureAtlas;
    public int textureSize = 16;

    [Header("World")]
    public int worldWidth = 48;
    public int worldHeight = 48;
    
    [Header("References")]
    public RawImage rawOverlayImage;
    public Image overlayImage;
    public RectTransform canvasTransform;

    private Texture2D targetTexture;
    
    private Texture2D atlasIndexMap;
    
    
    private static readonly int TargetTexProperty = Shader.PropertyToID("_WorldTex");
    private static readonly int AtlasTexProperty = Shader.PropertyToID("_AtlasTex");
    private static readonly int AtlasIndexTexProperty = Shader.PropertyToID("_AtlasIndexTex");

    private uint[,] world;
    private Color32[] atlasIndexes;

    private Dictionary<uint, Color32> colorLookup = new Dictionary<uint, Color32>();


    //TODO: Block texture ID's should be assigned to the BlockData once initialized and atlas created
    

    private void Start()
    {
        Initialize();
        
        //                     R=x, G=y, B=NA, A=transparency
        colorLookup.Add(0, new Color32(0, 0, 0, 0));
        colorLookup.Add(1, new Color32(1, 0, 0, 1));
        colorLookup.Add(2, new Color32(2, 0, 0, 1));
        colorLookup.Add(3, new Color32(3, 0, 0, 1));
        colorLookup.Add(4, new Color32(0, 1, 0, 1));
        colorLookup.Add(5, new Color32(1, 1, 0, 1));
        colorLookup.Add(6, new Color32(2, 1, 0, 1));
        
        int index = 0;
        // Create a sample world
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                // Select a random block ID
                uint rnd = (uint) Random.Range(0, 7);
                
                world[x, y] = rnd;
                
                atlasIndexes[index] = colorLookup[rnd];
                    
                index++;
            }
        }
    }

    private void Update()
    {
        UpdateWorldTexture();
    }

    [Button(ButtonSizes.Large, Name = "Stop")]
    private void Stop()
    {
        StopAllCoroutines();
        rawOverlayImage.texture = null;
    }

    private void UpdateWorldTexture()
    {
        if (gpuMethod)
        {
            atlasIndexMap.SetPixels32(atlasIndexes);
            
            atlasIndexMap.Apply(false);
            
            SendTexturesToGPU();
        }
        else
        {
            // Assign data to the GPU
            for (int y = 0; y < worldHeight; y++)
            {
                for (int x = 0; x < worldWidth; x++)
                {
                    // Select the correct texture based on block ID
                    //NOTE: In a real game, we would not use the block's ID, rather the Atlas offset assigned when the block asset is created.
                    int atlasOffset = (int)world[x, y] * textureSize;
                
                    Graphics.CopyTexture(textureAtlas, 0, 0, atlasOffset, 0, textureSize, textureSize,
                        targetTexture, 0, 0, x * textureSize, y * textureSize);
                }
            }
        }
    }

    private void OnValidate()
    {
        if(canvasTransform == null) return;
        
        canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, worldWidth);
        canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, worldHeight);

        canvasTransform.position = new Vector3(worldWidth / 2f, worldHeight / 2f);
    }

    private void SendTexturesToGPU()
    {
        overlayImage.material.SetTexture(AtlasIndexTexProperty, atlasIndexMap);
    }

    private void Initialize()
    {
        world = new uint[worldWidth, worldHeight];
        atlasIndexes = new Color32[worldWidth * worldHeight];

        targetTexture = new Texture2D(
            worldWidth * textureSize,
            worldHeight * textureSize,
            TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };

        atlasIndexMap = new Texture2D(
            worldWidth,
            worldHeight,
            TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        
        canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, worldWidth);
        canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, worldHeight);

        canvasTransform.position = new Vector3(worldWidth / 2f, worldHeight / 2f);

        if (gpuMethod)
        {
            rawOverlayImage.gameObject.SetActive(false);
            
            overlayImage.material.SetTexture(AtlasTexProperty, textureAtlas);
            overlayImage.material.SetTexture(TargetTexProperty, targetTexture);
        }
        else
        {
            overlayImage.gameObject.SetActive(false);

            rawOverlayImage.texture = targetTexture;
        }
    }
}
