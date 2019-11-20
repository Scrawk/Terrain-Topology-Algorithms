using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{

    public class CreateSlopeMap : CreateTopology
    {
        protected override bool OnChange()
        {
            return m_currentColorMode != m_coloredGradient;
        }

        protected override void CreateMap()
        {

            Texture2D slopeMap = new Texture2D(m_width, m_height);

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    Vector2 d1 = GetFirstDerivative(x, y);

                    float slope = Slope(d1.x, d1.y);

                    var color = Colorize(slope, 0.4f, true);

                    slopeMap.SetPixel(x, y, color);
                }

            }

            slopeMap.Apply();
            m_material.mainTexture = slopeMap;
        }

        private float Slope(float zx, float zy)
        {
            float p = zx * zx + zy * zy;
            float g = FMath.SafeSqrt(p);

            return Mathf.Atan(g) * FMath.Rad2Deg / 90.0f;
        }

    }

}
