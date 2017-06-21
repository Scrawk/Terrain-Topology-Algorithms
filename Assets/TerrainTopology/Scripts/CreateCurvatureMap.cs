using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{
    public enum CURVATURE_TYPE { HORIZONTAL, VERTICAL, AVERAGE };

    public class CreateCurvatureMap : CreateTopology
    {

        public float m_limit = 10000;

        public CURVATURE_TYPE m_curvatureType = CURVATURE_TYPE.AVERAGE;

        protected override void CreateMap(float[] heights, int width, int height)
        {

            float ux = 1.0f / (width - 1.0f);
            float uy = 1.0f / (height - 1.0f);

            Texture2D curveMap = new Texture2D(width, height, TextureFormat.ARGB32, false, true);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    int xp1 = (x == width - 1) ? x : x + 1;
                    int xn1 = (x == 0) ? x : x - 1;

                    int yp1 = (y == height - 1) ? y : y + 1;
                    int yn1 = (y == 0) ? y : y - 1;

                    float v = heights[x + y * width];

                    float l = heights[xn1 + y * width];
                    float r = heights[xp1 + y * width];

                    float b = heights[x + yn1 * width];
                    float t = heights[x + yp1 * width];

                    float lb = heights[xn1 + yn1 * width];
                    float lt = heights[xn1 + yp1 * width];

                    float rb = heights[xp1 + yn1 * width];
                    float rt = heights[xp1 + yp1 * width];

                    float dx = (r - l) / (2.0f * ux);
                    float dy = (t - b) / (2.0f * uy);

                    float dxx = (r - 2.0f * v + l) / (ux * ux);
                    float dyy = (t - 2.0f * v + b) / (uy * uy);

                    float dxy = (rt - rb - lt + lb) / (4.0f * ux * uy);

                    float curve = 0.0f;

                    switch (m_curvatureType)
                    {
                        case CURVATURE_TYPE.HORIZONTAL:
                            curve = Horizontal(dx, dy, dxx, dyy, dxy);
                            break;

                        case CURVATURE_TYPE.VERTICAL:
                            curve = Vertical(dx, dy, dxx, dyy, dxy);
                            break;

                        case CURVATURE_TYPE.AVERAGE:
                            curve = Average(dx, dy, dxx, dyy, dxy);
                            break;
                    }

                    curveMap.SetPixel(x, y, new Color(curve, curve, curve, 1));
                }

            }

            curveMap.Apply();
            m_material.mainTexture = curveMap;
        }

        private float Horizontal(float dx, float dy, float dxx, float dyy, float dxy)
        {
            float kh = -2.0f * (dy * dy * dxx + dx * dx * dyy - dx * dy * dxy);
            kh /= dx * dx + dy * dy;

            if (float.IsInfinity(kh) || float.IsNaN(kh)) kh = 0.0f;

            if (kh < -m_limit) kh = -m_limit;
            if (kh > m_limit) kh = m_limit;

            kh /= m_limit;
            kh = kh * 0.5f + 0.5f;

            return kh;
        }

        private float Vertical(float dx, float dy, float dxx, float dyy, float dxy)
        {
            float kv = -2.0f * (dx * dx * dxx + dy * dy * dyy + dx * dy * dxy);
            kv /= dx * dx + dy * dy;

            if (float.IsInfinity(kv) || float.IsNaN(kv)) kv = 0.0f;

            if (kv < -m_limit) kv = -m_limit;
            if (kv > m_limit) kv = m_limit;

            kv /= m_limit;
            kv = kv * 0.5f + 0.5f;

            return kv;
        }

        private float Average(float dx, float dy, float dxx, float dyy, float dxy)
        {
            float kh = Horizontal(dx, dy, dxx, dyy, dxy);
            float kv = Vertical(dx, dy, dxx, dyy, dxy);

            return (kh + kv) * 0.5f;
        }

    }

}
