using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainTopology
{
    public enum CURVATURE_TYPE
    {
        PLAN, HORIZONTAL, VERTICAL, MEAN, GAUSSIAN, MINIMAL, MAXIMAL, UNSPHERICITY, ROTOR,
        DIFFERENCE, HORIZONTAL_EXCESS, VERTICAL_EXCESS, RING, ACCUMULATION
    };

    public class CreateCurvatureMap : CreateTopology
    {

        public CURVATURE_TYPE m_curvatureType = CURVATURE_TYPE.HORIZONTAL;

        private CURVATURE_TYPE m_currentType;

        protected override bool OnChange()
        {
            return m_currentType != m_curvatureType || m_currentColorMode != m_coloredGradient;
        }

        /// <summary>
        /// Since curvature uses the second derivatives it can be sensitive to noise.
        /// For best results smooth the heights to reduce noise.
        /// </summary>
        /// <returns></returns>
        protected override bool DoSmoothHeights()
        {
            return true;
        }

        protected override void CreateMap()
        {
            m_currentType = m_curvatureType;

            Texture2D curveMap = new Texture2D(m_width, m_height);

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    Vector2 d1;
                    Vector3 d2;
                    GetDerivatives(x, y, out d1, out d2);

                    float curvature = 0;
                    Color color = Color.white;

                    switch (m_curvatureType)
                    {
                        case CURVATURE_TYPE.PLAN:
                            curvature = PlanCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 1.5f, false);
                            break;

                        case CURVATURE_TYPE.HORIZONTAL:
                            curvature = HorizontalCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.0f, false);
                            break;

                        case CURVATURE_TYPE.VERTICAL:
                            curvature = VerticalCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.0f, false);
                            break;

                        case CURVATURE_TYPE.MEAN:
                            curvature = MeanCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.4f, false);
                            break;

                        case CURVATURE_TYPE.GAUSSIAN:
                            curvature = GaussianCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 5.0f, false);
                            break;

                        case CURVATURE_TYPE.MINIMAL:
                            curvature = MinimalCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.5f, false);
                            break;

                        case CURVATURE_TYPE.MAXIMAL:
                            curvature = MaximalCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.5f, false);
                            break;

                        case CURVATURE_TYPE.UNSPHERICITY:
                            curvature = UnsphericityCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.0f, true);
                            break;

                        case CURVATURE_TYPE.ROTOR:
                            curvature = RotorCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.5f, false);
                            break;

                        case CURVATURE_TYPE.DIFFERENCE:
                            curvature = DifferenceCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.0f, false);
                            break;

                        case CURVATURE_TYPE.HORIZONTAL_EXCESS:
                            curvature = HorizontalExcessCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.0f, true);
                            break;

                        case CURVATURE_TYPE.VERTICAL_EXCESS:
                            curvature = VerticalExcessCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 2.0f, true);
                            break;

                        case CURVATURE_TYPE.RING:
                            curvature = RingCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 5.0f, true);
                            break;

                        case CURVATURE_TYPE.ACCUMULATION:
                            curvature = AccumulationCurvature(d1.x, d1.y, d2.x, d2.y, d2.z);
                            color = Colorize(curvature, 5.0f, false);
                            break;
                    };

                    curveMap.SetPixel(x, y, color);
                }

            }

            curveMap.Apply();
            m_material.mainTexture = curveMap;
        }

        /// <summary>
        /// Kp
        /// Plan curvature measures topographic convergence or divergence.
        /// Is positive for diverging flows on ridges and negative converging flows in valleys.
        /// </summary>
        private float PlanCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float zx2 = zx * zx;
            float zy2 = zy * zy;
            float p = zx2 + zy2;

            float n = zy2 * zxx - 2.0f * zxy * zx * zy + zx2 * zyy;
            float d = Mathf.Pow(p, 1.5f);

            return FMath.SafeDiv(n, d);
        }

        /// <summary>
        /// Kh
        /// Same as plan curvature but multiplied by the sine of the slope angle.
        /// Does not take on extremely large values when slope is small.
        /// aka Tangential curvature.
        /// </summary>
        private float HorizontalCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float zx2 = zx * zx;
            float zy2 = zy * zy;
            float p = zx2 + zy2;

            float n = zy2 * zxx - 2.0f * zxy * zx * zy + zx2 * zyy;
            float d = p * Mathf.Pow(p + 1, 0.5f);

            return FMath.SafeDiv(n, d);
        }

        /// <summary>
        /// Kv
        /// Vertical curvature measures the rate of change of the slope.
        /// Is negative for slope increasing downhill and positive for slope decreasing dowhill.
        /// aka profile curvature.
        /// </summary>
        private float VerticalCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float zx2 = zx * zx;
            float zy2 = zy * zy;
            float p = zx2 + zy2;

            float n = zx2 * zxx + 2.0f * zxy * zx * zy + zy2 * zyy;
            float d = p * Mathf.Pow(p + 1, 1.5f);

            return FMath.SafeDiv(n, d);
        }

        /// <summary>
        /// H
        /// Mean curvature represents convergence and relative deceleration with equal weights.
        /// </summary>
        private float MeanCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float zx2 = zx * zx;
            float zy2 = zy * zy;
            float p = zx2 + zy2;

            float n = (1 + zy2) * zxx - 2.0f * zxy * zx * zy + (1 + zx2) * zyy;
            float d = 2 * Mathf.Pow(p + 1, 1.5f);

            return FMath.SafeDiv(n, d);
        }

        /// <summary>
        /// K
        /// Gaussian curvature retains values in each point on the surface after
        /// its bending without breaking, stretching, and compressing.
        /// </summary>
        private float GaussianCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float zx2 = zx * zx;
            float zy2 = zy * zy;
            float p = zx2 + zy2;

            float n = zxx * zyy - zxy * zxy;
            float d = Mathf.Pow(p + 1, 2f);

            return FMath.SafeDiv(n, d);
        }

        /// <summary>
        /// Kmin
        /// Curvature of the principal section with the lowest value.
        /// </summary>
        private float MinimalCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float H = MeanCurvature(zx, zy, zxx, zyy, zxy);
            float K = GaussianCurvature(zx, zy, zxx, zyy, zxy);

            return H - FMath.SafeSqrt(H * H - K);
        }

        /// <summary>
        /// M
        /// Shows the extent to which the shape of the surface is nonspherical.
        /// Non-negative.
        /// </summary>
        private float UnsphericityCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float H = MeanCurvature(zx, zy, zxx, zyy, zxy);
            float K = GaussianCurvature(zx, zy, zxx, zyy, zxy);

            return FMath.SafeSqrt(H * H - K);
        }

        /// <summary>
        /// Kmax
        /// Curvature of the principal section with the highest value.
        /// </summary>
        private float MaximalCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float H = MeanCurvature(zx, zy, zxx, zyy, zxy);
            float K = GaussianCurvature(zx, zy, zxx, zyy, zxy);

            return H + FMath.SafeSqrt(H * H - K);
        }

        /// <summary>
        /// Krot
        /// Flow lines turn clockwise if rot is positive.
        /// </summary>
        private float RotorCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float zx2 = zx * zx;
            float zy2 = zy * zy;
            float p = zx2 + zy2;

            float n = (zx2 - zy2) * zxy - zx * zy * (zxx - zyy);
            float d = Mathf.Pow(p, 1.5f);

            return FMath.SafeDiv(n, d);
        }

        /// <summary>
        /// E
        /// Shows what extent the relative deceleration of flows is higher
        /// than the convergence at a given point.
        /// </summary>
        private float DifferenceCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float Kv = VerticalCurvature(zx, zy, zxx, zyy, zxy);
            float Kh = HorizontalCurvature(zx, zy, zxx, zyy, zxy);

            return 0.5f * (Kv - Kh);
        }

        /// <summary>
        /// Khe
        /// Shows what extent the bending of a normal section tangential to a contour line
        /// is larger that the minimal bending.
        /// Non-negative.
        /// </summary>
        private float HorizontalExcessCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float Kh = HorizontalCurvature(zx, zy, zxx, zyy, zxy);
            float Kmin = MinimalCurvature(zx, zy, zxx, zyy, zxy);

            return Kh - Kmin;
        }

        /// <summary>
        /// Kve
        /// Shows what extent the bending of a normal section having a common tangent line
        /// with a slope line is larger than the minimal bending.
        /// Non-negative.
        /// </summary>
        private float VerticalExcessCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float Kv = VerticalCurvature(zx, zy, zxx, zyy, zxy);
            float Kmin = MinimalCurvature(zx, zy, zxx, zyy, zxy);

            return Kv - Kmin;
        }

        /// <summary>
        /// Kr
        /// Describes flow lines twisting but does nor consider direction.
        /// Non-negative.
        /// </summary>
        private float RingCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float H = MeanCurvature(zx, zy, zxx, zyy, zxy);
            float K = GaussianCurvature(zx, zy, zxx, zyy, zxy);
            float Kh = HorizontalCurvature(zx, zy, zxx, zyy, zxy);

            return 2 * H * Kh - Kh * Kh - K;
        }
        /// <summary>
        /// Ka
        /// A measure of the extent of the flow accumulation.
        /// </summary>
        private float AccumulationCurvature(float zx, float zy, float zxx, float zyy, float zxy)
        {
            float Kh = HorizontalCurvature(zx, zy, zxx, zyy, zxy);
            float Kv = VerticalCurvature(zx, zy, zxx, zyy, zxy);

            return Kh * Kv;
        }

    }

}
