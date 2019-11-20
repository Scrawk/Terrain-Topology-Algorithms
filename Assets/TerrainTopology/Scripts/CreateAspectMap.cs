using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{

    public class CreateAspectMap : CreateTopology
    {

        protected override bool OnChange()
        {
            return m_currentColorMode != m_coloredGradient;
        }

        protected override void CreateMap()
        {

            Texture2D aspectMap = new Texture2D(m_width, m_height);

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    Vector2 d1 = GetFirstDerivative(x, y);

                    float aspect = (float)Aspect(d1.x, d1.y);

                    var color = Colorize(aspect, 0, true);

                    aspectMap.SetPixel(x, y, color);
                }

            }

            aspectMap.Apply();
            m_material.mainTexture = aspectMap;
        }

        private float Aspect(float zx, float zy)
        {
            float gyx = FMath.SafeDiv(zy, zx);
            float gxx = FMath.SafeDiv(zx, Math.Abs(zx));

            float aspect = 180 - Mathf.Atan(gyx) * FMath.Rad2Deg + 90 * gxx;
            aspect /= 360;

            return aspect;
        }

    }

}
