using System;
using System.Collections.Generic;
using System.IO;
using Nevergreen;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class LoadHelper
{
    #region PATH_VARIABLES

    private static string SavesRoot => Path.Combine(Application.persistentDataPath, "saves");
    
    // StreamingAssets -> content
    private static string ContentRoot => Path.Combine(Application.streamingAssetsPath, "content");
    
    // StreamingAssets -> content -> official
    private static string OfficialContentRoot => Path.Combine(ContentRoot, "official");
    
    // StreamingAssets -> content -> official -> textures
    private static string OfficialTexturesRoot => Path.Combine(OfficialContentRoot, "textures");
    
    // StreamingAssets -> content -> mods
    private static string ModsContentRoot => Path.Combine(ContentRoot, "mods");
    
    // StreamingAssets -> content -> mods -> textures
    private static string ModsTexturesRoot => Path.Combine(ModsContentRoot, "textures");
    
    
    // StreamingAssets -> content -> official -> blockdata.json
    private static string BlocksDataFile => Path.Combine(OfficialContentRoot, "blockdata.json");
    
    // StreamingAssets -> content -> official -> itemdata.json
    private static string ItemsDataFile => Path.Combine(OfficialContentRoot, "itemdata.json");

    #endregion

    private static RuleTile ruleTileTemplate;

    private static Sprite nullSprite;

    private const int AssetsPPU = 16;
    private const int TilesetWidth = 7;
    private const int TilesetHeight = 7;
    //private const int TileSpritePadding = 2;

    private const bool DefinedOrder = true;
    private const string RuleTileTemplateName = "OrderedNevergreenTile";

    #region PUBLIC_FUNCTIONS

    public static void SaveOfficialBlockData(IEnumerable<BlockData> items)
    {
        Initialize();
        
        // Write the blockData to json, and that to file
        string json = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(BlocksDataFile, json);

        int successfulSaves = 0;
        
        // Write all the textures to folder
        foreach (BlockData blockData in items)
        {
            if (blockData.UISprite == null)
            {
                Debug.LogWarning($"Block with ID {blockData.ID} ({blockData.Name}) is missing the UI texture! Skipping write...");
            }
            else
            {
                byte[] uiTex = blockData.UISprite.texture.EncodeToPNG();
            
                // Save to StreamingAssets/content/official/textures/ID_ui.png
                File.WriteAllBytes(GetUITexturePath(blockData.ID), uiTex);
            }
            
            if (blockData.TilesetTexture == null && blockData.ID != 0)
            {
                Debug.LogWarning($"Block with ID {blockData.ID} ({blockData.Name}) is missing the tileset texture! Skipping write...");
            }
            else
            {
                byte[] tilesetTex = blockData.TilesetTexture.EncodeToPNG();

                //TODO: Add support for frames.
                // Save to StreamingAssets/content/official/textures/ID.png
                if(tilesetTex != null)
                    File.WriteAllBytes(GetBaseTexturePath(blockData.ID), tilesetTex);
            }
            
            successfulSaves++;
        }
        
        Debug.Log("Wrote " + successfulSaves + " blocks to " + BlocksDataFile);
    }

    public static BlockData[] LoadOfficialBlockData()
    {
        Initialize();

        string json = File.ReadAllText(BlocksDataFile);

        BlockData[] data = JsonConvert.DeserializeObject<BlockData[]>(json);
        
        // Fetch all the sprites
        foreach (BlockData blockData in data)
        {
            // UI Texture
            blockData.UISprite = PNGToSprite(GetUITexturePath(blockData.ID));

            // Tileset Texture
            blockData.TilesetTexture = PNGToTexture(GetBaseTexturePath(blockData.ID));

            // Tile from the tileset
            blockData.Tile = TilesetToRuleTile(blockData.TilesetTexture, blockData.ID);

            if (blockData.Tile == null)
            {
                Debug.LogWarning($"There was an error setting the tile for the block with ID {blockData.ID} ({blockData.Name})!");
            }
            
            if(blockData.UISprite == null)
            {
                Debug.LogWarning($"Block with ID {blockData.ID} ({blockData.Name}) is missing the UI texture!");
            }
            
            if(blockData.TilesetTexture == null && blockData.ID != 0)
            {
                Debug.LogWarning($"Block with ID {blockData.ID} ({blockData.Name}) is missing the tileset texture!");
            }
        }
        
        Debug.Log($"Successfully loaded {data.Length} blocks.");

        return data;
    }

    #endregion

    #region PRIVATE_FUNCTIONS

    private static void Initialize()
    {
        Directory.CreateDirectory(OfficialTexturesRoot);
        Directory.CreateDirectory(ModsTexturesRoot);
        Directory.CreateDirectory(SavesRoot);
        
        ruleTileTemplate = Resources.Load<RuleTile>(RuleTileTemplateName);
        nullSprite = Resources.Load<Sprite>("nullsprite");

        if (ruleTileTemplate == null)
        {
            Debug.LogError("RuleTileTemplate cannot be found!");
        }
    }
    
    private static string GetUITexturePath(uint id)
    {
        return Path.Combine(OfficialTexturesRoot, $"{id}_ui.png");
    }

    private static string GetBaseTexturePath(uint id)
    {
        return Path.Combine(OfficialTexturesRoot, $"{id}.png");
    }
    
    private static Texture2D PNGToTexture(string pngPath)
    {
        if (!File.Exists(pngPath)) return null;
        
        byte[] fileData = File.ReadAllBytes(pngPath);
        
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        tex.filterMode = FilterMode.Point;

        return tex;
    }
    
    private static Sprite PNGToSprite(string pngPath)
    {
        Texture2D tex = PNGToTexture(pngPath);

        if (tex == null) return null;
        
        return TextureToSprite(tex);
    }

    private static Sprite TextureToSprite(Texture2D texture)
    {
        if (texture == null) return null;

        texture.filterMode = FilterMode.Point;

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0, 0),
            AssetsPPU);
    }

    private static Sprite[] TilesetToSprites(Texture2D tileset)
    {
        if (tileset.width != (AssetsPPU * TilesetWidth) ||
            tileset.height != (AssetsPPU * TilesetHeight))
        {
            Debug.LogError("Invalid Tileset size!");
            return null;
        }
        
        Sprite[] tiles = new Sprite[TilesetWidth * TilesetHeight];

        int ruleIndex = 0;

        // Loop all the tiles in the set (Left to right, top to bottom), convert them Tileset -> Texture -> Sprite -> Add to Array
        for (int y = TilesetHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < TilesetWidth; x++)
            {
                // Texture coordinates start at the texture's lower left corner
                Texture2D tileTex = new Texture2D(AssetsPPU, AssetsPPU);
                
                if (SystemInfo.copyTextureSupport == UnityEngine.Rendering.CopyTextureSupport.None)
                {
                    //WARN: Might cause a high GC alloc!
                    Color[] pixelBuffer = tileset.GetPixels(x * AssetsPPU, y * AssetsPPU, AssetsPPU, AssetsPPU);
                    tileTex.SetPixels(0, 0, AssetsPPU, AssetsPPU, pixelBuffer);
                }
                else
                {
                    Graphics.CopyTexture(tileset, 0, 0, x * AssetsPPU, y * AssetsPPU, AssetsPPU, AssetsPPU,
                        tileTex, 0, 0, 0, 0);
                }
                
                tileTex.Apply(true);
                
                tiles[ruleIndex] = TextureToSprite(tileTex);

                ruleIndex++;
            }
        }

        return tiles;
    }
    
    private static RuleTile TilesetToRuleTile(Texture2D tileset, uint tileID)
    {
        // Special operations for the AIR tile
        if (tileID == 0)
        {
            RuleTile tile = ScriptableObject.CreateInstance<RuleTile>();

            return tile;
        }
        
        // Splice the tileset in to a padded texture array
        Sprite[] sprites = TilesetToSprites(tileset);
        
        // Make a copy of the template tile and apply settings
        RuleTile newTile = ruleTileTemplate.CloneSO();
        
        if (sprites.Length != newTile.m_TilingRules.Count)
        {
            Debug.LogError("The Tileset doesn't have the same number of sprites than the Rule Tile template has rules. Template: " + newTile.m_TilingRules.Count + ", Tileset: " + sprites.Length);

            return null;
        }
        
        // Create all the tile rules
        for (int ruleIndex = 0; ruleIndex < newTile.m_TilingRules.Count; ruleIndex++)
        {
            // Special handling if order rule is predefined. Fuck me, this is painful.
            if (DefinedOrder)
            {
                newTile.m_TilingRules[ruleIndex].m_Sprites = ruleIndex switch   //TODO: Save the rule order as data to the BlockData, this way users can better define their own textures.
                {
                    0 => new[] {sprites[8]},
                    1 => new[] {sprites[1]},
                    2 => new[] {sprites[7]},
                    3 => new[] {sprites[9]},
                    4 => new[] {sprites[15]},
                    5 => new[] {sprites[0]},
                    6 => new[] {sprites[2]},
                    7 => new[] {sprites[14]},
                    8 => new[] {sprites[16]},
                    9 => new[] {sprites[17]},
                    10 => new[] {sprites[10]},
                    11 => new[] {sprites[3]},
                    12 => new[] {sprites[23]},
                    13 => new[] {sprites[22]},
                    14 => new[] {sprites[21]},
                    15 => new[] {sprites[4]},
                    16 => new[] {sprites[5]},
                    17 => new[] {sprites[11]},
                    18 => new[] {sprites[12]},
                    19 => new[] {sprites[6]},
                    20 => new[] {sprites[20]},
                    21 => new[] {sprites[27]},
                    22 => new[] {sprites[13]},
                    23 => new[] {sprites[19]},
                    24 => new[] {sprites[18]},
                    25 => new[] {sprites[25]},
                    26 => new[] {sprites[26]},
                    27 => new[] {sprites[28]},
                    28 => new[] {sprites[29]},
                    29 => new[] {sprites[30]},
                    30 => new[] {sprites[31]},
                    31 => new[] {sprites[32]},
                    32 => new[] {sprites[34]},
                    33 => new[] {sprites[35]},
                    34 => new[] {sprites[36]},
                    35 => new[] {sprites[37]},
                    36 => new[] {sprites[38]},
                    37 => new[] {sprites[40]},
                    38 => new[] {sprites[33]},
                    39 => new[] {sprites[39]},
                    40 => new[] {sprites[41]},
                    41 => new[] {sprites[47]},
                    42 => new[] {sprites[44]},
                    43 => new[] {sprites[45]},
                    44 => new[] {sprites[46]},
                    45 => new[] {sprites[48]},
                    46 => new[] {sprites[24]},
                    47 => new[] {/*sprites[42]*/ nullSprite},
                    48 => new[] {/*sprites[43]*/ nullSprite},
                    _ => newTile.m_TilingRules[24].m_Sprites
                };

                // Set the default sprite
                newTile.m_DefaultSprite = sprites[24];
            }
            /*else
            {
                newTile.m_TilingRules[ruleIndex].m_Sprites = new []{ sprites[ruleIndex] };
            
                // Set the default sprite
                newTile.m_DefaultSprite = sprites[24];
            }*/
        }

        //string path = "Assets/" + DateTime.Now.Millisecond + ".asset";
        //UnityEditor.AssetDatabase.CreateAsset(newTile, path);
        //UnityEditor.AssetDatabase.SaveAssets();
        //UnityEditor.AssetDatabase.Refresh();
        //Debug.Log("Saved debug tile to " + path);

        return newTile;
    }

    #endregion
}
