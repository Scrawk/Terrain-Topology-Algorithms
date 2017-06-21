using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{

    public class CreateNormalMap : CreateTopology
    {

        public float m_terrainWidth = 1;

        public float m_terrainHeight = 1;

        public float m_terrainLength = 1;

        protected override void CreateMap(float[] heights, int width, int height)
        {

            float ux = 1.0f / (width - 1.0f);
            float uy = 1.0f / (height - 1.0f);

            float scaleX = m_terrainHeight / m_terrainWidth;
            float scaleY = m_terrainHeight / m_terrainLength;

            Texture2D normalMap = new Texture2D(width, height, TextureFormat.ARGB32, false, true);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    int xp1 = (x == width - 1) ? x : x + 1;
                    int xn1 = (x == 0) ? x : x - 1;

                    int yp1 = (y == height - 1) ? y : y + 1;
                    int yn1 = (y == 0) ? y : y - 1;

                    float l = heights[xn1 + y * width] * scaleX;
                    float r = heights[xp1 + y * width] * scaleX;

                    float b = heights[x + yn1 * width] * scaleY;
                    float t = heights[x + yp1 * width] * scaleY;

                    float dx = (r - l) / (2.0f * ux);
                    float dy = (t - b) / (2.0f * uy);

                    Vector3 normal;
                    normal.x = -dx;
                    normal.y = dy;
                    normal.z = 1;
                    normal.Normalize();

                    Color pixel;
                    pixel.r = normal.x * 0.5f + 0.5f;
                    pixel.g = normal.y * 0.5f + 0.5f;
                    pixel.b = normal.z;
                    pixel.a = 1.0f;

                    normalMap.SetPixel(x, y, pixel);
                }

            }

            normalMap.Apply();
            m_material.mainTexture = normalMap;
        }

    }

}
