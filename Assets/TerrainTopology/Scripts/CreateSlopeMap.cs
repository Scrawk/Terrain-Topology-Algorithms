using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{

    public class CreateSlopeMap : CreateTopology
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

            Texture2D slopeMap = new Texture2D(width, height, TextureFormat.ARGB32, false, true);

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

                    float g = Mathf.Sqrt(dx * dx + dy * dy);
                    float slope = g / Mathf.Sqrt(1.0f + g * g);

                    slopeMap.SetPixel(x, y, new Color(slope, slope, slope, 1));
                }

            }

            slopeMap.Apply();
            m_material.mainTexture = slopeMap;
        }

    }

}
