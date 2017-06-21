using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{
    public enum ASPECT_TYPE { ASPECT, NORTHERNESS, EASTERNESS };

    public class CreateAspectMap : CreateTopology
    {

        public ASPECT_TYPE m_aspectType = ASPECT_TYPE.ASPECT;

        protected override void CreateMap(float[] heights, int width, int height)
        {

            float ux = 1.0f / (width - 1.0f);
            float uy = 1.0f / (height - 1.0f);

            Texture2D aspectMap = new Texture2D(width, height, TextureFormat.ARGB32, false, true);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    int xp1 = (x == width - 1) ? x : x + 1;
                    int xn1 = (x == 0) ? x : x - 1;

                    int yp1 = (y == height - 1) ? y : y + 1;
                    int yn1 = (y == 0) ? y : y - 1;

                    float l = heights[xn1 + y * width];
                    float r = heights[xp1 + y * width];

                    float b = heights[x + yn1 * width];
                    float t = heights[x + yp1 * width];

                    float dx = (r - l) / (2.0f * ux);
                    float dy = (t - b) / (2.0f * uy);

                    float m = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Acos(-dy / m) * Mathf.Rad2Deg;

                    if (float.IsInfinity(a) || float.IsNaN(a))
                        a = 0.0f;

                    float aspect = 180.0f * (1.0f + Sign(dx)) - Sign(dx) * a;

                    switch(m_aspectType)
                    {
                        case ASPECT_TYPE.NORTHERNESS:
                            aspect = Mathf.Cos(aspect * Mathf.Deg2Rad);
                            aspect = aspect * 0.5f + 0.5f;
                            break;

                        case ASPECT_TYPE.EASTERNESS:
                            aspect = Mathf.Sin(aspect * Mathf.Deg2Rad);
                            aspect = aspect * 0.5f + 0.5f;
                            break;

                        default:
                            aspect /= 360.0f;
                            break;
                    }

                    aspectMap.SetPixel(x, y, new Color(aspect, aspect, aspect, 1));
                }

            }

            aspectMap.Apply();
            m_material.mainTexture = aspectMap;
        }

        private float Sign(float v)
        {
            if (v > 0) return 1;
            if (v < 0) return -1;
            return 0;
        }

    }

}
