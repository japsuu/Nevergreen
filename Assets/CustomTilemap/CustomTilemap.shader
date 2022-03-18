Shader "Unlit/CustomTilemap"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        _WorldTex ("World Texture", 2D) = "white"
        _AtlasTex ("Atlas Texture", 2D) = "white"
        _AtlasIndexTex ("Atlas Index Texture", 2D) = "white"
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vertex_input
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragment_input
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            //sampler2D _MainTex;
            
            sampler2D _WorldTex;
            sampler2D _AtlasTex;
            sampler2D _AtlasIndexTex;
            float4 _WorldTex_ST;

            fragment_input vert (vertex_input IN)
            {
                fragment_input o;
                o.vertex = UnityObjectToClipPos(IN.vertex);
                o.uv = TRANSFORM_TEX(IN.uv, _WorldTex);

                return o;
            }

            float4 frag (fragment_input IN) : SV_Target
            {
                const float atlas_width = 64;
                const float atlas_height = 64;
                const float block_dimensions = 16;
                const float world_width_block_space = 512;
                const float textures_in_atlas_x = atlas_width / block_dimensions;
                const float textures_in_atlas_y = atlas_height / block_dimensions;
                const float world_width_pixel_space = world_width_block_space * block_dimensions;

                // Decode the index of the block from the RGB channels of the AtlasIndex -texture.
                // Returns 0/1/2 when using an atlas with three textures.
                float4 index_sample = tex2D(_AtlasIndexTex, IN.uv);

                //TODO: Setup wrapping/etc so that textureID -map can contain AIR tiles.
                float2 atlas_index = index_sample.rg * 255; //WARN: 255 or 256, not sure

                // Get the current position in the world based on the UVs, in the pixel space.
                // Returns (0 - worldWidthInPixels, 0 - worldWidthInPixels)
                float2 position_pixel_space = IN.uv.xy * world_width_pixel_space;

                // Returns repeating 0-1 UV coordinates for every block.
                float2 position_on_atlas_block_uv_space = float2(
                    fmod(position_pixel_space.x, block_dimensions) / block_dimensions / textures_in_atlas_x + (atlas_index.x / textures_in_atlas_x),
                    fmod(position_pixel_space.y, block_dimensions) / block_dimensions / textures_in_atlas_y + (atlas_index.y / textures_in_atlas_y));

                return tex2D(_AtlasTex, position_on_atlas_block_uv_space);

                /*      -- NOTE -- : This code could be used if "Border MipMaps -setting" is undesired. Manually assigns the derivatives for the MipMaps.
                float mip_bias = -2;

                // per pixel screen space partial derivatives
                float2 dx = ddx(position_on_atlas_block_uv_space);
                float2 dy = ddy(position_on_atlas_block_uv_space);
                // bias scale
                float bias = pow(2, mip_bias);

                return tex2Dgrad(_AtlasTex, position_on_atlas_block_uv_space, dx * bias, dy * bias);
                */

                // Return the UV coords mapped to R & G -channels for debugging.
                //float4 debug_val = float4(position_on_atlas_block_uv_space.x, position_on_atlas_block_uv_space.y, 0, 1);
                //return debug_val;
                
                //return tex2D(_AtlasTex, position_on_atlas_block_uv_space);
                //float2 position_block_space = IN.uv.xy * world_width_block_space;   // Returns (0 - 48, 0 - 48)

                // If this block index = 0, return this block index + additional pixel offset based on UV

                // Get the current pixel on the atlas with atlasOrigin + fmod(i.uv.x * worldWidth, 16)

                //float x_offset = (1 - (atlas_width - atlasXorigin - 1) / atlas_width);
                //float y_offset = (1 - (block_dimensions - atlasXorigin - 1) / block_dimensions);
                //return tex2Dlod(_AtlasTex, float4(x_offset, y_offset, 0, 0));







                
                //fixed4 col = tex2Dlod(_AtlasTex, float4(x_offset, y_offset, 0, 0));
                //float4 col = tex2D(_AtlasTex, float2(IN.uv.x / (atlas_width / block_dimensions), IN.uv.y));
                //float4 col = tex2Dlod(_AtlasTex, float4(2 * (IN.uv.x / (atlas_width / block_dimensions)), IN.uv.y, 0, 0));
                //float4 col = tex2Dlod(_AtlasTex, float4(atlasXorigin + fmod(IN.uv.x * world_width, 16), IN.uv.y * world_width * 3, 0, 0));
                
                //return col;
            }
            ENDCG
        }
    }
}
