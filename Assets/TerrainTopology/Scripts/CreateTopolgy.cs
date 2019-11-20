
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{

    public enum VISUALIZE_GRADIENT { WARM, COOL, COOL_WARM, GREY_WHITE, GREY_BLACK, BLACK_WHITE };

    public abstract class CreateTopology : MonoBehaviour
    {

        public Material m_material;

        public bool m_coloredGradient;

        protected float m_terrainWidth;

        protected float m_terrainHeight;

        protected float m_terrainLength;

        protected int m_width = 1024;

        protected int m_height = 1024;

        protected float m_cellLength;

        protected float[] m_heights;

        private Texture2D m_posGradient, m_negGradient, m_gradient;

        protected bool m_currentColorMode;

        void Start()
        {

            if (m_material == null) return;

            string fileName = Application.dataPath + "/TerrainTopology/Heights.raw";
            m_heights = Load16Bit(fileName);
            
            //The loaded heights map is a 16 bit 1024 by 1024 raw image
            m_width = 1024;
            m_height = 1024;

            //The terrain is about a 10Km square and about 2Km from lowest to highest point.
            m_terrainWidth = 10000;
            m_terrainHeight = 2000;
            m_terrainLength = 10000;

            //That makes each pixel in height map about 10m in length.
            m_cellLength = 10;

            //Create color gradients to help visualize the maps.
            m_currentColorMode = m_coloredGradient;
            CreateGradients(m_coloredGradient);

            //If required smooth the heights.
            if (DoSmoothHeights())
                SmoothHeightMap();

            CreateMap();
        }

        void OnDestroy()
        {
            if (m_material == null) return;
            m_material.mainTexture = null;
        }

        void Update()
        {
            //If settings changed then recreate map.
            if (OnChange())
            {
                CreateGradients(m_coloredGradient);
                CreateMap();

                m_currentColorMode = m_coloredGradient;
            }
        }

        /// <summary>
        /// Default mode is nothing changes.
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnChange()
        {
            return false;
        }

        /// <summary>
        /// Default mode is no smoothing.
        /// </summary>
        /// <returns></returns>
        protected virtual bool DoSmoothHeights()
        {
            return false;
        }

        /// <summary>
        /// Create the map. Update to derivered class to implement.
        /// </summary>
        protected abstract void CreateMap();

        /// <summary>
        /// Load the provided height map.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bigendian"></param>
        /// <returns></returns>
        protected float[] Load16Bit(string fileName, bool bigendian = false)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(fileName);

            int size = bytes.Length / 2;
            float[] data = new float[size];

            for (int x = 0, i = 0; x < size; x++)
            {
                data[x] = (bigendian) ? (bytes[i++] * 256.0f + bytes[i++]) : (bytes[i++] + bytes[i++] * 256.0f);
                data[x] /= ushort.MaxValue;
            }

            return data;
        }

        /// <summary>
        /// Get a hight value ranging from 0 - 1.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected float GetNormalizedHeight(int x, int y)
        {
            x = Mathf.Clamp(x, 0, m_width - 1);
            y = Mathf.Clamp(y, 0, m_height - 1);

            return m_heights[x + y * m_width];
        }

        /// <summary>
        /// Get a hight value ranging from 0 - actaul height in meters.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected float GetHeight(int x, int y)
        {
            return GetNormalizedHeight(x, y) * m_terrainHeight;
        }

        /// <summary>
        /// Get the heigts maps first derivative using Evans-Young method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected Vector2 GetFirstDerivative(int x, int y)
        {
            float w = m_cellLength;
            float z1 = GetHeight(x - 1, y + 1);
            float z2 = GetHeight(x + 0, y + 1);
            float z3 = GetHeight(x + 1, y + 1);
            float z4 = GetHeight(x - 1, y + 0);
            float z6 = GetHeight(x + 1, y + 0);
            float z7 = GetHeight(x - 1, y - 1);
            float z8 = GetHeight(x + 0, y - 1);
            float z9 = GetHeight(x + 1, y - 1);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w);

            return new Vector2(-zx, -zy);
        }

        /// <summary>
        /// Get the heigts maps first and second derivative using Evans-Young method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        protected void GetDerivatives(int x, int y, out Vector2 d1, out Vector3 d2)
        {
            float w = m_cellLength;
            float w2 = w * w;
            float z1 = GetHeight(x - 1, y + 1);
            float z2 = GetHeight(x + 0, y + 1);
            float z3 = GetHeight(x + 1, y + 1);
            float z4 = GetHeight(x - 1, y + 0);
            float z5 = GetHeight(x + 0, y + 0);
            float z6 = GetHeight(x + 1, y + 0);
            float z7 = GetHeight(x - 1, y - 1);
            float z8 = GetHeight(x + 0, y - 1);
            float z9 = GetHeight(x + 1, y - 1);

            //p, q
            float zx = (z3 + z6 + z9 - z1 - z4 - z7) / (6.0f * w);
            float zy = (z1 + z2 + z3 - z7 - z8 - z9) / (6.0f * w);

            //r, t, s
            float zxx = (z1 + z3 + z4 + z6 + z7 + z9 - 2.0f * (z2 + z5 + z8)) / (3.0f * w2);
            float zyy = (z1 + z2 + z3 + z7 + z8 + z9 - 2.0f * (z4 + z5 + z6)) / (3.0f * w2);
            float zxy = (z3 + z7 - z1 - z9) / (4.0f * w2);

            d1 = new Vector2(-zx, -zy);
            d2 = new Vector3(-zxx, -zyy, -zxy); //is zxy or -zxy?
        }

        /// <summary>
        /// Smooth heights using a 5X5 Gaussian kernel.
        /// </summary>
        protected void SmoothHeightMap()
        {
            var heights = new float[m_width * m_height];

            var gaussianKernel5 = new float[,]
            {
                {1,4,6,4,1},
                {4,16,24,16,4},
                {6,24,36,24,6},
                {4,16,24,16,4},
                {1,4,6,4,1}
            };

            float gaussScale = 1.0f / 256.0f;

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    float sum = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            int xi = x - 2 + i;
                            int yi = y - 2 + j;

                            sum += GetNormalizedHeight(xi, yi) * gaussianKernel5[i, j] * gaussScale;
                        }
                    }

                    heights[x + y * m_width] = sum;
                }
            }

            m_heights = heights;
        }

        /// <summary>
        /// Take a parameter, rescale it and return as a 
        /// color using a gradient. Helps visualize some 
        /// parameters better especially if they have a 
        /// wide dynamic range and can be negative. 
        /// </summary>
        /// <param name="v">The parameter</param>
        /// <param name="exponent">Amount to rescale the dynamic range. 
        /// Will change if terrain cell length changes.</param>
        /// <param name="nonNegative">If the parameter is always positive</param>
        /// <returns></returns>
        protected Color Colorize(float v, float exponent, bool nonNegative)
        {
            if (exponent > 0)
            {
                float sign = FMath.SignOrZero(v);
                float pow = Mathf.Pow(10, exponent);
                float log = Mathf.Log(1.0f + pow * Mathf.Abs(v));

                v = sign * log;
            }

            if (nonNegative)
                return m_gradient.GetPixelBilinear(v, 0);
            else
            {
                if (v > 0)
                    return m_posGradient.GetPixelBilinear(v, 0);
                else
                    return m_negGradient.GetPixelBilinear(-v, 0);
            }
        }

        private void CreateGradients(bool colored)
        {
            if (colored)
            {
                m_gradient = CreateGradient(VISUALIZE_GRADIENT.COOL_WARM);
                m_posGradient = CreateGradient(VISUALIZE_GRADIENT.WARM);
                m_negGradient = CreateGradient(VISUALIZE_GRADIENT.COOL);
            }
            else
            {
                m_gradient = CreateGradient(VISUALIZE_GRADIENT.BLACK_WHITE);
                m_posGradient = CreateGradient(VISUALIZE_GRADIENT.GREY_WHITE);
                m_negGradient = CreateGradient(VISUALIZE_GRADIENT.GREY_BLACK);
            }

            m_gradient.Apply();
            m_posGradient.Apply();
            m_negGradient.Apply();
        }

        private Texture2D CreateGradient(VISUALIZE_GRADIENT g)
        {
            switch (g)
            {
                case VISUALIZE_GRADIENT.WARM:
                    return CreateWarmGradient();

                case VISUALIZE_GRADIENT.COOL:
                    return CreateCoolGradient();

                case VISUALIZE_GRADIENT.COOL_WARM:
                    return CreateCoolToWarmGradient();

                case VISUALIZE_GRADIENT.GREY_WHITE:
                    return CreateGreyToWhiteGradient();

                case VISUALIZE_GRADIENT.GREY_BLACK:
                    return CreateGreyToBlackGradient();

                case VISUALIZE_GRADIENT.BLACK_WHITE:
                    return CreateBlackToWhiteGradient();
            }

            return null;
        }

        private Texture2D CreateWarmGradient()
        {
            var gradient = new Texture2D(5, 1, TextureFormat.ARGB32, false, true);
            gradient.SetPixel(0, 0, new Color32(80, 230, 80, 255));
            gradient.SetPixel(1, 0, new Color32(180, 230, 80, 255));
            gradient.SetPixel(2, 0, new Color32(230, 230, 80, 255));
            gradient.SetPixel(3, 0, new Color32(230, 180, 80, 255));
            gradient.SetPixel(4, 0, new Color32(230, 80, 80, 255));
            gradient.wrapMode = TextureWrapMode.Clamp;

            return gradient;
        }

        private Texture2D CreateCoolGradient()
        {
            var gradient = new Texture2D(5, 1, TextureFormat.ARGB32, false, true);
            gradient.SetPixel(0, 0, new Color32(80, 230, 80, 255));
            gradient.SetPixel(1, 0, new Color32(80, 230, 180, 255));
            gradient.SetPixel(2, 0, new Color32(80, 230, 230, 255));
            gradient.SetPixel(3, 0, new Color32(80, 180, 230, 255));
            gradient.SetPixel(4, 0, new Color32(80, 80, 230, 255));
            gradient.wrapMode = TextureWrapMode.Clamp;

            return gradient;
        }

        private Texture2D CreateCoolToWarmGradient()
        {
            var gradient = new Texture2D(9, 1, TextureFormat.ARGB32, false, true);
            gradient.SetPixel(0, 0, new Color32(80, 80, 230, 255));
            gradient.SetPixel(1, 0, new Color32(80, 180, 230, 255));
            gradient.SetPixel(2, 0, new Color32(80, 230, 230, 255));
            gradient.SetPixel(3, 0, new Color32(80, 230, 180, 255));
            gradient.SetPixel(4, 0, new Color32(80, 230, 80, 255));
            gradient.SetPixel(5, 0, new Color32(180, 230, 80, 255));
            gradient.SetPixel(6, 0, new Color32(230, 230, 80, 255));
            gradient.SetPixel(7, 0, new Color32(230, 180, 80, 255));
            gradient.SetPixel(8, 0, new Color32(230, 80, 80, 255));
            gradient.wrapMode = TextureWrapMode.Clamp;

            return gradient;
        }

        private Texture2D CreateGreyToWhiteGradient()
        {
            var gradient = new Texture2D(3, 1, TextureFormat.ARGB32, false, true);
            gradient.SetPixel(0, 0, new Color32(128, 128, 128, 255));
            gradient.SetPixel(1, 0, new Color32(192, 192, 192, 255));
            gradient.SetPixel(2, 0, new Color32(255, 255, 255, 255));
            gradient.wrapMode = TextureWrapMode.Clamp;

            return gradient;
        }

        private Texture2D CreateGreyToBlackGradient()
        {
            var gradient = new Texture2D(3, 1, TextureFormat.ARGB32, false, true);
            gradient.SetPixel(0, 0, new Color32(128, 128, 128, 255));
            gradient.SetPixel(1, 0, new Color32(64, 64, 64, 255));
            gradient.SetPixel(2, 0, new Color32(0, 0, 0, 255));
            gradient.wrapMode = TextureWrapMode.Clamp;

            return gradient;
        }

        private Texture2D CreateBlackToWhiteGradient()
        {
            var gradient = new Texture2D(5, 1, TextureFormat.ARGB32, false, true);
            gradient.SetPixel(0, 0, new Color32(0, 0, 0, 255));
            gradient.SetPixel(1, 0, new Color32(64, 64, 64, 255));
            gradient.SetPixel(2, 0, new Color32(128, 128, 128, 255));
            gradient.SetPixel(3, 0, new Color32(192, 192, 192, 255));
            gradient.SetPixel(4, 0, new Color32(255, 255, 255, 255));
            gradient.wrapMode = TextureWrapMode.Clamp;

            return gradient;
        }

    }

}
